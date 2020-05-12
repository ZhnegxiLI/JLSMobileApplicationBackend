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
using JLSDataModel.Models;

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
        public async Task<long> SaveOrder(List<OrderProductViewModelMobile> References, long ShippingAdressId, long FacturationAdressId, int UserId, long? ClientRemarkId, long CutomerInfoId)
        {
            /* Step1: get progressing status ri */
            var status = await (from ri in db.ReferenceItem
                          join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                          where rc.ShortLabel == "OrderStatus" && ri.Code == "OrderStatus_Progressing"
                          select ri).FirstOrDefaultAsync();

            var orderType = await (from ri in db.ReferenceItem
                                join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                where rc.ShortLabel == "OrderType" && ri.Code == "OrderType_External"
                                   select ri).FirstOrDefaultAsync();


            var TaxRate = await (from ri in db.ReferenceItem
                                   join rc in db.ReferenceCategory on ri.ReferenceCategoryId equals rc.Id
                                   where rc.ShortLabel == "TaxRate" && ri.Validity == true
                                   orderby ri.Order descending
                                   select ri).FirstOrDefaultAsync();

            /* Step2: construct the orderInfo object */
            var Order = new OrderInfo();
            Order.FacturationAdressId = FacturationAdressId;
            Order.ShippingAdressId = ShippingAdressId;
            Order.UserId = UserId;
            Order.StatusReferenceItemId = status.Id;
            Order.OrderTypeId = orderType.Id;
            Order.TaxRateId = TaxRate.Id;

            Order.CreatedBy = UserId;
            Order.CreatedOn = DateTime.Now;

            Order.ClientRemarkId = ClientRemarkId;
            Order.CustomerId = CutomerInfoId;

            await db.AddAsync(Order);
            await db.SaveChangesAsync();

            /* Step3: add OrderInfoStatusLog*/

            var orderInfoStatusLog = new OrderInfoStatusLog();

            orderInfoStatusLog.OrderInfoId = Order.Id;

            orderInfoStatusLog.NewStatusId = Order.StatusReferenceItemId;
            orderInfoStatusLog.UserId = UserId;
            orderInfoStatusLog.ActionTime = DateTime.Now;
            db.OrderInfoStatusLog.Add(orderInfoStatusLog);

            await db.SaveChangesAsync();

            /* Step4: Add product */
            float TotalPrice = 0;
            var OrderProducts = new List<OrderProduct>();
            foreach(var r in References)
            {
                OrderProducts.Add(new OrderProduct()
                {
                    OrderId = Order.Id,
                    Quantity = r.Quantity,
                    ReferenceId = r.ReferenceId,
                    UnitPrice = r.Price
                });
                TotalPrice = (r.Quantity * r.Price.Value) + TotalPrice;
            }
            await db.AddRangeAsync(OrderProducts);
            await db.SaveChangesAsync();

            Order.TotalPrice = TotalPrice;

            db.Update(Order);
            await db.SaveChangesAsync();
            // Return new orderId
            return Order.Id;
        }


        public async Task<long> SaveAdminOrder(OrderInfo order,List<OrderProductViewModelMobile> References,  int CreatedOrUpdatedBy)
        {
            try
            {
                OrderInfo orderToUpdate = null;
                if (order.Id == 0)
                {
                    orderToUpdate = new OrderInfo();

                    orderToUpdate.CreatedBy = CreatedOrUpdatedBy;
                    orderToUpdate.CreatedOn = DateTime.Now;

                    orderToUpdate.UserId = CreatedOrUpdatedBy;
                }
                else
                {
                    orderToUpdate = await db.OrderInfo.AsNoTracking().Where(x => x.Id == order.Id).FirstOrDefaultAsync();
                    var oldOrder = await db.OrderInfo.FindAsync(order.Id);
                    orderToUpdate.UpdatedBy = CreatedOrUpdatedBy;

                    if (oldOrder.StatusReferenceItemId != order.StatusReferenceItemId)
                    {
                        var orderInfoStatusLog = new OrderInfoStatusLog();

                        orderInfoStatusLog.OrderInfoId = order.Id;
                        orderInfoStatusLog.OldStatusId = oldOrder.StatusReferenceItemId;
                        orderInfoStatusLog.NewStatusId = order.StatusReferenceItemId;
                        orderInfoStatusLog.UserId = CreatedOrUpdatedBy;
                        orderInfoStatusLog.ActionTime = DateTime.Now;

                        db.OrderInfoStatusLog.Add(orderInfoStatusLog);
                        await db.SaveChangesAsync();
                    }
                }

                orderToUpdate.AdminRemarkId = order.AdminRemarkId;
                orderToUpdate.ClientRemarkId = order.ClientRemarkId;
                orderToUpdate.FacturationAdressId = order.FacturationAdressId;
                orderToUpdate.ShipmentInfoId = order.ShipmentInfoId;
                orderToUpdate.StatusReferenceItemId = order.StatusReferenceItemId;
                orderToUpdate.TaxRateId = order.TaxRateId;
                orderToUpdate.CustomerId = order.CustomerId;
                orderToUpdate.UserId = order.UserId;

                if (order.Id > 0)
                {
                    db.Update(order);

                    await db.SaveChangesAsync();
                }
                else
                {
                    await db.OrderInfo.AddAsync(order);
                    await db.SaveChangesAsync();

                    var orderInfoStatusLog = new OrderInfoStatusLog();

                    orderInfoStatusLog.OrderInfoId = order.Id;

                    orderInfoStatusLog.NewStatusId = order.StatusReferenceItemId;
                    orderInfoStatusLog.UserId = CreatedOrUpdatedBy;
                    orderInfoStatusLog.ActionTime = DateTime.Now;

                    db.OrderInfoStatusLog.Add(orderInfoStatusLog);

                    await db.SaveChangesAsync();
                 
                }
   

                /* Step 1: remove all the product in order */
                var PreviousOrderProducts = await db.OrderProduct.Where(p => p.OrderId == order.Id).ToListAsync();
                db.RemoveRange(PreviousOrderProducts);

                float TotalPrice = 0;
                List<OrderProduct> products = new List<OrderProduct>();
                /* Step 2: Add product in order */
                if (References.Count() > 0)
                {
                    foreach (var item in References)
                    {
                        var reference = new OrderProduct();
                        reference.ReferenceId = item.ReferenceId;
                        reference.Quantity = item.Quantity;
                        reference.UnitPrice = item.Price;
                        reference.OrderId = order.Id;
                      
                        TotalPrice = TotalPrice + (item.Price.Value * item.Quantity);
                        
                        products.Add(reference);
                    }
                }


                await db.AddRangeAsync(products);

                order.TotalPrice = TotalPrice;

                db.Update(order);

                await db.SaveChangesAsync();

                // Return new orderId
                return order.Id;
            }
            catch (Exception e)
            {
                throw e;
            }

         
        }

        public async Task<List<OrderListViewModelMobile>> GetOrdersListByUserId(int UserId, string StatusCode, string Lang)
        {
            var result = await (from o in db.OrderInfo
                                join riStatus in db.ReferenceItem on o.StatusReferenceItemId equals riStatus.Id
                                where o.UserId == UserId && (StatusCode == "All" || riStatus.Code == StatusCode)
                                orderby o.CreatedOn descending
                                select new OrderListViewModelMobile()
                                {
                                    Id = o.Id,
                                    CreatedOn = o.CreatedOn,
                                    TotalPrice = o.TotalPrice,
                                    NumberOfProduct = db.OrderProduct.Where(p=>p.OrderId == o.Id).Sum(p=>p.Quantity),
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


        public async Task<dynamic> GetOrdersListByOrderId(long OrderId, string Lang)
        {
            var result = await (from o in db.OrderInfo
                                where o.Id == OrderId
                                select new 
                                {
                                   OrderInfo = new OrderInfo { 
                                        Id = o.Id,
                                        UserId = o.UserId,
                                        User = (from u in db.Users
                                                where u.Id == o.UserId
                                                select u).FirstOrDefault(),
                                        TaxRateId = o.TaxRateId,
                                        TotalPrice = o.TotalPrice,
                                        ClientRemarkId = o.ClientRemarkId,
                                        AdminRemarkId = o.AdminRemarkId,
                                        PaymentInfo = o.PaymentInfo,
                                        CreatedOn = o.CreatedOn,
                                        UpdatedOn = o.UpdatedOn,
                                        OrderTypeId = o.OrderTypeId,
                                       ShipmentInfoId = o.ShipmentInfoId
                                   },
                                   ShippingMessage = (from riShippingMessage in db.ReferenceItem
                                                      join rlShippingMessage in db.ReferenceLabel on riShippingMessage.Id equals rlShippingMessage.ReferenceItemId
                                                      where rlShippingMessage.Lang == Lang && riShippingMessage.Code == "ShippingMessage"
                                                      select rlShippingMessage.Label).FirstOrDefault(),
                                   ClientRemark = ( from clientRemark in db.Remark
                                                    where clientRemark.Id == o.ClientRemarkId
                                                    select clientRemark).FirstOrDefault(),
                                   AdminRemark = (from adminRemark in db.Remark
                                                   where adminRemark.Id == o.AdminRemarkId
                                                   select adminRemark).FirstOrDefault(),
                                   ShipmentInfo = (from shipmentInfo in db.ShipmentInfo
                                                   where shipmentInfo.Id == o.ShipmentInfoId
                                                   select shipmentInfo).FirstOrDefault(),
                                   TaxRate = (from taxRateRi in db.ReferenceItem
                                              where taxRateRi.Id == o.TaxRateId 
                                              select new {
                                                Id = taxRateRi.Id,
                                                Code = taxRateRi.Code,
                                                Value = taxRateRi.Value
                                              }).FirstOrDefault(),

                                   CustomerInfo = (from customer in db.CustomerInfo
                                                   where customer.Id == o.CustomerId
                                                   select customer).FirstOrDefault(),
                                   OrderType = (from riOrderType in db.ReferenceItem
                                                join rlOrderType in db.ReferenceLabel on riOrderType.Id equals rlOrderType.ReferenceItemId
                                                where rlOrderType.Lang == Lang && riOrderType.Id == o.OrderTypeId
                                                select new { 
                                                    Id = riOrderType.Id,
                                                    Code = riOrderType.Code,
                                                    Label = rlOrderType.Label
                                                }).FirstOrDefault(),
                                   Status = (from ri in db.ReferenceItem
                                             join rl in db.ReferenceLabel on ri.Id equals rl.ReferenceItemId
                                             where rl.Lang == Lang && ri.Id == o.StatusReferenceItemId
                                             select new ReferenceItemViewModel()
                                             {
                                                 Id = ri.Id,
                                                 Label = rl.Label,
                                                 Code = ri.Code
                                             }).FirstOrDefault(),
                                   StatusInfo = (from statusInfo in db.OrderInfoStatusLog
                                                 where statusInfo.OrderInfoId == o.Id
                                                 orderby statusInfo.ActionTime descending
                                                 select new { 
                                                     Id = statusInfo.Id,
                                                     OldStatus = (from rlOld in db.ReferenceLabel
                                                                  join riOld in db.ReferenceItem on rlOld.ReferenceItemId equals riOld.Id
                                                                  where rlOld.ReferenceItemId == statusInfo.OldStatusId && rlOld.Lang == Lang
                                                                  select new { 
                                                                        ReferenceId = rlOld.ReferenceItemId,
                                                                        Code = riOld.Code,
                                                                        Label = rlOld.Label
                                                                  }).FirstOrDefault(),
                                                    NewStatus = (from rlNew in db.ReferenceLabel
                                                                 join riNew in db.ReferenceItem on rlNew.ReferenceItemId equals riNew.Id
                                                                 where rlNew.ReferenceItemId == statusInfo.NewStatusId && rlNew.Lang == Lang
                                                                 select new
                                                                 {
                                                                     ReferenceId = rlNew.ReferenceItemId,
                                                                     Code = riNew.Code,
                                                                     Label = rlNew.Label
                                                                 }).FirstOrDefault(),
                                                    ActionTime = statusInfo.ActionTime,

                                                    UserId = statusInfo.UserId,
                                                    UserName = (from u in db.Users
                                                            where u.Id == statusInfo.UserId
                                                            select u.UserName).FirstOrDefault()
                                                 }).ToList(),
                                   FacturationAdress = db.Adress.Where(p=>p.Id == o.FacturationAdressId).FirstOrDefault(),
                                   ShippingAdress = db.Adress.Where(p=>p.Id == o.ShippingAdressId).FirstOrDefault(),
                                   ProductList = (from op in db.OrderProduct
                                                  join p in db.Product on op.ReferenceId equals p.ReferenceItemId
                                                  join riProduct in db.ReferenceItem on p.ReferenceItemId equals riProduct.Id
                                                  join rc in db.ReferenceCategory on riProduct.ReferenceCategoryId equals rc.Id
                                                  join rl in db.ReferenceLabel on riProduct.Id equals rl.ReferenceItemId
                                                  where op.OrderId == o.Id && rc.ShortLabel == "Product" &&  rl.Lang == Lang 
                                                  select new 
                                                  {
                                                      Quantity = op.Quantity,
                                                      ProductId = p.Id,
                                                      ReferenceId = riProduct.Id,
                                                      Code = riProduct.Code,
                                                      ParentId = riProduct.ParentId,
                                                      Value = riProduct.Value,
                                                      Order = riProduct.Order,
                                                      Label = rl.Label,
                                                      Price = op.UnitPrice,
                                                      QuantityPerBox = p.QuantityPerBox,
                                                      MinQuantity = p.MinQuantity,
                                                      Size = p.Size,
                                                      Color = p.Color,
                                                      Material = p.Material,
                                                      DefaultPhotoPath = (from path in db.ProductPhotoPath
                                                                          where path.ProductId == p.Id
                                                                          select path.Path).FirstOrDefault(),
                                                      PhotoPath = (from path in db.ProductPhotoPath
                                                                   where path.ProductId == p.Id
                                                                   select new ProductListPhotoPath() { Path = path.Path }).ToList()
                                                  }).ToList()
                                }).FirstOrDefaultAsync();
            return result;
        }


        /*
         * Admin Zoom 
         */

        public async Task<List<dynamic>> AdvancedOrderSearchByCriteria(string Lang, int? UserId, DateTime? FromDate, DateTime? ToDate, string OrderId, long? StatusId)
        {
            var result = await (from order in db.OrderInfo
                          from statusRi in db.ReferenceItem.Where(p => p.Id == order.StatusReferenceItemId).DefaultIfEmpty()
                            where (StatusId == null || statusRi.Id == StatusId)
                          && (UserId == null || order.UserId == UserId)
                          && (OrderId == null || order.Id.ToString().Contains(OrderId))
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
                            OrderType = (from orderTypeRi in db.ReferenceItem
                                         join orderTypeRl in db.ReferenceLabel on orderTypeRi.Id equals orderTypeRl.ReferenceItemId
                                         where orderTypeRi.Id == order.OrderTypeId && orderTypeRl.Lang == Lang
                                         select new {
                                            Id = orderTypeRi.Id,
                                            Code = orderTypeRi.Code,
                                            Label = orderTypeRl.Label
                                         }).FirstOrDefault(),

                            UpdatedUser = (from uu in db.Users
                                           where uu.Id == order.UpdatedBy
                                           select uu).FirstOrDefault()
                          }).ToListAsync<dynamic>();
            return result;
        }

        public async Task<long> SaveOrderRemark(Remark remark, int? CreatedOrUpadatedBy)
        {
            if (remark.Id != 0)
            {
                remark.UpdatedBy = CreatedOrUpadatedBy;
                db.Remark.Update(remark);
            }
            else
            {
                remark.CreatedBy = CreatedOrUpadatedBy;
                remark.CreatedOn = DateTime.Now;
                await db.Remark.AddAsync(remark);
            }
            await db.SaveChangesAsync();
            return remark.Id;
        }

        public async Task<long> SaveCustomerInfo(CustomerInfo customer, int? CreatedOrUpadatedBy)
        {
            if (customer.Id != 0)
            {
                customer.UpdatedBy = CreatedOrUpadatedBy;
                db.CustomerInfo.Update(customer);
            }
            else
            {
                customer.CreatedBy = CreatedOrUpadatedBy;
                customer.CreatedOn = DateTime.Now;
                await db.CustomerInfo.AddAsync(customer);
            }
            await db.SaveChangesAsync();
            return customer.Id;
        }

        public async Task<long> SaveOrderShipmentInfo(ShipmentInfo shipment, int? CreatedOrUpadatedBy)
        {
            if (shipment.Id != 0)
            {
                shipment.UpdatedBy = CreatedOrUpadatedBy;

                db.ShipmentInfo.Update(shipment);
            }
            else
            {
                shipment.CreatedBy = CreatedOrUpadatedBy;
                shipment.CreatedOn = DateTime.Now;

                await db.ShipmentInfo.AddAsync(shipment);
            }

            await db.SaveChangesAsync();

            return shipment.Id;
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
                                   // TaxRateId = order.TaxRateId,
                                    TotalPrice = order.TotalPrice,
                                   // AdminRemarkId = order.AdminRemarkId,
                                  //  ClientRemark = order.ClientRemarkId,
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
