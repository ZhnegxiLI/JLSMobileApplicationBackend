using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<JsonResult> GetReferenceItemsByCategoryLabels([FromBody] GetReferenceItemsByCategoryLabelsCriteria criteria)
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
        public async Task<JsonResult> GetWbesiteslides()
        {
            try
            {
                var result = await _referenceRepository.GetWbesiteslides();

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

                if (step == 0 && begin == 0)
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

    }
}