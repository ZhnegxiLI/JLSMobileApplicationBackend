using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers.AdminService
{

    [Route("admin/[controller]/{action}")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public OrderController(IOrderRepository orderRepository, IMapper mapper)
        {
            this._orderRepository = orderRepository;
            _mapper = mapper;
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

    }
}