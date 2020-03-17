using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSDataAccess.Interfaces;
using JLSDataModel.AdminViewModel;
using JLSDataModel.Models;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace JLSConsoleApplication.Controllers.AdminService
{
    [Authorize]
    [Route("admin/[controller]/{action}")]
    [ApiController]
    public class ReferenceController : Controller
    {
        private readonly IReferenceRepository _referenceRepository;

        public ReferenceController(IReferenceRepository referenceRepository)
        {
            _referenceRepository = referenceRepository;
        }

        //[HttpGet]
        //public async Task<JsonResult> GetAllReferenceItems(int intervalCount, int size, string orderActive, string orderDirection, string filter)
        //{
        //    ApiResult result;
        //    try
        //    {
        //        ListViewModelWithCount<ReferenceItemViewModel> data = await _referenceRepository.GetReferenceItemWithInterval(intervalCount, size, orderActive, orderDirection, filter);
        //        result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data  };
        //    }
        //    catch (Exception e)
        //    {
        //        result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
        //    }

        //    return Json(result);
        //}

        //[HttpGet]
        //public async Task<JsonResult> GetAllReferenceCategory()
        //{
        //    ApiResult result;
        //    try
        //    {
        //        List<ReferenceCategory> data = await _referenceRepository.GetAllReferenceCategory();
        //        result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
        //    }
        //    catch (Exception e)
        //    {
        //        result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
        //    }

        //    return Json(result);
        //}

        //[HttpGet]
        //public async Task<JsonResult> GetAllValidityReferenceCategory()
        //{
        //    ApiResult result;
        //    try
        //    {
        //        List<ReferenceCategory> data = await _referenceRepository.GetAllValidityReferenceCategory();
        //        result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
        //    }
        //    catch (Exception e)
        //    {
        //        result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
        //    }

        //    return Json(result);
        //}

        
        public class GetReferenceItemsByCategoryLabelsCriteria
        {
            public GetReferenceItemsByCategoryLabelsCriteria()
            {
                this.ShortLabels = new List<string>();
            }
            public List<string> ShortLabels { get; set; }
            public string Lang { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult> GetReferenceItemsByCategoryLabels([FromBody]GetReferenceItemsByCategoryLabelsCriteria criteria)
        {
            try
            {
                var result = await _referenceRepository.GetReferenceItemsByCategoryLabels(criteria.ShortLabels, criteria.Lang);

                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        //[HttpPost]
        //public async Task<JsonResult> CreatorUpdateReferenceCategory([FromBody]ReferenceCategory category)
        //{
        //    int res = await this._referenceRepository.CreatorUpdateCategory(category);
        //    ApiResult result = new ApiResult() { Success = true, Msg = "OK", Type = "200" };
        //    return Json(result);
        //}
    }
}