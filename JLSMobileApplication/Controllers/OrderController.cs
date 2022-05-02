using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using JLSDataModel.Models.Adress;
using JLSDataModel.Models.Order;
using JLSDataModel.Models.User;
using JLSDataModel.ViewModels;
using JLSMobileApplication.Services;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace JLSMobileApplication.Controllers
{
    [Authorize]
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IAdressRepository _adressRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISendEmailAndMessageService _sendEmailAndMessageService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        public OrderController(IOrderRepository order, IAdressRepository adress, UserManager<User> userManager, IProductRepository product, ISendEmailAndMessageService sendEmailAndMessageService, IWebHostEnvironment env, ILogger<OrderController> logger)
        {
            _userManager = userManager;
            _orderRepository = order;
            _adressRepository = adress;
            _productRepository = product;
            _sendEmailAndMessageService = sendEmailAndMessageService;
            _env = env;
            _logger = logger;
        }

        public class SaveOrderCriteria
        {
            public SaveOrderCriteria()
            {
                this.References = new List<OrderProductViewModelMobile>();
            }

            public long ShippingAdressId { get; set; }
            public long FacturationAdressId { get; set; }
            public int UserId { get; set; }
            public List<OrderProductViewModelMobile> References { get; set; }

            public string ClientRemark { get; set; }
        }

        [HttpPost]
        public async Task<JsonResult> SaveOrder([FromBody] SaveOrderCriteria criteria)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required,new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    var orderId = await SaveOrderAction(criteria);
                    if (orderId != null && orderId > 0)
                    {
                        if (!_env.IsDevelopment())
                        {
                            await _sendEmailAndMessageService.CreateOrUpdateOrderAsync((long)orderId, "CreateNewOrder");
                        }

                        var User = await _userManager.FindByIdAsync(criteria.UserId.ToString());
                        return Json(new ApiResult()
                        {
                            Data = orderId,
                            Msg = "OK",
                            Success = true,
                            DataExt = User.Email
                        });
                    }
                    else
                    {
                        return Json(new ApiResult()
                        {
                            Data = null,
                            Msg = "Fail",
                            Success = false
                        });
                    }
                }
                catch (Exception exc)
                {
                    // Log and rollback
                    scope.Dispose();
                    _logger.LogError(exc, "OrderController SaveOrder method fail");
                    return Json(new ApiResult()
                    {
                        Data = null,
                        Msg = "Fail",
                        Success = false
                    });
                }
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOrdersListByUserId(int UserId, string StatusCode, string Lang, int? Step, int? Begin)
        {
            try
            {
                var result = await _orderRepository.GetOrdersListByUserId(UserId, StatusCode, Lang);
                if (Step != null && Step > 0 && Begin != null && Begin >= 0)
                {
                    result = result.Skip((int)Begin * (int)Step).Take((int)Step).ToList();
                }

                return Json(new ApiResult()
                {
                    Data = result,
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOrdersListByOrderId(long OrderId, string Lang)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _orderRepository.GetOrdersListByOrderId(OrderId, Lang),
                    Msg = "OK",
                    Success = true
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private async Task<long?> SaveOrderAction(SaveOrderCriteria criteria)
        {
            /* Step1: Get shipping and facturation address */
            var shippingAddress = await _adressRepository.GetAdressByIdAsync(criteria.ShippingAdressId);
            var facturationAddress = await _adressRepository.GetAdressByIdAsync(criteria.FacturationAdressId);


            /* Step2: Save both address for order */
            var shippingAddressToInsert = MappingAddress(shippingAddress);
            shippingAddressToInsert.IsDefaultAdress = true;

            var facturationAddressToInsert = MappingAddress(facturationAddress);

            var shippingAddressId = await _adressRepository.CreateOrUpdateAdress(shippingAddressToInsert);
            var facturationAddressId = await _adressRepository.CreateOrUpdateAdress(facturationAddressToInsert);

            /* Step3: Customer info */
            var User = await _userManager.FindByIdAsync(criteria.UserId.ToString());
            long CustomerId = 0;
            if (User != null)
            {
                var customer = MappingCustomerInfo(User);
                CustomerId = await _orderRepository.SaveCustomerInfo(customer, criteria.UserId);
            }

            long ClientRemarkId = 0;
            /* Step4: save Admin remark info */
            if (criteria.ClientRemark != "")
            {
                var ClientRemark = MapingOrderRemark(criteria);
                ClientRemarkId = await _orderRepository.SaveOrderRemark(ClientRemark, criteria.UserId);
            }

            /* Step5: reforme the productlist */
            var ReferenceList = criteria.References.Select(x => x.ReferenceId).ToList(); ;
            var ProductList = await _productRepository.GetProductInfoByReferenceIds(ReferenceList, "fr");
            long? orderId = null;
            if (ProductList != null)
            {
                var FormatedReferenceList = (from p in ProductList
                                             join ri in criteria.References on p.ReferenceId equals ri.ReferenceId
                                             select new OrderProductViewModelMobile()
                                             {
                                                 Price = p.Price, // Modify accroding to client specification
                                                 UnityQuantity = (int)p.QuantityPerBox,
                                                 Quantity = ri.Quantity,
                                                 ReferenceId = ri.ReferenceId
                                             }).ToList();
                orderId = await _orderRepository.SaveOrder(FormatedReferenceList, shippingAddressId, criteria.FacturationAdressId, criteria.UserId, ClientRemarkId, CustomerId);
            }
            return orderId;
        }

        private Adress MappingAddress(Adress address)
        {
            var addressToInsert = new Adress();
            addressToInsert.ZipCode = address.ZipCode;
            addressToInsert.ContactTelephone = address.ContactTelephone;
            addressToInsert.ContactFax = address.ContactFax;
            addressToInsert.ContactLastName = address.ContactLastName;
            addressToInsert.ContactFirstName = address.ContactFirstName;
            addressToInsert.SecondLineAddress = address.SecondLineAddress;
            addressToInsert.FirstLineAddress = address.FirstLineAddress;
            addressToInsert.City = address.City;
            addressToInsert.EntrepriseName = address.EntrepriseName;
            addressToInsert.Country = address.Country;

            return addressToInsert;
        }

        private CustomerInfo MappingCustomerInfo(User user)
        {
            CustomerInfo customer = new CustomerInfo();

            customer.PhoneNumber = user.PhoneNumber;
            customer.Siret = user.Siret;
            customer.EntrepriseName = user.EntrepriseName;
            customer.Email = user.Email;
            customer.UserId = user.Id;

            return customer;
        }

        private Remark MapingOrderRemark(SaveOrderCriteria criteria)
        {
            var ClientRemark = new Remark();
            ClientRemark.Text = criteria.ClientRemark;
            ClientRemark.UserId = criteria.UserId;
            ClientRemark.CreatedBy = criteria.UserId;
            ClientRemark.CreatedOn = DateTime.Now;

            return ClientRemark;
        }
    }
}