using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    //[Authorize] //todo add authorize
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IAdressRepository _adressRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly ISendEmailAndMessageService _sendEmailAndMessageService;

        public OrderController(IMapper mapper, IOrderRepository order, IAdressRepository adress, UserManager<User> userManager, IProductRepository product, ISendEmailAndMessageService sendEmailAndMessageService)
        {
            _userManager = userManager;
            _orderRepository = order;
            _adressRepository = adress;
            _mapper = mapper;
            _productRepository = product;
            _sendEmailAndMessageService = sendEmailAndMessageService;
        }
        public class SaveOrderCriteria{
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
            try
            {
                /* Step1: Get shipping and facturation address */
                var shippingAddress = await _adressRepository.GetAdressByIdAsync(criteria.ShippingAdressId);
                var facturationAddress = await _adressRepository.GetAdressByIdAsync(criteria.FacturationAdressId) ;

                /* Step2: Save both address for order */
                var shippingAddressToInsert = new Adress();
                shippingAddressToInsert.ZipCode = shippingAddress.ZipCode;
                shippingAddressToInsert.ContactTelephone = shippingAddress.ContactTelephone;
                shippingAddressToInsert.ContactFax = shippingAddress.ContactFax;
                shippingAddressToInsert.ContactLastName = shippingAddress.ContactLastName;
                shippingAddressToInsert.ContactFirstName = shippingAddress.ContactFirstName;
                shippingAddressToInsert.SecondLineAddress = shippingAddress.SecondLineAddress;
                shippingAddressToInsert.FirstLineAddress = shippingAddress.FirstLineAddress;
                shippingAddressToInsert.City = shippingAddress.City;
                shippingAddressToInsert.EntrepriseName = shippingAddress.EntrepriseName;
                shippingAddressToInsert.CountryId = shippingAddress.CountryId;
                shippingAddressToInsert.IsDefaultAdress = true;

                var facturationAddressToInsert = new Adress();
                facturationAddressToInsert.ZipCode = facturationAddress.ZipCode;
                facturationAddressToInsert.ContactTelephone = facturationAddress.ContactTelephone;
                facturationAddressToInsert.ContactFax = facturationAddress.ContactFax;
                facturationAddressToInsert.ContactLastName = facturationAddress.ContactLastName;
                facturationAddressToInsert.ContactFirstName = facturationAddress.ContactFirstName;
                facturationAddressToInsert.SecondLineAddress = facturationAddress.SecondLineAddress;
                facturationAddressToInsert.FirstLineAddress = facturationAddress.FirstLineAddress;
                facturationAddressToInsert.City = facturationAddress.City;
                facturationAddressToInsert.EntrepriseName = facturationAddress.EntrepriseName;
                facturationAddressToInsert.CountryId = facturationAddress.CountryId;

                var shippingAddressId = await _adressRepository.CreateOrUpdateAdress(shippingAddressToInsert);
                var facturationAddressId = await _adressRepository.CreateOrUpdateAdress(facturationAddressToInsert);


                /* Step3: Customer info */
                var User = await _userManager.FindByIdAsync(criteria.UserId.ToString());
                long CustomerId = 0;
                if (User!=null)
                {
                    CustomerInfo customer = new CustomerInfo();
                    customer.PhoneNumber = User.PhoneNumber;
                    customer.Siret = User.Siret;
                    customer.EntrepriseName = User.EntrepriseName;
                    customer.Email = User.Email;

                    CustomerId = await _orderRepository.SaveCustomerInfo(customer, criteria.UserId);
                }

                long ClientRemarkId = 0;
                /* Step4: save Admin remark info */
                if (criteria.ClientRemark!= "")
                {
                    var ClientRemark = new Remark();
                    ClientRemark.Text = criteria.ClientRemark;
                    ClientRemark.UserId = criteria.UserId;
                    ClientRemark.CreatedBy = criteria.UserId;
                    ClientRemark.CreatedOn = DateTime.Now;
                    ClientRemarkId = await _orderRepository.SaveOrderRemark(ClientRemark, criteria.UserId);
                }

                /* Step5: reforme the productlist */
                List<long> ReferenceList = new List<long>();

                criteria.References.ForEach(p =>
                {
                    ReferenceList.Add(p.ReferenceId);
                });
                var ProductList = await _productRepository.GetProductInfoByReferenceIds(ReferenceList, "fr");
                if (ProductList!=null)
                {
                    var FormatedReferenceList = (from p in ProductList
                                                 join ri in criteria.References on p.ReferenceId equals ri.ReferenceId
                                                 select new OrderProductViewModelMobile()
                                                 {
                                                     Price = p.Price,
                                                     Quantity = ri.Quantity,
                                                     ReferenceId = ri.ReferenceId
                                                 }).ToList();
                    var orderId = await _orderRepository.SaveOrder(FormatedReferenceList, shippingAddressId, criteria.FacturationAdressId, criteria.UserId, ClientRemarkId, CustomerId);
                    await _sendEmailAndMessageService.CreateOrUpdateOrderAsync(orderId, "CreateNewOrder");
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
                throw exc;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOrdersListByUserId(int UserId, string StatusCode, string Lang)
        {
            try
            {
               
                return Json(new ApiResult()
                {
                    Data = await _orderRepository.GetOrdersListByUserId(UserId, StatusCode, Lang),
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
    }
}