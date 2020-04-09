using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Product;
using JLSMobileApplication.Resources;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(IMapper mapper, IProductRepository product)
        {
            _productRepository = product;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<JsonResult> GetProductMainCategory(string Lang)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.GetProductMainCategory(Lang),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductListBySalesPerformance(string Lang, int Begin, int Step)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.GetProductListBySalesPerformance(Lang, Begin, Step),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductListByPublishDate(string Lang, int Begin, int Step)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.GetProductListByPublishDate(Lang, Begin, Step),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductSecondCategory(long MainCategoryReferenceId, string Lang)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.GetProductSecondCategory(MainCategoryReferenceId, Lang),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetProductListBySecondCategory(long SecondCategoryReferenceId, string Lang, int Begin, int Step)
        {
            try
            {
                var productList = await _productRepository.GetProductListBySecondCategory(SecondCategoryReferenceId, Lang, Begin, Step);
            
                return Json(new ApiResult()
                {
                    Data = new
                    {
                        ProductListData = productList.ProductListData,
                        TotalCount = productList.TotalCount
                    },
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetProductCommentListByProductId(long ProductId,int Begin, int Step,string Lang)
        {
            try
            {
                var productComments = await _productRepository.GetProductCommentListByProductId(ProductId, Lang);
                
                return Json(new ApiResult()
                {
                    Data = new
                    {
                        ProductCommentListData = productComments.Skip(Begin * Step).Take(Step).ToList(),
                        TotalCount = productComments.Count()
                    },
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetProductById(long ProductId, string Lang)
        {
            try
            {
                var data = await _productRepository.GetProductById(ProductId, Lang);
                return Json(data);
            }
            catch (Exception e)
            {
                throw e;
            }
        }




        /******************************** ZOOM Service with authentification *******************************/
        [Authorize]
        [HttpGet]
        public async Task<JsonResult> GetProductListBySecondCategoryWithAuth(long SecondCategoryReferenceId, string Lang, int Begin, int Step)
        {
            try
            {
                var productList = await _productRepository.GetProductListBySecondCategory(SecondCategoryReferenceId, Lang, Begin, Step);
              // var result = _mapper.Map<ProductListViewModelWithAuth>(productList.ProductListData);
                return Json(new ApiResult()
                {
                    Data = new {
                        ProductListData = productList.ProductListData,
                        TotalCount = productList.TotalCount
                    },
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveProductComment([FromBody] ProductComment comment)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.SaveProductComment(comment.ProductId,comment.Title,comment.Body,comment.Level,comment.UserId),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}
