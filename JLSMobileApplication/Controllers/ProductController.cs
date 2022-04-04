using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Product;
using JLSDataModel.ViewModels;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<JsonResult> GetProductByPrice(string Lang, long? MainCategoryId, int Begin, int Step)
        {
            try
            {
                var result = await _productRepository.GetProductByPrice(Lang, MainCategoryId);

                return Json(new
                {
                    TotalCount = result.Count,
                    List = result.Skip(Begin * Step).Take(Step).ToList()
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductListByPublishDate(string Lang, long? MainCategoryId, int Begin, int Step)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.GetProductListByPublishDate(Lang, MainCategoryId, Begin, Step),
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
        public async Task<JsonResult> GetProductCommentListByCriteria(long? ProductId, long? UserId, int Begin, int Step, string Lang)
        {
            try
            {
                var productComments = await _productRepository.GetProductCommentListByCriteria(ProductId, UserId, Lang);

                List<ProductCommentViewModel> list = new List<ProductCommentViewModel>();
                if (Begin != -1 && Step != -1)
                {
                    list = productComments.Skip(Begin * Step).Take(Step).ToList();
                }
                else
                {
                    list = productComments.ToList();
                }
                return Json(new ApiResult()
                {
                    Data = new
                    {
                        ProductCommentListData = list,
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
        public async Task<JsonResult> GetPromotionProduct(int Begin, int Step, string Lang)
        {
            try
            {
                var productList = await _productRepository.GetPromotionProduct(Lang);
                var list = new List<dynamic>();
                if (Begin != -1 && Step != -1)
                {
                    list = productList.Skip(Begin * Step).Take(Step).ToList();
                }
                else
                {
                    list = productList.ToList();
                }
                return Json(new
                {
                    ProductList = list,
                    TotalCount = productList.Count()
                }
               );
            }
            catch (Exception exc)
            {
                throw exc;
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

        [HttpGet]
        public async Task<JsonResult> SimpleProductSearch(string SearchText, string Lang, int Begin, int Step)
        {
            try
            {
                var result = await _productRepository.SimpleProductSearch(SearchText, Lang);
                return Json(new
                {
                    TotalCount = result.Count,
                    List = result.Skip(Begin * Step).Take(Step).ToList()
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        public class AdvancedSearchCriteria
        {
            public string SearchText { get; set; }
            public long? MainCategory { get; set; }
            public long? SecondCategory { get; set; }
            public int? PriceIntervalLower { get; set; }
            public int? PriceIntervalUpper { get; set; }
            public int? MinQuantity { get; set; }
            public string OrderBy { get; set; }
            public string Lang { get; set; }

            public int Begin { get; set; }
            public int Step { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult> AdvancedProductSearchClient(AdvancedSearchCriteria criteria)
        {
            try
            {
                var result = await _productRepository.AdvancedProductSearchClient(criteria.SearchText, criteria.MainCategory, criteria.SecondCategory, criteria.PriceIntervalLower, criteria.PriceIntervalUpper, criteria.MinQuantity, criteria.OrderBy, criteria.Lang);
                return Json(new
                {
                    TotalCount = result.Count,
                    List = result.Skip(criteria.Begin * criteria.Step).Take(criteria.Step).ToList()
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductListByNote(string Lang, int Begin, int Step)
        {
            try
            {
                var result = await _productRepository.GetProductListByNote(Lang);
                return Json(new
                {
                    TotalCount = result.Count(),
                    List = result.Skip(Begin * Step).Take(Step).ToList()
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetProductById(long ProductId, string Lang, int? UserId)
        {
            try
            {
                var data = await _productRepository.GetProductById(ProductId, Lang, UserId);
                return Json(data);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetFavoriteListByUserId(int UserId, string Lang, int? Step, int? Begin)
        {
            try
            {
                var favoriteProductList = await _productRepository.GetFavoriteListByUserId(UserId, Lang);
                var totalCount = favoriteProductList.Count();
                var result = favoriteProductList;
                if (Step != null && Begin != null)
                {
                    result = favoriteProductList.Skip((int)Begin * (int)Step).Take((int)Step).ToList();
                }
                return Json(new
                {
                    TotalCount = totalCount,
                    List = result
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        [HttpGet]
        public async Task<JsonResult> AddIntoProductFavoriteList(int UserId, long ProductId, bool? IsFavorite)
        {
            try
            {
                var favoriteProductList = await _productRepository.AddIntoProductFavoriteList(UserId, ProductId, IsFavorite);
                return Json(favoriteProductList);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        /******************************** ZOOM Service with authentification *******************************/
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> SaveProductComment([FromBody] ProductComment comment)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.SaveProductComment(comment.ProductId, comment.Title, comment.Body, comment.Level, comment.UserId),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /* Only for web site */
        [HttpGet]
        public async Task<JsonResult> GetCategoryForWebSite(int NumberOfCateogry, string Lang)
        {
            try
            {
                var result = await _productRepository.GetCategoryForWebSite(Lang);
                if (NumberOfCateogry != -1)
                {
                    result = result.Take(NumberOfCateogry).ToList();
                }
                return Json(result);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetMainPageForWebSite(string Lang)
        {
            int Step = 10;
            int Begin = 0;
            try
            {
                var productByPublishDate = await _productRepository.GetProductListByPublishDate(Lang, null, Begin, Step);
                var productBySalesPerformance = await _productRepository.GetProductListBySalesPerformance(Lang, Begin, Step);
                var productByPrice = (await _productRepository.GetProductByPrice(Lang, null)).Take(Step).ToList();


                return Json(new
                {
                    resultByPublishDate = productByPublishDate,
                    resultBySalesPerformance = productBySalesPerformance,
                    resultByPrice = productByPrice
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}
