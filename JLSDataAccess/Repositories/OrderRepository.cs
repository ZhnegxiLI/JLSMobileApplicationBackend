using JLSDataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.ViewModels;
using JLSDataModel.Models.Order;

namespace JLSDataAccess.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly JlsDbContext db;
        public OrderRepository(JlsDbContext context)
        {
            db = context;
        }

        /* Only for mobile usage */
        public async Task<long> SaveOrder(List<OrderProductViewModel> References, long ShippingAdressId, long FacturationAdressId, int UserId)
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
    }
}
