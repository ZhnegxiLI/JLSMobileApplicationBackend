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
using JLSDataModel.Models.User;
using System.Data.SqlClient;

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
         // todo adapt mobile and admin
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
                                DefaultPhotoPath = (from path in db.ProductPhotoPath
                                                                       where path.ProductId == product.Id
                                                                       select path.Path).FirstOrDefault(),
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
                                    DefaultPhotoPath = (from path in db.ProductPhotoPath
                                                    where path.ProductId == product.Id
                                                    select path.Path).FirstOrDefault()
                                });
            var totalCount = result.Count();
            var productList = await result.Skip(begin * step).Take(step).ToListAsync();
            return new ProductListViewModel()
            {
                ProductListData = productList,
                TotalCount = totalCount
            }; 
        }
        public async Task<long> RemoveProductCommentById(long ProductCommentId)
        {
            var ProductComment = db.ProductComment.Find(ProductCommentId);
            if (ProductComment!=null)
            {
                db.Remove(ProductComment);
                await db.SaveChangesAsync();

                return ProductComment.Id;
            }
            return 0;
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
                              DefaultPhotoPath = (from path in db.ProductPhotoPath
                                           where path.ProductId == product.Id
                                           select path.Path).FirstOrDefault()
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
        public async Task<dynamic> GetProductListBySalesPerformance(string Lang, int begin, int step)
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
                          select new
                          {
                              ReferenceId = g.Key.Id,
                              ProductId = g.Key.productId,
                              Code = g.Key.Code,
                              ParentId = g.Key.ParentId,
                              Value = g.Key.Value,
                              Label = g.Key.Label,
                              Price = g.Key.Price,
                              QuantityPerBox = g.Key.QuantityPerBox,
                              MinQuantity = g.Key.MinQuantity
                          });
            var totalCount = result.Count();
            var productList = await result.Skip(begin * step).Take(step).ToListAsync();

            var result1 = (from r in productList
                           select new
                           {
                               ReferenceId = r.ReferenceId,
                               ProductId = r.ProductId,
                               Code = r.Code,
                               ParentId = r.ParentId,
                               Value = r.Value,
                               Label = r.Label,
                               Price = r.Price,
                               QuantityPerBox = r.QuantityPerBox,
                               MinQuantity = r.MinQuantity,
                               DefaultPhotoPath = (from pp in db.ProductPhotoPath
                                                   where pp.ProductId == r.ProductId
                                                   select pp.Path).FirstOrDefault()
                           }).ToList();
            return new
            {
                ProductListData = result1,
                TotalCount = totalCount
            };
        }

        public async Task<List<dynamic>> GetFavoriteListByUserId(int UserId,string Lang)
        {
            var result = await (from ri in db.ReferenceItem
                                join riSecond in db.ReferenceItem on ri.ParentId equals riSecond.Id
                                join riMain in db.ReferenceItem on riSecond.ParentId equals riMain.Id
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                                join product in db.Product on ri.Id equals product.ReferenceItemId
                                join favoriteList in db.ProductFavorite on product.Id equals favoriteList.ProductId
                                where rc.ShortLabel == "Product" && ri.Validity == true && rl.Lang == Lang
                                && riSecond.Validity == true && riMain.Validity == true && ri.Validity == true && favoriteList.UserId == UserId
                                orderby ri.CreatedOn descending, rc.Id, rl.Label
                                select new
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
                                    DefaultPhotoPath = (from path in db.ProductPhotoPath
                                                        where path.ProductId == product.Id
                                                        select path.Path).FirstOrDefault()
                                }).ToListAsync<dynamic>();
            return result;
        }

        public async Task<long> AddIntoProductFavoriteList(int UserId, long ProductId, bool? IsFavorite)
        {
            var result = db.ProductFavorite.Where(p => p.UserId == UserId && p.ProductId == ProductId).FirstOrDefault();
            if (result==null&& IsFavorite==true)
            {
                var FavoriteToInsert = new ProductFavorite();
                FavoriteToInsert.UserId = UserId;
                FavoriteToInsert.ProductId = ProductId;

                await db.AddAsync(FavoriteToInsert);
                await db.SaveChangesAsync();
                return FavoriteToInsert.Id;
            }
            else if (result!=null && IsFavorite== false)
            {
                db.Remove(result);
                await db.SaveChangesAsync();
                return result.Id;
            }
            return 0;
        }

        public async Task<List<ProductCategoryViewModel>> GetProductMainCategory(string Lang)
        {
            var result = await (from ri in db.ReferenceItem
                          join riSecond in db.ReferenceItem on ri.ParentId equals riSecond.Id
                          join rip in db.ReferenceItem on riSecond.ParentId equals rip.Id
                          join rlp in db.ReferenceLabel on rip.Id equals rlp.ReferenceItemId
                          join rcp in db.ReferenceCategory on rip.ReferenceCategoryId equals rcp.Id
                          where rcp.ShortLabel == "MainCategory" && rlp.Lang == Lang && rip.Validity == true && ri.Validity == true && riSecond.Validity == true
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
                                where rcp.ShortLabel == "SecondCategory" && rip.ParentId ==       MainCategoryReferenceId && rlp.Lang == Lang && rip.Validity == true && ri.Validity == true 
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

        public async Task<List<ProductCommentViewModel>> GetProductCommentListByCriteria(long? ProductId,long? UserId, string Lang)
        {
            var result = await (from pc in db.ProductComment
                                where (ProductId == null || pc.ProductId == ProductId) 
                                && (UserId == null || pc.UserId == UserId )
                                orderby pc.CreatedOn
                                select new ProductCommentViewModel() {
                                    CreatedOn = pc.CreatedOn,
                                    UserId = pc.UserId,
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
        
                                where (ProductLabel == "" || rl.Label.Contains(ProductLabel) || ri.Code.Contains(ProductLabel)) 
                                && (MainCategoryReferenceId==0 || riMain.Id == MainCategoryReferenceId)
                                && (SecondCategoryReferenceId.Count()== 0 || SecondCategoryReferenceId.Contains(riSecond.Id))
                                && (Validity == null ||ri.Validity == Validity)
                                && rl.Lang == Lang && rc.ShortLabel == "Product"
                                select new { 
                                    CreatedOn = p.CreatedOn,
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

        // For mobile attention : same format as by sales performance... (todo: fix format)
        public async Task<List<dynamic>> SimpleProductSearch(string SearchText, string Lang)
        {
            var result = await (from riProduct in db.ReferenceItem
                                join p in db.Product on riProduct.Id equals p.ReferenceItemId
                                join rlProduct in db.ReferenceLabel on riProduct.Id equals rlProduct.ReferenceItemId
                                join riSecond in db.ReferenceItem on riProduct.ParentId equals riSecond.Id
                                join rlSecond in db.ReferenceLabel on riProduct.Id equals rlSecond.ReferenceItemId
                                join riMain in db.ReferenceItem on riSecond.ParentId equals riMain.Id
                                join rlMain in db.ReferenceLabel on riMain.Id equals rlMain.ReferenceItemId
                                where riProduct.Validity == true && riSecond.Validity == true && riMain.Validity == true && rlProduct.Lang == Lang && rlSecond.Lang == Lang && rlMain.Lang == Lang && (rlMain.Label.Contains(SearchText) || rlSecond.Label.Contains(SearchText) || rlProduct.Label.Contains(SearchText) || p.Description.Contains(SearchText) || riProduct.Code.Contains(SearchText)) 
                                select new {
                                    ReferenceId = p.ReferenceItemId,
                                    ProductId = p.Id,
                                    Code = riProduct.Code,
                                    ParentId = riProduct.ParentId,
                                    Value = riProduct.Value,
                                    Label = rlProduct.Label,
                                    Price = p.Price,
                                    QuantityPerBox = p.QuantityPerBox,
                                    MinQuantity = p.MinQuantity,
                                    DefaultPhotoPath = (from pp in db.ProductPhotoPath
                                                        where pp.ProductId == p.Id
                                                        select pp.Path).FirstOrDefault()
                                }).ToListAsync<dynamic>();

            return result;
        }

        public async Task<List<dynamic>> AdvancedProductSearchClient(string SearchText,long? MainCategory, long? SecondCategory,int? PriceIntervalLower, int? PriceIntervalUpper, int? MinQuantity, string OrderBy_PublishDate, string OrderBy_SalesPerformance, string OrderBy_Price, string Lang)
        {
            var result = (from riProduct in db.ReferenceItem
                                join p in db.Product on riProduct.Id equals p.ReferenceItemId
                                join rlProduct in db.ReferenceLabel on riProduct.Id equals rlProduct.ReferenceItemId
                                join riSecond in db.ReferenceItem on riProduct.ParentId equals riSecond.Id
                                join riMain in db.ReferenceItem on riSecond.ParentId equals riMain.Id
                                where riProduct.Validity == true && riSecond.Validity == true && riMain.Validity == true && rlProduct.Lang == Lang 
                            && ( SearchText==null || SearchText=="" || rlProduct.Label.Contains(SearchText))
                            &&(MainCategory == null || riMain.ReferenceCategoryId == MainCategory)
                            &&(SecondCategory == null || riSecond.ReferenceCategoryId == SecondCategory)
                            &&(PriceIntervalLower==null || p.Price >= PriceIntervalLower)
                            &&(PriceIntervalUpper==null || p.Price<= PriceIntervalUpper)
                            &&(MinQuantity==null || p.MinQuantity<=MinQuantity)
                                select new
                                {
                                    ReferenceId = p.ReferenceItemId,
                                    ProductId = p.Id,
                                    Code = riProduct.Code,
                                    ParentId = riProduct.ParentId,
                                    Value = riProduct.Value,
                                    Label = rlProduct.Label,
                                    Price = p.Price,
                                    QuantityPerBox = p.QuantityPerBox,
                                    MinQuantity = p.MinQuantity,
                                    DefaultPhotoPath = (from pp in db.ProductPhotoPath
                                                        where pp.ProductId == p.Id
                                                        select pp.Path).FirstOrDefault()
                                });

            return await result.ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> GetProductByPrice(string Lang)
        {
            var result = (from riProduct in db.ReferenceItem
                          join p in db.Product on riProduct.Id equals p.ReferenceItemId
                          join rlProduct in db.ReferenceLabel on riProduct.Id equals rlProduct.ReferenceItemId
                          join riSecond in db.ReferenceItem on riProduct.ParentId equals riSecond.Id
                          join riMain in db.ReferenceItem on riSecond.ParentId equals riMain.Id
                          where riProduct.Validity == true && riSecond.Validity == true && riMain.Validity == true && rlProduct.Lang == Lang
                          orderby p.Price 
                          select new
                          {
                              ReferenceId = p.ReferenceItemId,
                              ProductId = p.Id,
                              Code = riProduct.Code,
                              ParentId = riProduct.ParentId,
                              Value = riProduct.Value,
                              Label = rlProduct.Label,
                              Price = p.Price,
                              QuantityPerBox = p.QuantityPerBox,
                              MinQuantity = p.MinQuantity,
                              DefaultPhotoPath = (from pp in db.ProductPhotoPath
                                                  where pp.ProductId == p.Id
                                                  select pp.Path).FirstOrDefault()
                          });

            return await result.ToListAsync<dynamic>();
        }


        public async Task<long> SaveProductInfo(long ProductId, long ReferenceId, int? QuantityPerBox, int? MinQuantity,float? Price, long? TaxRateId, string Description, string Color, string Material, string Size,int? CreatedOrUpdatedBy)
        {
            Product ProductToUpdateOrCreate = null;
            if (ProductId == 0)
            {
                ProductToUpdateOrCreate = new Product();
                ProductToUpdateOrCreate.ReferenceItemId = ReferenceId;
                ProductToUpdateOrCreate.CreatedBy = CreatedOrUpdatedBy;
                ProductToUpdateOrCreate.CreatedOn = DateTime.Now;
            }
            else
            {
                ProductToUpdateOrCreate = db.Product.Where(p => p.Id == ProductId).FirstOrDefault();
                ProductToUpdateOrCreate.UpdatedBy = CreatedOrUpdatedBy;
            }
            if (ProductToUpdateOrCreate!=null)
            {
                ProductToUpdateOrCreate.QuantityPerBox = QuantityPerBox;
                ProductToUpdateOrCreate.MinQuantity = MinQuantity;
                ProductToUpdateOrCreate.Price = Price;
                ProductToUpdateOrCreate.TaxRateId = TaxRateId;
                ProductToUpdateOrCreate.Description = Description;
                ProductToUpdateOrCreate.Color = Color;
                ProductToUpdateOrCreate.Material = Material;
                ProductToUpdateOrCreate.Size = Size;

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


        public async Task<dynamic> GetProductById(long Id,string Lang, int? UserId)
        {

            var result = await (from ri in db.ReferenceItem
                                join p in db.Product on ri.Id equals p.ReferenceItemId
                                where p.Id == Id
                                select new
                                {
                                    ProductId = p.Id,
                                    IsFavorite = db.ProductFavorite.Where(p => p.ProductId == Id).FirstOrDefault()!=null? true : false,
                                    HasBought = (from o in db.OrderInfo
                                                 join op in db.OrderProduct on o.Id equals op.OrderId 
                                                 join riStatus in db.ReferenceItem on o.StatusReferenceItemId equals riStatus.Id
                                                 where o.UserId == UserId && riStatus.Code == "OrderStatus_Valid" && op.ReferenceId == ri.Id
                                                 select o).FirstOrDefault()!=null? true: false,
                                    MainCategoryId = (from riMain in db.ReferenceItem
                                                      join riSecond in db.ReferenceItem on riMain.Id equals riSecond.ParentId
                                                      where riSecond.Id == ri.ParentId
                                                      select riMain.Id).FirstOrDefault(),
                                    SecondCategoryId = ri.ParentId,
                                    ReferenceCode = ri.Code,
                                    MinQuantity = p.MinQuantity,
                                    Price = p.Price,
                                    QuantityPerBox = p.QuantityPerBox,
                                    Description = p.Description,
                                    ReferenceId = ri.Id,
                                    TaxRateId = p.TaxRateId,
                                    Label = (from rl in db.ReferenceLabel
                                             where rl.ReferenceItemId == ri.Id && rl.Lang == Lang
                                             select rl.Label).FirstOrDefault(),
                                    TaxRate = (from riTaxRate in db.ReferenceItem
                                               where riTaxRate.Id == p.TaxRateId
                                               select riTaxRate).FirstOrDefault(),
                                    Translation = (from label in db.ReferenceLabel
                                                   where label.ReferenceItemId == ri.Id
                                                   select new {
                                                       Id = label.Id,
                                                       Label = label.Label,
                                                       Lang = label.Lang
                                                   }).ToList(),
                                    ImagesPath = (from path in db.ProductPhotoPath
                                                  where path.ProductId == p.Id
                                                  select new { path.Id, path.Path }).ToList(),
                                    Comments = (from c in db.ProductComment
                                                where c.ProductId == p.Id
                                                select c).ToList(),
                                    Color = p.Color,
                                    Material = p.Material,
                                    Size = p.Size,
                                    Validity = ri.Validity,
                                     DefaultPhotoPath = (from pp in db.ProductPhotoPath
                                                         where pp.ProductId == p.Id
                                                         select pp.Path).FirstOrDefault()
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


        public async Task<int> RemoveImageById(long Id)
        {
            ProductPhotoPath image = await db.ProductPhotoPath.FindAsync(Id);

            if (image == null)
            {
                return 0;
            }

            string imagePath = "images/" + image.Path; // todo place into the configuration

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
