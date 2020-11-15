using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess;
using JLSDataAccess.Interfaces;
using JLSDataModel.AdminViewModel;
using JLSDataModel.Models;
using JLSDataModel.Models.Product;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace JLSMobileApplication.Controllers.AdminService
{
    [Authorize]
    [Route("admin/[controller]/{action}")]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private IReferenceRepository _referenceRepository;
        private readonly IMapper _mapper;
        private readonly JlsDbContext db;

        public ProductController(IProductRepository productRepository, IReferenceRepository referenceRepository, IMapper mapper, JlsDbContext context)
        {
            this._referenceRepository = referenceRepository;
            this._productRepository = productRepository;
            _mapper = mapper;
            db = context;
        }

        [HttpPost]
        public async Task<long> RemoveImageById([FromBody]long Id)
        {
            ProductPhotoPath image = await db.ProductPhotoPath.FindAsync(Id);

            if (image == null)
            {
                return 0;
            }

            string imagePath = "images/" + image.Path; // todo place into the configuration

            try
            {
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                var PhotoId = image.Id;
                db.ProductPhotoPath.Remove(image);

                await db.SaveChangesAsync();

                return PhotoId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public async Task<long> RemoveProductCommentById(long CommentId)
        {
            try
            {
                var productCommentId = await _productRepository.RemoveProductCommentById(CommentId);
                return productCommentId;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        public class AdvancedProductSearchCriteria {

            public AdvancedProductSearchCriteria()
            {
                this.SecondCategoryReferenceId = new List<long>();
            }

            public string ProductLabel { get; set; }
            public long MainCategoryReferenceId { get; set; }
            public List<long> SecondCategoryReferenceId { get; set; }

            public bool? Validity { get; set; }
            public string Lang { get; set; }

            public int begin { get; set; }

            public int step { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult> AdvancedProductSearchByCriteria(AdvancedProductSearchCriteria criteria )
        {
            try
            {
               var result = await _productRepository.AdvancedProductSearchByCriteria(criteria.ProductLabel, criteria.MainCategoryReferenceId, criteria.SecondCategoryReferenceId, criteria.Validity, criteria.Lang);
                var list = result.Skip(criteria.begin * criteria.step).Take(criteria.step);

                return Json(new { 
                    ProductList = list,
                    TotalCount = result.Count()
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public class UpdateOrCreateProductCriteria
        {
            public int? CreatedOrUpdatedBy { get; set; }
            public string Labelfr { get; set; }
            public string Labelen { get; set; }
            public string Labelcn { get; set; }

            public string ReferenceCode { get; set; }

            public string Description { get; set; }

            public long SecondCategoryId { get; set; }

            public long ProductId { get; set; }

            public long ReferenceCategoryId { get; set; }

            public long ReferenceId { get; set; }

            public int? QuantityPerBox { get; set; }

            public int? MinQuantity { get; set; }

            public float? Price { get; set; }

            public long? TaxRateId { get; set; }

            public bool Validity { get; set; }

            public string Color { get; set; }

            public string Material { get; set; }

            public string Size { get; set; }

            public string Forme { get; set; }
        }

        [HttpPost]
        public async Task<long> UpdateOrCreateProduct(UpdateOrCreateProductCriteria criteria)
        {
            try
            {
                var ProductReferenceCategory = await _referenceRepository.GetReferenceCategoryByShortLabel("product");
                if (ProductReferenceCategory != null)
                {
                    long ReferenceId = await _referenceRepository.SaveReferenceItem(criteria.ReferenceId, ProductReferenceCategory.Id, criteria.ReferenceCode, criteria.SecondCategoryId, criteria.Validity, null, criteria.CreatedOrUpdatedBy);

                    if (ReferenceId != 0)
                    {
                        long ProductId = await _productRepository.SaveProductInfo(criteria.ProductId, ReferenceId, criteria.QuantityPerBox, criteria.MinQuantity, criteria.Price, criteria.TaxRateId, criteria.Description,criteria.Color, criteria.Material, criteria.Size, criteria.Forme,criteria.CreatedOrUpdatedBy);
                        // todo change : SaveReferenceLabel take an list of param and save one time all the three translation
                        long ReferenceLabelFrId = await _referenceRepository.SaveReferenceLabel(ReferenceId, criteria.Labelfr, "fr");
                        long ReferenceLabelEnId = await _referenceRepository.SaveReferenceLabel(ReferenceId, criteria.Labelen, "en");
                        long ReferenceLabelCnId = await _referenceRepository.SaveReferenceLabel(ReferenceId, criteria.Labelcn, "cn");
                        if (ProductId != 0 && ReferenceLabelFrId != 0 && ReferenceLabelEnId != 0 && ReferenceLabelCnId != 0)
                        {
                            return ProductId;
                        }
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public class GetProductInfoByReferenceIdsCriteria
        {
            public GetProductInfoByReferenceIdsCriteria()
            {
                this.ReferenceIds = new List<long>();
            }
            public List<long> ReferenceIds { get; set; }
            public string Lang { get; set; }
        }

        [HttpPost]
        public async Task<JsonResult> GetProductInfoByReferenceIds([FromBody] GetProductInfoByReferenceIdsCriteria criteria)
        {
            try
            {
                return Json(await _productRepository.GetProductInfoByReferenceIds(criteria.ReferenceIds, criteria.Lang));
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPhoto()
        {
            try
            {
                StringValues ProductId = "";

                var file = Request.Form.Files[0];

                Request.Form.TryGetValue("ProductId", out ProductId); // get ProductId todo change 

                var folderName = Path.Combine("Images", ProductId.ToString()); // todo : place into the configruation file
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                DirectoryInfo di = Directory.CreateDirectory(pathToSave); // 创建路径

                if (file.Length > 0)
                {
                   
                    var fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
               
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);


                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    //SavePhotoPath
                    await _productRepository.SavePhotoPath(long.Parse(ProductId), dbPath);
                    return Ok(new { dbPath }); // todo : 改变返回值
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductPhotoPathById(long ProductId)
        {
            var data = await _productRepository.GetProductPhotoPathById(ProductId);
            return Json(data);
        }

 

        [HttpGet]
        public async Task<JsonResult> GetProductById(long Id)
        {
            try
            {
               var data = await _productRepository.GetProductById(Id,"", null);
                return Json(data);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}