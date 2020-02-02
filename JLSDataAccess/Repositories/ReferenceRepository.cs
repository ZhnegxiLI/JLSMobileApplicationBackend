using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<dynamic>> GetReferenceItemsByCategoryLabels(string shortLabels, string lang)
        {
            List<string> referenceLabelList = new List<string>(shortLabels.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));

            var result =  (from ri in db.ReferenceItem
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          from rl in db.ReferenceLabel.Where(p => p.ReferenceItemId == ri.Id && p.Lang == lang).DefaultIfEmpty()
                          where referenceLabelList.Contains(rc.ShortLabel)
                          select new
                          {
                              ri,
                              rc,
                              rl
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
    }
}
