using JLSDataAccess.Interfaces;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository product)
        {
            _productRepository = product;
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
        public async Task<JsonResult> GetProductSecondCategory(long MainCategoryReferenceId, string Lang)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _productRepository.GetProductSecondCategory(MainCategoryReferenceId,Lang),
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
