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

        public async Task<ReferenceCategory> GetReferenceCategoryByShortLabel(string ShortLabel)
        {
            return await db.ReferenceCategory.Where(p => p.ShortLabel == ShortLabel).FirstOrDefaultAsync();
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

        public async Task<long> SaveReferenceItem(long ReferenceId, long CategoryId, string Code ,long? ParentId, bool Validity, string Value)
        {  // TODO Add createdBy / createdOn / UpdatedBy
            ReferenceItem ReferenceToUpdateOrCreate = null;
            if (ReferenceId == 0)
            {
                ReferenceToUpdateOrCreate = new ReferenceItem();
            }
            else
            {
                ReferenceToUpdateOrCreate = db.ReferenceItem.Where(p => p.Id == ReferenceId).FirstOrDefault();
            }
            if (ReferenceToUpdateOrCreate != null)
            {
                ReferenceToUpdateOrCreate.ReferenceCategoryId = CategoryId;
                ReferenceToUpdateOrCreate.Code = Code;
                ReferenceToUpdateOrCreate.ParentId = ParentId;
                ReferenceToUpdateOrCreate.Validity = Validity;
                ReferenceToUpdateOrCreate.Value = Value;

                if (ReferenceId == 0)
                {
                    await db.ReferenceItem.AddAsync(ReferenceToUpdateOrCreate);
                }
                else
                {
                    db.ReferenceItem.Update(ReferenceToUpdateOrCreate);
                }
                await db.SaveChangesAsync();
                return ReferenceToUpdateOrCreate.Id;
            }
            return 0;
        }

        public async Task<long> SaveReferenceLabel(long ReferenceId, string Label , string Lang)
        {  // TODO Add createdBy / createdOn / UpdatedBy
            ReferenceLabel ReferenceLabelToUpdateOrCreate = null;
            if (ReferenceId == 0)
            {
                ReferenceLabelToUpdateOrCreate = new ReferenceLabel();
                ReferenceLabelToUpdateOrCreate.Id = ReferenceId;
            }
            else
            {
                ReferenceLabelToUpdateOrCreate = db.ReferenceLabel.Where(p => p.ReferenceItemId == ReferenceId && p.Lang == Lang).FirstOrDefault();
            }
            if (ReferenceLabelToUpdateOrCreate != null)
            {
                ReferenceLabelToUpdateOrCreate.Label = Label;
                ReferenceLabelToUpdateOrCreate.Lang = Lang;

                if (ReferenceId == 0)
                {
                    await db.ReferenceLabel.AddAsync(ReferenceLabelToUpdateOrCreate);
                }
                else
                {
                    db.ReferenceLabel.Update(ReferenceLabelToUpdateOrCreate);
                }
                await db.SaveChangesAsync();
                return ReferenceLabelToUpdateOrCreate.Id;
            }
            return 0;
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
