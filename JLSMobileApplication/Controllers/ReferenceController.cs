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

namespace JLSConsoleApplication.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class ReferenceController : Controller
    {
        private readonly IReferenceRepository _referenceRepository;

        public ReferenceController(IReferenceRepository referenceRepository)
        {
            _referenceRepository = referenceRepository;
        }

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

        [HttpGet]
        public async Task<JsonResult> GetAllCategoryList(int step, int begin)
        {
            try
            {
                var result = await _referenceRepository.GetAllCategoryList();
                var totalCount = result.Count();

                List<ReferenceCategory> list = new List<ReferenceCategory>();

                if (step==0 && begin==0)
                {
                    list = result;
                }
                else
                {
                    list = result.Skip(step * begin).Take(step).ToList();
                }


                return Json(new
                {
                    ReferenceCategoryList = list,
                    TotalCount = result.Count()
                });
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public class AdvancedSearchReferenceItemCriteria
        {

            public string SearchText { get; set; }

            public long? ReferenceCategoryId { get; set; }

            public bool? Validity { get; set; }

            public long? ParentId { get; set; }

            public string Lang { get; set; }

            public bool? IgnoreProduct { get; set; }
            public int step { get; set; }

            public int begin { get; set; }
        }

        [HttpPost]
        public async Task<JsonResult> AdvancedSearchReferenceItem(AdvancedSearchReferenceItemCriteria criteria)
        {
            try
            {
                var result = await _referenceRepository.AdvancedSearchReferenceItem(criteria.SearchText,criteria.ReferenceCategoryId,criteria.Validity,criteria.ParentId,criteria.Lang,criteria.IgnoreProduct);
                var totalCount = result.Count();
                var list = result.Skip(criteria.step * criteria.begin).Take(criteria.step);

                return Json(new
                {
                    ReferenceItemList = list,
                    TotalCount = result.Count()
                });
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllReferenceItemWithChildren(string Lang)
        {
            try
            {
                var result = await _referenceRepository.GetAllReferenceItemWithChildren(Lang);
                return Json(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [HttpGet]
        public async Task<bool> CheckReferenceCodeExists(string Code)
        {
            try
            {
                var result = await _referenceRepository.CheckReferenceCodeExists(Code);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public class SaveReferenceItemCriteria
        {
            public int? CreatedOrUpdatedBy { get; set; }
            public long Id { get; set; }
            public long CategoryId { get; set; }
            public string Code { get; set; }
            public long? ParentId { get; set; }
            public bool Validity { get; set; }
            public string Value { get; set; }
            public string LabelFR { get; set; }
            public string LabelCN { get; set; }
            public string LabelEN { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult> SaveReferenceItem([FromBody]SaveReferenceItemCriteria criteria)
        {
            try
            {
                var result = await _referenceRepository.SaveReferenceItem(criteria.Id, criteria.CategoryId,criteria.Code,criteria.ParentId,criteria.Validity,criteria.Value, criteria.CreatedOrUpdatedBy);

                long ReferenceLabelFrId = await _referenceRepository.SaveReferenceLabel(result, criteria.LabelFR, "fr");
                long ReferenceLabelEnId = await _referenceRepository.SaveReferenceLabel(result, criteria.LabelEN, "en");
                long ReferenceLabelCnId = await _referenceRepository.SaveReferenceLabel(result, criteria.LabelCN, "cn");
           

                return Json(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
    }
}