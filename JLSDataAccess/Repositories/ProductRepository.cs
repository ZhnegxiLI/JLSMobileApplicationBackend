using JLSDataAccess.Interfaces;
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

        public async Task<List<ProductListData>> GetProductInfoByReferenceIds(List<long> ReferenceIds, string Lang)
        {
            var result = (from ri in db.ReferenceItem
                            join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                            join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                            join product in db.Product on ri.Id equals product.ReferenceItemId
                            where rc.ShortLabel == "Product" && ri.Validity == true && rl.Lang == Lang && ReferenceIds.Contains(ri.Id)
                            select new ProductListData()
                            {
                                ProductId = product.Id,
                                ReferenceId = ri.Id,
                                Code = ri.Code,
                                ParentId = ri.ParentId,
                                Value = ri.Value,
                                Order = ri.Order,
                                Label = rl.Label,
                                Price = product.Price,
                                QuantityPerBox = product.QuantityPerBox,
                                MinQuantity = product.MinQuantity,
                                PhotoPath = (from path in db.ProductPhotoPath
                                            where path.ProductId == product.Id
                                            select new ProductListPhotoPath() { Path = path.Path }).ToList()
                            });
            return await result.ToListAsync();
        }

        public async Task<ProductListViewModel> GetProductListBySecondCategory(long SecondCategoryReferenceId, string Lang, int begin, int step)
        {
            var result = (from ri in db.ReferenceItem
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                                join product in db.Product on ri.Id equals product.ReferenceItemId
                                where rc.ShortLabel == "Product" && ri.Validity == true && rl.Lang == Lang &&       ri.ParentId == SecondCategoryReferenceId
                                select new ProductListData()
                                {
                                    ProductId = product.Id,
                                    ReferenceId = ri.Id,
                                    Code = ri.Code,
                                    ParentId = ri.ParentId,
                                    Value = ri.Value,
                                    Order = ri.Order,
                                    Label = rl.Label,
                                    Price = product.Price,
                                    QuantityPerBox = product.QuantityPerBox,
                                    MinQuantity = product.MinQuantity,
                                    PhotoPath = (from path in db.ProductPhotoPath
                                                    where path.ProductId == product.Id
                                                    select new ProductListPhotoPath() { Path = path.Path }).ToList()
                                });
            var totalCount = result.Count();
            var productList = await result.Skip(begin * step).Take(step).ToListAsync();
            return new ProductListViewModel()
            {
                ProductListData = productList,
                TotalCount = totalCount
            }; 
        }

        public async Task<List<ProductCategoryViewModel>> GetProductMainCategory(string Lang)
        {
            var result = await (from ri in db.ReferenceItem
                          join rip in db.ReferenceItem on ri.ParentId equals rip.Id
                          join rlp in db.ReferenceLabel on rip.Id equals rlp.ReferenceItemId
                          join rcp in db.ReferenceCategory on rip.ReferenceCategoryId equals rcp.Id
                          where rcp.ShortLabel == "MainCategory" && rlp.Lang == Lang && rip.Validity == true
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
                                where rcp.ShortLabel == "SecondCategory" && rip.ParentId ==       MainCategoryReferenceId && rlp.Lang == Lang && rip.Validity == true
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
