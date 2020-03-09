using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.AdminViewModel;
using JLSDataModel.Models;
using JLSDataModel.Models.Product;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace JLSMobileApplication.Controllers.AdminService
{
    [Route("admin/[controller]/{action}")]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private IReferenceRepository _referenceRepository;
        private readonly IMapper _mapper;
        public ProductController(IProductRepository productRepository, IReferenceRepository referenceRepository, IMapper mapper)
        {
            this._referenceRepository = referenceRepository;
            this._productRepository = productRepository;
            _mapper = mapper;
        }

        //[HttpPost]
        //public  Task<long> RemoveImageById([FromBody]long id)
        ////{
        ////    int res = await this._productRepository.RemoveImageById(id);
        ////    ApiResult result;
        ////    if (res == 1)
        ////    {
        ////        result = new ApiResult() { Success = true, Msg = "OK", Type = "200" };
        ////    }
        ////    else
        ////    {
        ////        result = new ApiResult() { Success = false, Msg = "Fail", Type = "500" };
        ////    }

        ////    return Json(result);
        //    return 0;
        //}


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
            public string Labelfr { get; set; }
            public string Labelen { get; set; }
            public string Labelcn { get; set; }

            public string ReferenceCode { get; set; }

            public string Description { get; set; }

            public long SecondCategoryId { get; set; }

            public long ProductId { get; set; }

            public long ReferenceCategoryId { get; set; }

            public long ReferenceId { get; set; }

            public int QuantityPerBox { get; set; }

            public int MinQuantity { get; set; }

            public float? Price { get; set; }

            public float? TaxRate { get; set; }

            public bool Validity { get; set; }
        }

        [HttpPost]
        public async Task<long> UpdateOrCreateProduct(UpdateOrCreateProductCriteria criteria)
        {
            try
            {
                var ProductReferenceCategory = await _referenceRepository.GetReferenceCategoryByShortLabel("product");
                if (ProductReferenceCategory != null)
                {
                    long ReferenceId = await _referenceRepository.SaveReferenceItem(criteria.ReferenceId, ProductReferenceCategory.Id, criteria.ReferenceCode, criteria.SecondCategoryId, criteria.Validity, null);

                    if (ReferenceId != 0)
                    {
                        long ProductId = await _productRepository.SaveProductInfo(criteria.ProductId, ReferenceId, criteria.QuantityPerBox, criteria.MinQuantity, criteria.Price, criteria.TaxRate, criteria.Description);
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

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPhoto()
        {
            try
            {
                StringValues ProductId = "";

                var file = Request.Form.Files[0];

                Request.Form.TryGetValue("ProductId", out ProductId); // get ProductId todo change 

                var folderName = Path.Combine("Images", ProductId.ToString());
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                DirectoryInfo di = Directory.CreateDirectory(pathToSave); // 创建路径

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
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
               var data = await _productRepository.GetProductById(Id);
                return Json(data);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}