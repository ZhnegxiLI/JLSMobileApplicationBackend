using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Adress;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public OrderController(IOrderRepository orderRepository, IMapper mapper, IAdressRepository adressRepository)
        {
            this._orderRepository = orderRepository;
            _mapper = mapper;
            _adressRepository = adressRepository;
        }

        public class AdvancedOrderSearchCriteria
        {
            public string Lang { get; set; }
            public int? UserId { get; set; }

            public DateTime? FromDate { get; set; }

            public DateTime? ToDate { get; set; }

            public long? StatusId { get; set; }
            
            public long? OrderId { get; set; }

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
            public Adress ShippingAddress { get; set; }

            public Adress FacturationAddress { get; set; }

            public List<OrderProductViewModelMobile> References { get; set; }

            public long OrderId { get; set; }

            public int CreatedOrUpdatedBy { get; set; }

            public long StatusReferenceId { get; set; }
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

                /* Step 3: save order info */


                var result = await _orderRepository.SaveAdminOrder(criteria.OrderId, criteria.References, ShippingAddressId, FacturationAddressId, criteria.CreatedOrUpdatedBy, criteria.StatusReferenceId);

                return Json(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        

    }
}