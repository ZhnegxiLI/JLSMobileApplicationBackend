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
using LinqKit;

namespace JLSDataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly JlsDbContext db;
        private readonly IReferenceRepository _referencRepository;

        public ProductRepository(JlsDbContext context, IReferenceRepository Reference)
        {
            db = context;
            _referencRepository = Reference; // todo change
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
        {
            var ProductComment = new ProductComment();
            ProductComment.Title = Title;
            ProductComment.Body = Body;
            ProductComment.ProductId = ProductId;
            ProductComment.Level = Level;
            ProductComment.CreatedOn = DateTime.Now;
            ProductComment.CreatedBy = UserId;

            await db.ProductComment.AddAsync(ProductComment);
            await db.SaveChangesAsync();

            return ProductComment.Id;
        }

        public async Task<List<ProductComment>> GetProductCommentList()
        {
            var result = await (from pc in db.ProductComment
                          orderby pc.CreatedOn
                          select pc).ToListAsync();
            return result;
        }

        public async Task<List<ProductComment>> GetProductCommentListByProductId(long ProductId)
        {
            var result = await (from pc in db.ProductComment
                                where pc.ProductId == ProductId
                                orderby pc.CreatedOn
                                select pc).ToListAsync();
            return result;
        }

        public async Task<List<ProductComment>> GetProductCommentListByUserId(int UserId)
        {
            var result = await (from pc in db.ProductComment
                                where pc.UserId == UserId
                                orderby pc.CreatedOn
                                select pc).ToListAsync();
            return result;
        }





        /*
         * Admin Zoom
         */

        // TODO change: send from font-end OR from controller
        public async Task<List<ReferenceItemViewModel>> GetProductCategory(string lang)
        {
            var result = await _referencRepository.GetReferenceItemsByCategoryLabelsAdmin("MainCategory;SecondCategory", lang);
            return result;
        }

        // TODO change: send from font-end OR from controller
        public async Task<List<ReferenceItemViewModel>> GetTaxRate()
        {
            var result = await _referencRepository.GetReferenceItemsByCategoryLabelsAdmin("TaxRate", null);
            return result;
        }

        public async Task<int> SaveProduct(Product product, List<IFormFile> images, List<ReferenceLabel> labels)
        {
            string imagesPath = "images/" + product.ReferenceItem.Code + "/";

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            if (product.Id == 0)
            {
                db.ReferenceItem.Add(product.ReferenceItem);
                await db.SaveChangesAsync();
                product.ReferenceItemId = product.ReferenceItem.Id;
                db.Product.Add(product);
                await db.SaveChangesAsync();

                labels = _referencRepository.CheckLabels(labels, product.ReferenceItemId);
                foreach (ReferenceLabel label in labels)
                {
                    db.ReferenceLabel.Add(label);
                }
            }
            else
            {
                db.Product.Update(product);

                labels = _referencRepository.CheckLabels(labels, product.ReferenceItemId);
                foreach (ReferenceLabel label in labels)
                {
                    db.ReferenceLabel.Update(label);
                }

            }

            foreach (IFormFile image in images)
            {
                if (await SaveImage(image, imagesPath))
                {
                    db.ProductPhotoPath.Add(
                        new ProductPhotoPath
                        {
                            Path = product.ReferenceItem.Code + "/" + image.FileName,
                            ProductId = product.Id
                        }
                    );
                }
            }
            await db.SaveChangesAsync();
            return 1;
        }

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

        public async Task<List<ProductsListViewModel>> GetAllProduct(string lang, int intervalCount, int size, string orderActive, string orderDirection)
        {
            var request = (from ri in db.ReferenceItem
                           join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                           from rl in db.ReferenceLabel.Where(p => p.ReferenceItemId == ri.Id && p.Lang == lang).DefaultIfEmpty()
                           where rc.ShortLabel.Equals("Product")
                           join p in db.Product on ri.Id equals p.ReferenceItemId
                           from img in db.ProductPhotoPath.Where(img => p.Id == img.ProductId).Take(1).DefaultIfEmpty()
                           select new ProductsListViewModel
                           {
                               Id = p.Id,
                               Name = rl.Label,
                               Category = (from rlp in db.ReferenceLabel
                                           where rlp.ReferenceItemId == ri.ParentId
                                           select rlp.Label).FirstOrDefault(),
                               Image = img.Path,
                               Price = p.Price,
                               ReferenceCode = ri.Code,
                               Validity = ri.Validity,
                           });

            if (orderActive == "null" || orderActive == "undefined" || orderDirection == "null")
            {
                return await request.Skip(intervalCount * size).Take(size).ToListAsync();
            }

            Expression<Func<ProductsListViewModel, object>> funcOrder;// TODO :change

            switch (orderActive)
            {
                case "reference":
                    funcOrder = p => p.ReferenceCode;
                    break;
                case "name":
                    funcOrder = p => p.Name;
                    break;
                case "categories":
                    funcOrder = p => p.Category;
                    break;
                case "price":
                    funcOrder = p => p.Price;
                    break;
                case "active":
                    funcOrder = p => p.Validity;
                    break;
                default:
                    funcOrder = p => p.Id;
                    break;
            }

            if (orderDirection == "asc")
            {
                request = request.OrderBy(funcOrder);
            }
            else
            {
                request = request.OrderByDescending(funcOrder);
            }

            var result = await request.Skip(intervalCount * size).Take(size).ToListAsync();


            return result;
        }

        public async Task<ProductViewModel> GetProductById(long id)
        {
            var result = await (from ri in db.ReferenceItem
                                from rl in db.ReferenceLabel.Where(p => p.ReferenceItemId == ri.Id).DefaultIfEmpty()
                                join p in db.Product on ri.Id equals p.ReferenceItemId
                                where p.Id == id
                                select new ProductViewModel
                                {
                                    Id = p.Id,
                                    Category = ri.ParentId,
                                    ReferenceCode = ri.Code,
                                    Color = p.Color,
                                    Description = p.Description,
                                    Material = p.Material,
                                    Size = p.Size,
                                    MinQuantity = p.MinQuantity,
                                    Price = p.Price,
                                    QuantityPerBox = p.QuantityPerBox,
                                    ReferenceItemId = ri.Id,
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }
            result.Label = await (from rl in db.ReferenceLabel
                                  where rl.ReferenceItemId == result.ReferenceItemId
                                  select rl).ToListAsync();

            result.Images = await (from img in db.ProductPhotoPath
                                   where img.ProductId == result.Id
                                   select img).ToListAsync();
            return result;
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

        public async Task<List<ProductsListViewModel>> SearchProducts(string lang, string filter)
        {
            var predicate = PredicateBuilder.New<ProductsListViewModel>();
            predicate.Or(p => p.ReferenceCode.Contains(filter));
            predicate.Or(p => p.Name.Contains(filter));
            predicate.Or(p => p.Category.Contains(filter));

            var result = await (from ri in db.ReferenceItem
                                where ri.Code.Contains(filter)
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                from rl in db.ReferenceLabel.Where(p => p.ReferenceItemId == ri.Id && p.Lang == lang).DefaultIfEmpty()
                                where rc.ShortLabel.Equals("Product") // TODO change
                                join p in db.Product on ri.Id equals p.ReferenceItemId
                                from img in db.ProductPhotoPath.Where(img => p.Id == img.ProductId).Take(1).DefaultIfEmpty()
                                select new ProductsListViewModel
                                {
                                    Id = p.Id,
                                    Name = rl.Label,
                                    Category = (from rlp in db.ReferenceLabel
                                                where rlp.ReferenceItemId == ri.ParentId
                                                select rlp.Label).FirstOrDefault(),
                                    Image = img.Path,
                                    Price = p.Price,
                                    ReferenceCode = ri.Code,
                                    Validity = ri.Validity,
                                }).Where(predicate).Take(10).ToListAsync();
            return result;
        }

    }
}
