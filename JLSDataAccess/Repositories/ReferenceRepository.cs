using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSMobileApplication.Resources;
using System.Linq.Expressions;
using JLSDataModel.ViewModels;
using Microsoft.Extensions.Configuration;
using JLSDataModel.AdminViewModel;

namespace JLSDataAccess.Repositories
{
    public class ReferenceRepository : IReferenceRepository
    {
        private readonly JlsDbContext db;


        public ReferenceRepository(JlsDbContext context)
        {
            db = context;
        }

        /*
         *  Mobile Zoom
         */
        public Task<List<ReferenceItem>> GetReferenceItemsByCategoryIds(string categoryIds, string lang)
        {
            throw new NotImplementedException();
        }

        public async Task<List<dynamic>> GetReferenceItemsByCategoryLabels(List<string> shortLabels, string lang)
        {

            var result =  (from ri in db.ReferenceItem
                           join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                           where shortLabels.Contains(rc.ShortLabel)
                           select new 
                          {
                              Id = ri.Id,
                              Code = ri.Code,
                              ReferenceCategoryId = rc.Id,
                              ReferenceCategoryLabel = rc.ShortLabel,
                              Label = (from rl in db.ReferenceLabel
                                       where rl.ReferenceItemId == ri.Id && rl.Lang == lang
                                       select rl.Label).FirstOrDefault(),
                              ParentId = ri.ParentId,
                              Validity = ri.Validity,
                              Value = ri.Value
                          });
            return await result.ToListAsync<dynamic>();
        }

        public Task<List<ReferenceItem>> GetReferenceItemsByCode(string referencecode, string lang)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReferenceItem>> GetReferenceItemsById(long referenceId, string lang)
        {
            throw new NotImplementedException();
        }

        /*
         * Admin Zoom
         */

        // TODO Merge with the mobile version
        public async Task<List<ReferenceItemViewModel>> GetReferenceItemsByCategoryLabelsAdmin(string shortLabels, string lang)
        {
            List<string> referenceLabelList = new List<string>(shortLabels.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));

            var result = (from ri in db.ReferenceItem
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          from rl in db.ReferenceLabel.Where(p => p.ReferenceItemId == ri.Id && p.Lang == lang).DefaultIfEmpty()
                          where referenceLabelList.Contains(rc.ShortLabel)
                          select new ReferenceItemViewModel
                          {
                              Id = ri.Id,
                              Code = ri.Code,
                              Value = ri.Value,
                              Order = ri.Order,
                              ParentId = ri.ParentId,
                              Label = rl.Label,
                              Lang = rl.Lang,
                              Category = rc.ShortLabel,
                              Validity = ri.Validity
                          });
            return await result.ToListAsync<ReferenceItemViewModel>();
        }
        public async Task<List<ReferenceCategory>> GetAllReferenceCategory()
        {
            var result = await (from rc in db.ReferenceCategory
                                select rc).ToListAsync();

            return result;
        }

        public async Task<List<ReferenceCategory>> GetAllValidityReferenceCategory()
        {
            var result = await (from rc in db.ReferenceCategory
                                where rc.Validity == true
                                select rc).ToListAsync();

            return result;
        }


        public async Task<int> CreatorUpdateCategory(ReferenceCategory category)
        {
            if (category.Id == 0)
            {
                db.ReferenceCategory.Add(category);
            }
            else
            {
                db.ReferenceCategory.Update(category);
            }

            await db.SaveChangesAsync();

            return 1;
        }



        public Task<ListViewModelWithCount<ReferenceItemViewModel>> GetReferenceItemWithInterval(int intervalCount, int size, string orderActive, string orderDirection, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
