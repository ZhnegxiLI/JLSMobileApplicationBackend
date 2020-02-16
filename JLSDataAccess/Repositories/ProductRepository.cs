using JLSDataAccess.Interfaces;
using JLSMobileApplication.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.ViewModels;

namespace JLSDataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly JlsDbContext db;
        public ProductRepository(JlsDbContext context)
        {
            db = context;
        }

        public Task<List<dynamic>> GetProduct(long SecondCategoryReferenceId, string Lang)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductCategoryViewModel>> GetProductMainCategory(string Lang)
        {
            var result = await (from ri in db.ReferenceItem
                          join rip in db.ReferenceItem on ri.ParentId equals rip.Id
                          join rlp in db.ReferenceLabel on rip.Id equals rlp.ReferenceItemId
                          join rcp in db.ReferenceCategory on rip.ReferenceCategoryId equals rcp.Id
                          where rcp.ShortLabel == "MainCategory" && rlp.Lang == Lang
                          group rip by new { rip.Id,rip.Code, rlp.Label} into g
                          select new ProductCategoryViewModel()
                          {
                              TotalCount = g.Count(),
                              ReferenceId  = g.Key.Id,
                              Reference = new SubProductCategoryViewModel() {
                                    ReferenceId = g.Key.Id,
                                    Label = g.Key.Label,
                                    Code = g.Key.Code
                              } 
                          }).ToListAsync();

            return result;
        }

        public async Task<List<ProductCategoryViewModel>> GetProductSecondCategory(long MainCategoryReferenceId, string Lang)
        {
            var result = await (from ri in db.ReferenceItem
                                join rip in db.ReferenceItem on ri.ParentId equals rip.Id
                                join rlp in db.ReferenceLabel on rip.Id equals rlp.ReferenceItemId
                                join rcp in db.ReferenceCategory on rip.ReferenceCategoryId equals rcp.Id
                                where rcp.ShortLabel == "SecondCategory" && rip.ParentId ==       MainCategoryReferenceId && rlp.Lang == Lang
                                group rip by new { rip.Id, rip.Code, rlp.Label } into g
                                select new ProductCategoryViewModel()
                                {
                                    TotalCount = g.Count(),
                                    ReferenceId = g.Key.Id,
                                    Reference = new SubProductCategoryViewModel()
                                    {
                                        ReferenceId = g.Key.Id,
                                        Label = g.Key.Label,
                                        Code = g.Key.Code
                                    }
                                }).ToListAsync();

            return result;
        }
    }
}
