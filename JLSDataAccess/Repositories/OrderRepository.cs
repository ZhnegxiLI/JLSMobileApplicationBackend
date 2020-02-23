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
        public async Task<long> SaveOrder(List<OrderProductViewModelMobile> References, long ShippingAdressId, long FacturationAdressId, int UserId)
        {
            /* Step1: get progressing status ri */
            var status = await (from ri in db.ReferenceItem
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          where rc.ShortLabel == "OrderStatus" && ri.Code == "OrderStatus_Progressing"
                          select ri).FirstOrDefaultAsync();

            /* Step2: construct the orderInfo object */
            var Order = new OrderInfo();
            Order.FacturationAdressId = FacturationAdressId;
            Order.ShippingAdressId = ShippingAdressId;
            Order.UserId = UserId;
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


        /*
         * Admin Zoom 
         */

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
                                && rls.Lang == lang).Take(1).DefaultIfEmpty()
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
