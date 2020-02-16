using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSMobileApplication.Resources;

namespace JLSDataAccess.Repositories
{
    public class ReferenceRepository : IReferenceRepository
    {
        private readonly JlsDbContext db;

        public ReferenceRepository(JlsDbContext context)
        {
            db = context;
        }
        public Task<List<ReferenceItem>> GetReferenceItemsByCategoryIds(string categoryIds, string lang)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ReferenceItemViewModel>> GetReferenceItemsByCategoryLabels(string shortLabels, string lang)
        {
            List<string> referenceLabelList = new List<string>(shortLabels.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));

            var result =  (from ri in db.ReferenceItem
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          from rl in db.ReferenceLabel.Where(p => p.ReferenceItemId == ri.Id && p.Lang == lang).DefaultIfEmpty()
                          where referenceLabelList.Contains(rc.ShortLabel)
                          select new ReferenceItemViewModel()
                          {
                              Id = ri.Id,
                              Code = ri.Code,
                              ParentId = ri.ParentId,
                              ReferenceParent = (from rip in db.ReferenceItem
                                                 join rcp in db.ReferenceCategory on rip.ReferenceCategoryId equals rcp.Id
                                                 join rlp in db.ReferenceLabel on rip.Id equals rlp.ReferenceItemId
                                                 where rip.Id == ri.ParentId
                                                 select new ReferenceItemViewModel()
                                                 {
                                                     Id = rip.Id,
                                                     Code = rip.Code,
                                                     Value = rip.Value,
                                                     Order = rip.Order,
                                                     ReferenceCategoryId = rcp.Id,
                                                     ReferenceCategoryLongLabel = rcp.LongLabel,
                                                     Label = rlp.Label,
                                                     Validity = rip.Validity
                                                 }).FirstOrDefault(),
                              Value = ri.Value,
                              Order  = ri.Order,
                              ReferenceCategoryId = rc.Id,
                              ReferenceCategoryLongLabel = rc.LongLabel,
                              Label = rl.Label,
                              Validity = ri.Validity
                          });
            return await result.ToListAsync();
        }

     

        public Task<List<ReferenceItem>> GetReferenceItemsByCode(string referencecode, string lang)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReferenceItem>> GetReferenceItemsById(long referenceId, string lang)
        {
            throw new NotImplementedException();
        }
    }
}
