using JLSDataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.ViewModels;
using JLSDataModel.Models.Order;
using System.Linq.Expressions;
using JLSDataModel.Models.Adress;

namespace JLSDataAccess.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly JlsDbContext db;
        public OrderRepository(JlsDbContext context)
        {
            db = context;
        }

        /*
         * Mobile Zoom 
         */
        public async Task<long> SaveOrder(List<OrderProductViewModelMobile> References, Adress adress, long FacturationAdressId, int UserId)
        {
            /* Step1: get progressing status ri */
            var status = await (from ri in db.ReferenceItem
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          where rc.ShortLabel == "OrderStatus" && ri.Code == "OrderStatus_Progressing"
                          select ri).FirstOrDefaultAsync();

            /* Copy the shipping address from usershipping adress*/
            Adress adressToShipping = new Adress();
            adressToShipping.FirstLineAddress = adress.ContactTelephone;
            adressToShipping.ContactFax = adress.ContactFax;
            adressToShipping.ContactLastName = adress.ContactLastName;
            adressToShipping.ContactFirstName = adress.ContactFirstName;
            adressToShipping.ZipCode = adress.ZipCode;
            adressToShipping.FirstLineAddress = adress.FirstLineAddress;
            adressToShipping.SecondLineAddress = adress.SecondLineAddress;
            adressToShipping.City = adress.City;
            adressToShipping.Provence = adress.Provence;

            adressToShipping.Country = adress.Country;
            adressToShipping.EntrepriseName = adress.EntrepriseName;
            await db.AddAsync(adressToShipping);
            await db.SaveChangesAsync();

            // TODO :change
            /* Step2: construct the orderInfo object */
            var Order = new OrderInfo();
            Order.FacturationAdressId = FacturationAdressId;
            Order.ShippingAdressId = adressToShipping.Id;
            Order.UserId = UserId;
            //TODO: Order.TotalPrice
            Order.StatusReferenceItemId = status.Id;
            await db.AddAsync(Order);
            await db.SaveChangesAsync();

            var OrderProducts = new List<OrderProduct>();
            foreach(var r in References)
            {
                OrderProducts.Add(new OrderProduct()
                {
                    OrderId = Order.Id,
                    Quantity = r.Quantity,
                    ReferenceId = r.ReferenceId
                });
            }
            await db.AddRangeAsync(OrderProducts);
            await db.SaveChangesAsync();

            // Return new orderId
            return Order.Id;
        }

        public async Task<List<OrderListViewModelMobile>> GetOrdersListByUserId(int UserId,string Lang)
        {
            var result = await (from o in db.OrderInfo
                          where o.UserId == UserId
                          orderby o.CreatedOn descending
                          select new OrderListViewModelMobile()
                          {
                              Id = o.Id,
                              CreatedOn = o.CreatedOn,
                              TotalPrice = o.TotalPrice,
                              ShippingAdressId = o.ShippingAdressId,
                              ShippingAdress = (from a in db.Adress
                                                where a.Id == o.ShippingAdressId
                                                select a).FirstOrDefault(),
                              StatusCode = (from ri in db.ReferenceItem
                                        where ri.Id == o.StatusReferenceItemId
                                        select ri.Code).FirstOrDefault(),
                              StatusLabel = (from rl in db.ReferenceLabel
                                            where rl.ReferenceItemId == o.StatusReferenceItemId && rl.Lang == Lang
                                             select rl.Label).FirstOrDefault(),
                          }).ToListAsync();
            return result;
        }


        public async Task<OrderDetailViewModelMobile> GetOrdersListByOrderId(long OrderId, string Lang)
        {
            var result = await (from o in db.OrderInfo
                                where o.Id == OrderId
                                select new OrderDetailViewModelMobile()
                                {
                                   OrderInfo = o,
                                   Status = (from ri in db.ReferenceItem
                                             join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                                             where rl.Lang == Lang && ri.Id == o.StatusReferenceItemId
                                             select new ReferenceItemViewModel()
                                             {
                                                 Id = ri.Id,
                                                 Label = rl.Label,
                                                 Code = ri.Code
                                             }).FirstOrDefault(),
                                   FacturationAdress = db.Adress.Where(p=>p.Id == o.FacturationAdressId).FirstOrDefault(),
                                   ShippingAdress = db.Adress.Where(p=>p.Id == o.ShippingAdressId).FirstOrDefault(),
                                   ProductList = (from op in db.OrderProduct
                                                  join p in db.Product on op.ReferenceId equals p.ReferenceItemId
                                                  where op.OrderId == o.Id
                                                  select new ProductDetailViewModelMobile()
                                                  {
                                                      Id = p.Id,
                                                      Price = p.Price,
                                                      QuantityPerBox = p.QuantityPerBox,
                                                      MinQuantity = p.MinQuantity,
                                                      Quantity = op.Quantity,
                                                      PhotoPaths = db.ProductPhotoPath.Where(x=>x.ProductId == p.Id).ToList(),
                                                      Reference = (from ri in db.ReferenceItem
                                                                   join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                                                                   where ri.Id == p.ReferenceItemId && rl.Lang== Lang
                                                                   select new ReferenceItemViewModel() { 
                                                                   Id = ri.Id,
                                                                   Code = ri.Code,
                                                                   ParentId = ri.ParentId,
                                                                   ReferenceCategoryId = ri.ReferenceCategoryId,
                                                                   Label = rl.Label,
                                                                   Validity = ri.Validity
                                                                   }).FirstOrDefault()

                                                  }).ToList()
                                }).FirstOrDefaultAsync();
            return result;
        }


        /*
         * Admin Zoom 
         */

        public async Task<List<dynamic>> AdvancedOrderSearchByCriteria(string Lang, int? UserId, DateTime? FromDate, DateTime? ToDate, long? OrderId, long? StatusId)
        {
            var result = await (from order in db.OrderInfo
                          from statusRi in db.ReferenceItem.Where(p => p.Id == order.StatusReferenceItemId).DefaultIfEmpty()
                          where (StatusId == null || statusRi.Id == StatusId)
                          && (UserId == null || order.UserId == UserId)
                          && (OrderId == null || order.Id == OrderId)
                          && (FromDate == null || order.CreatedOn >= FromDate)
                          && (ToDate == null || order.CreatedOn <= ToDate)
                          orderby order.CreatedOn descending
                          select new
                          {
                              Id = order.Id,
                              User = (from u in db.Users
                                      where u.Id == order.UserId
                                      select u).FirstOrDefault(),
                              UpdatedBy = order.UpdatedBy,
                              UpdatedByUser  = (from u in db.Users
                                                where u.Id == order.UpdatedBy
                                                select u).FirstOrDefault(),

                              StatusId = statusRi.Id,
                              Status = ( from statusLabel in db.ReferenceLabel
                                         where statusLabel.ReferenceItemId == statusRi.Id && statusLabel.Lang == Lang
                                         select new
                                         {
                                             Id = statusRi.Id,
                                             Label = statusLabel.Label,
                                             Code = statusRi.Code
                                         }).FirstOrDefault(),
                            CreatedOn = order.CreatedOn,
                            UpdatedOn = order.UpdatedOn,
                            TotalPrice = order.TotalPrice,
                            UpdatedUser = (from uu in db.Users
                                           where uu.Id == order.UpdatedBy
                                           select uu).FirstOrDefault()
                          }).ToListAsync<dynamic>();
            return result;
        }

        public async Task<List<OrdersListViewModel>> GetAllOrdersWithInterval(string lang, int intervalCount, int size, string orderActive, string orderDirection)
        {
            var request = (from order in db.OrderInfo
                           join user in db.Users on order.UserId equals user.Id
                           select new OrdersListViewModel
                           {
                               Id = order.Id,
                               OrderReferenceCode = order.OrderReferenceCode,
                               EntrepriseName = user.EntrepriseName,
                               UserName = user.UserName,
                               TotalPrice = order.TotalPrice,
                               Date = order.CreatedOn,
                               StatusReferenceItemLabel = (from ri in db.ReferenceItem
                                                           where ri.Id == order.StatusReferenceItemId
                                                           from rl in db.ReferenceLabel
                                                           .Where(rl => rl.ReferenceItemId == ri.Id && rl.Lang == lang).DefaultIfEmpty()
                                                           select rl.Label).FirstOrDefault()
                           });

            if (orderActive == "null" || orderActive == "undefined" || orderDirection == "null")
            {
                return await request.Skip(intervalCount * size).Take(size).ToListAsync();
            }

            Expression<Func<OrdersListViewModel, object>> funcOrder;// TOdo: check !!

            switch (orderActive)
            {
                case "id":
                    funcOrder = order => order.Id;
                    break;
                case "reference":
                    funcOrder = order => order.OrderReferenceCode;
                    break;
                case "name":
                    funcOrder = order => order.UserName;
                    break;
                case "entrepriseName":
                    funcOrder = order => order.EntrepriseName;
                    break;
                case "total":
                    funcOrder = order => order.TotalPrice;
                    break;
                case "status":
                    funcOrder = order => order.StatusReferenceItemLabel;
                    break;
                case "date":
                    funcOrder = order => order.Date;
                    break;
                default:
                    funcOrder = order => order.Id;
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

        public async Task<OrderViewModel> GetOrderById(long id, string lang)
        {
            var result = await (from order in db.OrderInfo
                                where order.Id == id
                                join sa in db.Adress on order.ShippingAdressId equals sa.Id
                                join fa in db.Adress on order.FacturationAdressId equals fa.Id
                                join user in db.Users on order.UserId equals user.Id
                                join ris in db.ReferenceItem on order.StatusReferenceItemId equals ris.Id
                                from rls in db.ReferenceLabel.Where(rls => rls.ReferenceItemId == ris.Id
                                && rls.Lang == lang).DefaultIfEmpty()
                                select new OrderViewModel
                                {
                                    OrderReferenceCode = order.OrderReferenceCode,
                                    PaymentInfo = order.PaymentInfo,
                                    TaxRate = order.TaxRate,
                                    TotalPrice = order.TotalPrice,
                                    AdminRemark = order.AdminRemark,
                                    ClientRemark = order.ClientRemark,
                                    StatusLabel = rls.Label,
                                    StatusReferenceItem = ris,
                                    User = new UserViewModel
                                    {
                                        Id = user.Id,
                                        Email = user.Email,
                                        EntrepriseName = user.EntrepriseName,
                                        Name = user.UserName,
                                        Telephone = user.PhoneNumber
                                    },
                                    FacturationAdress = fa,
                                    ShippingAdress = sa,
                                    Products = (from po in db.OrderProduct
                                                where po.OrderId == order.Id
                                                join rip in db.ReferenceItem on po.ReferenceId equals rip.Id
                                                join pi in db.Product on rip.Id equals pi.ReferenceItemId
                                                from img in db.ProductPhotoPath.Where(img => img.ProductId == pi.Id)
                                                .Take(1).DefaultIfEmpty()
                                                from rlp in db.ReferenceLabel.Where(rlp => rlp.ReferenceItemId == rip.Id
                                                && rlp.Lang == lang).Take(1).DefaultIfEmpty()
                                                select new OrderProductViewModel
                                                {
                                                    Id = pi.Id,
                                                    Image = img.Path,
                                                    Name = rlp.Label,
                                                    Price = pi.Price,
                                                    Quantity = po.Quantity,
                                                    ReferenceCode = rip.Code
                                                }).ToList()
                                }).FirstOrDefaultAsync();

            return result;
        }
    }
}
