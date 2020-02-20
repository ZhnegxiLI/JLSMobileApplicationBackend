using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.ViewModels;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderController(IMapper mapper, IOrderRepository order)
        {
            _orderRepository = order;
            _mapper = mapper;
        }
        public class SaveOrderCriteria{
            public SaveOrderCriteria()
            {
                this.References = new List<OrderProductViewModel>();
            }
            public long ShippingAdressId { get; set; }
            public long FacturationAdressId { get; set; }
            public int UserId { get; set; }
            public List<OrderProductViewModel> References { get; set; }

        }

        [HttpPost]
        public async Task<JsonResult> SaveOrder([FromBody] SaveOrderCriteria criteria)
        {
            try
            {
                return Json(new ApiResult()
                {
                    Data = await _orderRepository.SaveOrder(criteria.References, criteria.ShippingAdressId,criteria.FacturationAdressId,criteria.UserId),
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