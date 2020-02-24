using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSConsoleApplication.Resources;
using JLSDataAccess.Interfaces;
using JLSDataModel.AdminViewModel;
using JLSDataModel.Models;
using JLSDataModel.Models.Product;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            this._productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<JsonResult> RemoveImageById([FromBody]long id)
        {
            int res = await this._productRepository.RemoveImageById(id);
            ApiResult result;
            if (res == 1)
            {
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200" };
            }
            else
            {
                result = new ApiResult() { Success = false, Msg = "Fail", Type = "500" };
            }

            return Json(result);
        }


        [HttpGet]
        public async Task<JsonResult> GetAllProducts(string lang, int intervalCount, int size, string orderActive, string orderDirection, string filter)
        {
            ApiResult result;
            try
            {
                ListViewModelWithCount<ProductsListViewModel> data = await _productRepository.GetAllProduct(lang, intervalCount, size, orderActive, orderDirection, filter);
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
            }
            catch (Exception e)
            {
                result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductById(long id)
        {
            ApiResult result;
            try
            {
                ProductViewModel data = await _productRepository.GetProductById(id);
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
            }
            catch (Exception e)
            {
                result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> SearchProducts(string lang, string filter)
        {
            ApiResult result;
            try
            {
                List<ProductsListViewModel> data = await _productRepository.SearchProducts(lang, filter);
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
            }
            catch (Exception e)
            {
                result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
            }
            return Json(result);
        }
    

    }
}