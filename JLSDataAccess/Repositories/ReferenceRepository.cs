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

        public async Task<long> SaveReferenceItem(long ReferenceId, long CategoryId, string Code ,long? ParentId, bool Validity, string Value, int? CreatedOrUpdatedBy)
        {  // TODO Add createdBy / createdOn / UpdatedBy
            ReferenceItem ReferenceToUpdateOrCreate = null;
            if (ReferenceId == 0)
            {
                ReferenceToUpdateOrCreate = new ReferenceItem();
                ReferenceToUpdateOrCreate.CreatedBy = CreatedOrUpdatedBy;
                ReferenceToUpdateOrCreate.CreatedOn = DateTime.Now;
            }
            else
            {
                ReferenceToUpdateOrCreate = db.ReferenceItem.Where(p => p.Id == ReferenceId).FirstOrDefault();
                ReferenceToUpdateOrCreate.UpdatedBy = CreatedOrUpdatedBy;
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

            var ReferenceLabelToUpdateOrCreate = db.ReferenceLabel.Where(p => p.ReferenceItemId == ReferenceId && p.Lang == Lang).FirstOrDefault();

            if (ReferenceLabelToUpdateOrCreate == null)
            {
                ReferenceLabelToUpdateOrCreate = new ReferenceLabel();
                ReferenceLabelToUpdateOrCreate.ReferenceItemId = ReferenceId;
            }

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




        public async Task<List<ReferenceCategory>> GetAllCategoryList()
        {
            var result = await db.ReferenceCategory.ToListAsync();
            return result;
        }

        public async Task<List<dynamic>> AdvancedSearchReferenceItem(string SearchText, long? ReferenceCategoryId,bool? Validity, long? ParentId, string Lang, bool? IgnoreProduct)
        {
            var result = await (from ri in db.ReferenceItem
                                from rl in db.ReferenceLabel.Where(p => p.ReferenceItemId == ri.Id).DefaultIfEmpty()
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                where (rl==null || rl.Lang == Lang) &&
                                (SearchText == "" || ri.Code == SearchText || rl.Label == SearchText) &&
                                (ReferenceCategoryId == null || ri.ReferenceCategoryId == ReferenceCategoryId) &&
                                (Validity == null || ri.Validity == Validity) &&
                                (ParentId == null || ri.ParentId == ParentId ) && 
                                (IgnoreProduct == null || IgnoreProduct == false || (IgnoreProduct == true && rc.ShortLabel!= "Product"))
                                select new
                                {
                                    Id = ri.Id,
                                    Label = rl.Label,
                                    Value = ri.Value,
                                    Validity = ri.Validity,
                                    Code = ri.Code,
                                    CategoryId = ri.ReferenceCategoryId,
                                    Category = (from category in db.ReferenceCategory
                                                where category.Id == ri.ReferenceCategoryId
                                                select new
                                                {
                                                    Id = category.Id,
                                                    ShortLabel = category.ShortLabel
                                                }).FirstOrDefault(),
                                    Labels = (from l in db.ReferenceLabel
                                              where l.ReferenceItemId == ri.Id
                                              select new
                                              {
                                                  Id = l.Id,
                                                  Lang = l.Lang,
                                                  Label = l.Label
                                              }).ToList(),
                                    ParentId = ri.ParentId,
                                    ParentReferenceItem = (from rip in db.ReferenceItem
                                                           join rlp in db.ReferenceLabel on rip.Id equals rlp.ReferenceItemId
                                                           where rip.Id == ri.ParentId && rlp.Lang == Lang
                                                           select new
                                                           {
                                                               Id = rip.Id,
                                                               Code = rip.Code,
                                                               Label = rlp.Label,
                                                               CategoryId = rip.ReferenceCategoryId
                                                           }).FirstOrDefault(),
                                }).ToListAsync<dynamic>();
                          
            return result;
        }

        public async Task<List<dynamic>> GetAllReferenceItemWithChildren(string Lang)
        {
            var result = await (from ri in db.ReferenceItem
                                join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                where rl.Lang == Lang && rc.ShortLabel != "Product" // todo place all into an  configuration table
                                select new {
                                    Id = ri.Id,
                                    Code = ri.Code,
                                    Label = rl.Label,
                                    CategoryId = ri.ReferenceCategoryId
                                }).Distinct().ToListAsync<dynamic>();
            return result;
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


        public async Task<bool> CheckReferenceCodeExists(string Code)
        {
            var result = await db.ReferenceItem.Where(p => p.Code == Code).FirstOrDefaultAsync();

            return result!=null ? true : false;
        }


        public Task<ListViewModelWithCount<ReferenceItemViewModel>> GetReferenceItemWithInterval(int intervalCount, int size, string orderActive, string orderDirection, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
