using JLSDataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.ViewModels;
using JLSDataModel.Models.Product;
using Microsoft.AspNetCore.Http;
using JLSDataModel.Models;
using System.IO;
using System.Linq.Expressions;
using JLSDataModel.AdminViewModel;

namespace JLSDataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly JlsDbContext db;

        public ProductRepository(JlsDbContext context)
        {
            db = context;
        }

        /*
         * Mobile Zoom
         */
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

        public async Task<ProductListViewModel> GetProductListByPublishDate(string Lang, int begin, int step)
        {
            var result = (from ri in db.ReferenceItem
                          join riSecond in db.ReferenceItem on ri.ParentId equals riSecond.Id
                          join riMain in db.ReferenceItem on riSecond.ParentId equals riMain.Id
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                          join product in db.Product on ri.Id equals product.ReferenceItemId
                          where rc.ShortLabel == "Product" && ri.Validity == true && rl.Lang == Lang
                          && riSecond.Validity == true && riMain.Validity == true
                          orderby ri.CreatedOn descending, rc.Id, rl.Label
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

        // By sales performance // todo: by every month
        public async Task<ProductListViewModel> GetProductListBySalesPerformance(string Lang, int begin, int step)
        {
            var result = (from ri in db.ReferenceItem
                          join riSecond in db.ReferenceItem on ri.ParentId equals riSecond.Id
                          join riMain in db.ReferenceItem on riSecond.ParentId equals riMain.Id
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                          join product in db.Product on ri.Id equals product.ReferenceItemId
                          from op in db.OrderProduct.Where(p => p.ReferenceId == ri.Id).DefaultIfEmpty()
                          where rc.ShortLabel == "Product" && ri.Validity == true && rl.Lang == Lang
                          && riSecond.Validity == true && riMain.Validity == true
                          group op by new { ri.Id, productId = product.Id, ri.ParentId, ri.Code, ri.Value, rl.Label, product.Price, product.QuantityPerBox, product.MinQuantity } into g
                          orderby g.Sum(x => x.Quantity) descending
                          select new ProductListData()
                          {
                              ReferenceId = g.Key.Id,
                              ProductId = g.Key.productId,
                              Code = g.Key.Code,
                              ParentId = g.Key.ParentId,
                              Value = g.Key.Value,
                              Label = g.Key.Label,
                              Price = g.Key.Price,
                              QuantityPerBox = g.Key.QuantityPerBox,
                              MinQuantity = g.Key.MinQuantity,
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

        public async Task<long> SaveProductComment(long ProductId, string Title, string Body, int Level, int UserId)
        {// TODO: change modify
            var ProductComment = new ProductComment();
            ProductComment.Title = Title;
            ProductComment.Body = Body;
            ProductComment.ProductId = ProductId;
            ProductComment.Level = Level;
            ProductComment.CreatedOn = DateTime.Now;
            ProductComment.UserId = UserId;
            ProductComment.CreatedBy = UserId;

            await db.ProductComment.AddAsync(ProductComment);
            await db.SaveChangesAsync();

            return ProductComment.Id;
        }

        public async Task<List<ProductComment>> GetAllProductCommentList(int begin, int step)
        {
            var result = await (from pc in db.ProductComment
                                orderby pc.CreatedOn
                                select pc).Skip(begin * step).Take(step).ToListAsync();
            return result;
        }

        public async Task<List<ProductCommentViewModel>> GetProductCommentListByProductId(long ProductId,string Lang)
        {
            var result = await (from pc in db.ProductComment
                                where pc.ProductId == ProductId
                                orderby pc.CreatedOn
                                select new ProductCommentViewModel() {
                                    User= (from u in db.Users
                                           where u.Id == pc.UserId
                                           select u).FirstOrDefault(),
                                    ProductComment = pc,
                                    PhotoPath = (from pp in db.ProductPhotoPath
                                                 where pp.ProductId == pc.ProductId
                                                 select pp.Path).FirstOrDefault(),
                                    Label = (from p in db.Product
                                             join rl in db.ReferenceLabel on p.ReferenceItemId equals rl.ReferenceItemId
                                             where rl.Lang == Lang && p.Id == pc.ProductId
                                             select rl.Label).FirstOrDefault()

                                }).ToListAsync();
            return result;
        }
        // Todo change
        public async Task<List<ProductComment>> GetProductCommentListByUserId(int UserId, int begin, int step)
        {
            var result = await (from pc in db.ProductComment
                                where pc.UserId == UserId
                                orderby pc.CreatedOn
                                select pc).Skip(begin * step).Take(step).ToListAsync();
            return result;
        }


        /* TODO: change */
        public async Task<List<dynamic>> SearchProductByLabel(string Label, string Lang, int begin, int step)
        {
            var result = await (from rl in db.ReferenceLabel
                                join ri in db.ReferenceItem on rl.ReferenceItemId equals ri.Id
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                where (rc.ShortLabel == "Product" || rc.ShortLabel == "MainCategory" || rc.ShortLabel == "SecondCategory") && rl.Label.Contains(Label) && rl.Lang == Lang
                                select ri).Skip(begin * step).Take(step).ToListAsync<dynamic>();
            return result;
        }


        public async Task<List<dynamic>> AdvancedProductSearchByCriteria(string ProductLabel, long MainCategoryReferenceId, List<long> SecondCategoryReferenceId, bool? Validity ,string Lang)
        {
            var result = await (from rl in db.ReferenceLabel
                                join ri in db.ReferenceItem on rl.ReferenceItemId equals ri.Id
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                join p in db.Product on ri.Id equals p.ReferenceItemId
                                join riSecond in db.ReferenceItem on ri.ParentId equals riSecond.Id
                                join riMain in db.ReferenceItem on riSecond.ParentId equals riMain.Id
        
                                where (ProductLabel == "" || rl.Label.Contains(ProductLabel)) 
                                && (MainCategoryReferenceId==0 || riMain.Id == MainCategoryReferenceId)
                                && (SecondCategoryReferenceId.Count()== 0 || SecondCategoryReferenceId.Contains(riSecond.Id))
                                && (Validity == null ||ri.Validity == Validity)
                                && rl.Lang == Lang && rc.ShortLabel == "Product"
                                select new { 
                                    ReferenceId = ri.Id,
                                    ProductId = p.Id,
                                    Label = rl.Label,
                                    Code = ri.Code,
                                    Validity = ri.Validity,
                                    CategoryId = rc.Id,
                                    CategoryLabel = rc.ShortLabel,
                                    Price = p.Price,
                                    DefaultPhotoPath = (from path in db.ProductPhotoPath
                                                        where path.ProductId == p.Id
                                                        select path.Path).FirstOrDefault(),
                                    MainCategoryLabel = (from rlMain in db.ReferenceLabel
                                                    where rlMain.ReferenceItemId ==  riMain.Id && rlMain.Lang == Lang
                                                    select rlMain.Label).FirstOrDefault(),
                                    SecondCategoryLabel = (from rlSecond in db.ReferenceLabel
                                                    where rlSecond.ReferenceItemId == riSecond.Id && rlSecond.Lang == Lang
                                                    select rlSecond.Label).FirstOrDefault(),
                                }).Distinct().ToListAsync<dynamic>();

            return result;
        }



        public async Task<long> SaveProductInfo(long ProductId, long ReferenceId, int QuantityPerBox, int MinQuantity,float? Price, float? TaxRate, string Description)
        {
            Product ProductToUpdateOrCreate = null;
            if (ProductId == 0)
            {
                ProductToUpdateOrCreate = new Product();
                ProductToUpdateOrCreate.ReferenceItemId = ReferenceId;
            }
            else
            {
                ProductToUpdateOrCreate = db.Product.Where(p => p.Id == ProductId).FirstOrDefault();
            }
            if (ProductToUpdateOrCreate!=null)
            {
                ProductToUpdateOrCreate.QuantityPerBox = QuantityPerBox;
                ProductToUpdateOrCreate.MinQuantity = MinQuantity;
                ProductToUpdateOrCreate.Price = Price;
                ProductToUpdateOrCreate.TaxRate = TaxRate;
                ProductToUpdateOrCreate.Description = Description;

                if (ProductId == 0)
                {
                    await db.Product.AddAsync(ProductToUpdateOrCreate);
                }
                else
                {
                    db.Product.Update(ProductToUpdateOrCreate);
                }
                await db.SaveChangesAsync();
                return ProductToUpdateOrCreate.Id;
            }
            return 0;
        }


        /*
         * Admin Zoom
         */


        private async Task<Boolean> SaveImage(IFormFile image, string path)
        {
            try
            {
                var imagePath = Path.Combine(path, image.FileName);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (FileStream fs = File.Create(imagePath))
                {
                    // 复制文件
                    await image.CopyToAsync(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
            }
            catch (Exception e)
            {
                return false;
            }


            return true;
        }


        public async Task<dynamic> GetProductById(long Id)
        {
            var result = await (from ri in db.ReferenceItem
                                join p in db.Product on ri.Id equals p.ReferenceItemId
                                where p.Id == Id
                                select new 
                                {
                                    ProductId = p.Id,
                                    MainCategoryId =  (from riMain in db.ReferenceItem
                                                       join riSecond in db.ReferenceItem on                                riMain.Id equals riSecond.ParentId
                                                       where riSecond.Id == ri.ParentId
                                                       select riMain.Id).FirstOrDefault(),
                                    SecondCategoryId = ri.ParentId,
                                    ReferenceCode = ri.Code,
                                    MinQuantity = p.MinQuantity,
                                    Price = p.Price,
                                    QuantityPerBox = p.QuantityPerBox,
                                    Description = p.Description,
                                    ReferenceId = ri.Id, 
                                    TaxRate = ri.Value,
                                    Translation = (from label in db.ReferenceLabel
                                                   where label.ReferenceItemId == ri.Id
                                                   select new { 
                                                        Id = label.Id,
                                                        Label= label.Label,
                                                        Lang = label.Lang
                                                   }).ToList(),
                                    ImagesPath = (from path in db.ProductPhotoPath
                                                  where path.ProductId == p.Id
                                                  select path.Path).ToList(),

                                    Color = p.Color,
                             
                                    Material = p.Material,
                                    Size = p.Size,
                                    Validity = ri.Validity
                               
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }
 
            return result;
        }


        public async Task<dynamic> GetProductPhotoPathById(long ProductId)
        {
            var result = await (from pt in db.ProductPhotoPath
                                where pt.ProductId == ProductId
                                select pt).ToListAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<long> SavePhotoPath(long ProductId, string Path)
        {
            var photoPath = db.ProductPhotoPath.Where(p => p.ProductId == ProductId && p.Path == Path).FirstOrDefault();
            if (ProductId>0 && Path!="" && photoPath==null)
            {
                ProductPhotoPath path = new ProductPhotoPath();
                path.ProductId = ProductId;
                path.Path = Path;
                await db.ProductPhotoPath.AddAsync(path);
                await db.SaveChangesAsync();

                return path.Id;
            }
            return 0;
        }


        public async Task<int> RemoveImageById(long id)
        {
            ProductPhotoPath image = await db.ProductPhotoPath.FindAsync(id);

            if (image == null)
            {
                return 0;
            }

            string imagePath = "images/" + image.Path;

            try
            {
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                db.ProductPhotoPath.Remove(image);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
        }

        // TODO change

        public Task<ListViewModelWithCount<ProductsListViewModel>> GetAllProduct(string lang, int intervalCount, int size, string orderActive, string orderDirection, string filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductsListViewModel>> SearchProducts(string lang, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
