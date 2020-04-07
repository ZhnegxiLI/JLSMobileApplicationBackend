using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using JLSDataModel.Models.Adress;
using JLSDataModel.Models.Order;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JLSMobileApplication.Controllers.AdminService
{
    [Authorize]
    [Route("admin/[controller]/{action}")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IAdressRepository _adressRepository;
        private readonly JlsDbContext db;

        public OrderController(IOrderRepository orderRepository, IMapper mapper, IAdressRepository adressRepository, JlsDbContext context)
        {
            this._orderRepository = orderRepository;
            _mapper = mapper;
            _adressRepository = adressRepository;
            db = context;
        }

        public class AdvancedOrderSearchCriteria
        {
            public string Lang { get; set; }
            public int? UserId { get; set; }

            public DateTime? FromDate { get; set; }

            public DateTime? ToDate { get; set; }

            public long? StatusId { get; set; }
            
            public string OrderId { get; set; }

            public int begin { get; set; }

            public int step { get; set; }
        }

        [HttpPost]
        public async Task<JsonResult> AdvancedOrderSearchByCriteria(AdvancedOrderSearchCriteria criteria)
        {
            try
            {
                var result = await _orderRepository.AdvancedOrderSearchByCriteria(criteria.Lang,criteria.UserId,criteria.FromDate,criteria.ToDate,criteria.OrderId,criteria.StatusId);
                var list = result.Skip(criteria.begin * criteria.step).Take(criteria.step);

                return Json(new
                {
                    OrderList = list,
                    TotalCount = result.Count()
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public class SaveAdminOrderCriteria
        {
            public CustomerInfo CustomerInfo { get; set; }
            public Remark AdminRemark { get; set; }

            public Remark ClientRemark { get; set; }

            public ShipmentInfo ShipmentInfo { get; set; }
            public Adress ShippingAddress { get; set; }

            public Adress FacturationAddress { get; set; }

            public List<OrderProductViewModelMobile> References { get; set; }

            public OrderInfo Orderinfo { get; set; }

            public int CreatedOrUpdatedBy { get; set; }
        }

        [HttpPost]
        public async Task<JsonResult> SaveAdminOrder(SaveAdminOrderCriteria criteria)
        {
            try
            {

                /* Step1 : save shipping address */
                if (criteria.ShippingAddress!=null && criteria.ShippingAddress.Id==0)
                {
                    criteria.ShippingAddress.CreatedBy = criteria.CreatedOrUpdatedBy;
                    criteria.ShippingAddress.CreatedOn = DateTime.Now;
                }
                else
                {
                    criteria.ShippingAddress.UpdatedBy = criteria.CreatedOrUpdatedBy;
                }
                var ShippingAddressId = await _adressRepository.CreateOrUpdateAdress(criteria.ShippingAddress);

                /* Step2 : save facturation address */
                if (criteria.FacturationAddress != null && criteria.FacturationAddress.Id == 0)
                {
                    criteria.FacturationAddress.CreatedBy = criteria.CreatedOrUpdatedBy;
                    criteria.FacturationAddress.CreatedOn = DateTime.Now;
                }
                else
                {
                    criteria.FacturationAddress.UpdatedBy = criteria.CreatedOrUpdatedBy;
                }
                var FacturationAddressId = await _adressRepository.CreateOrUpdateAdress(criteria.FacturationAddress);

                /* Step3: save shipment info */
                if (criteria.ShipmentInfo!=null )
                {
                    var ShipmentInfoId = await _orderRepository.SaveOrderShipmentInfo(criteria.ShipmentInfo, criteria.CreatedOrUpdatedBy);
                    criteria.Orderinfo.ShipmentInfoId = ShipmentInfoId;
                }

                /* Step4: save Admin remark info */
                if (criteria.AdminRemark != null)
                {
                    var AdminRemarkId = await _orderRepository.SaveOrderRemark(criteria.AdminRemark, criteria.CreatedOrUpdatedBy);
                    criteria.Orderinfo.AdminRemarkId = AdminRemarkId;
                }
                if (criteria.CustomerInfo !=null)
                {
                    var CustomerId = await _orderRepository.SaveCustomerInfo(criteria.CustomerInfo, criteria.CreatedOrUpdatedBy);
                    criteria.Orderinfo.CustomerId = CustomerId;
                }

                /* Step5: save Admin remark info */
                if (criteria.ClientRemark != null)
                {
                    var ClientRemarkId = await _orderRepository.SaveOrderRemark(criteria.ClientRemark, criteria.CreatedOrUpdatedBy);
                    criteria.Orderinfo.ClientRemarkId = ClientRemarkId;
                }

                /* Step 6: save order info */
                criteria.Orderinfo.ShippingAdressId = ShippingAddressId;
                criteria.Orderinfo.FacturationAdressId = FacturationAddressId;

                OrderInfo orderToUpdate = null;
                if (criteria.Orderinfo.Id == 0)
                {
                    orderToUpdate = new OrderInfo();

                    orderToUpdate.CreatedBy = criteria.CreatedOrUpdatedBy;
                    orderToUpdate.CreatedOn = DateTime.Now;

                    orderToUpdate.UserId = criteria.CreatedOrUpdatedBy;
                }
                else
                {
                    orderToUpdate = db.OrderInfo.Find(criteria.Orderinfo.Id);
                    var oldOrder = await db.OrderInfo.FindAsync(criteria.Orderinfo.Id);
                    orderToUpdate.UpdatedBy = criteria.CreatedOrUpdatedBy;

                    if (oldOrder.StatusReferenceItemId != criteria.Orderinfo.StatusReferenceItemId)
                    {
                        var orderInfoStatusLog = new OrderInfoStatusLog();

                        orderInfoStatusLog.OrderInfoId = criteria.Orderinfo.Id;
                        orderInfoStatusLog.OldStatusId = oldOrder.StatusReferenceItemId;
                        orderInfoStatusLog.NewStatusId = criteria.Orderinfo.StatusReferenceItemId;
                        orderInfoStatusLog.UserId = criteria.CreatedOrUpdatedBy;
                        orderInfoStatusLog.ActionTime = DateTime.Now;

                        db.OrderInfoStatusLog.Add(orderInfoStatusLog);
                        await db.SaveChangesAsync();
                    }
                }

                orderToUpdate.AdminRemarkId = criteria.Orderinfo.AdminRemarkId;
                orderToUpdate.ClientRemarkId = criteria.Orderinfo.ClientRemarkId;
                orderToUpdate.FacturationAdressId = criteria.Orderinfo.FacturationAdressId;
                orderToUpdate.ShippingAdressId = criteria.Orderinfo.ShippingAdressId;
                orderToUpdate.OrderTypeId = await db.ReferenceItem.Where(p=>p.Code == "OrderType_Internal").Select(p=>p.Id).FirstOrDefaultAsync();
                orderToUpdate.ShipmentInfoId = criteria.Orderinfo.ShipmentInfoId;
                orderToUpdate.StatusReferenceItemId = criteria.Orderinfo.StatusReferenceItemId;
                orderToUpdate.TaxRateId = criteria.Orderinfo.TaxRateId;
                orderToUpdate.CustomerId = criteria.Orderinfo.CustomerId;

                if (criteria.Orderinfo.Id == 0)
                {
                    await db.AddAsync(orderToUpdate);
                    await db.SaveChangesAsync();

                    var orderInfoStatusLog = new OrderInfoStatusLog();

                    orderInfoStatusLog.OrderInfoId = orderToUpdate.Id;

                    orderInfoStatusLog.NewStatusId = orderToUpdate.StatusReferenceItemId;
                    orderInfoStatusLog.UserId = criteria.CreatedOrUpdatedBy;
                    orderInfoStatusLog.ActionTime = DateTime.Now;

                    db.OrderInfoStatusLog.Add(orderInfoStatusLog);

                    await db.SaveChangesAsync();
                }
                else
                {
                    db.Update(orderToUpdate);
                    await db.SaveChangesAsync();
                }

                /* Step 1: remove all the product in order */
                var PreviousOrderProducts = await db.OrderProduct.Where(p => p.OrderId == orderToUpdate.Id).ToListAsync();
                db.RemoveRange(PreviousOrderProducts);

                float TotalPrice = 0;
                List<OrderProduct> products = new List<OrderProduct>();
                /* Step 2: Add product in order */
                if (criteria.References.Count() > 0)
                {
                    foreach (var item in criteria.References)
                    {
                        var reference = new OrderProduct();
                        reference.ReferenceId = item.ReferenceId;
                        reference.Quantity = item.Quantity;
                        reference.UnitPrice = item.Price;
                        reference.OrderId = orderToUpdate.Id;

                        TotalPrice = TotalPrice + (item.Price.Value * item.Quantity);

                        products.Add(reference);
                    }
                }


                await db.AddRangeAsync(products);

                orderToUpdate.TotalPrice = TotalPrice;

                db.Update(orderToUpdate);

                await db.SaveChangesAsync();

                return Json(orderToUpdate.Id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        

    }
}