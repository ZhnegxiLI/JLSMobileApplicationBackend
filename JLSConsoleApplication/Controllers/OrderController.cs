﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSConsole.Heplers;
using JLSDataAccess.Interfaces;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSConsoleApplication.Controllers
{
    // TODO: 不能把内部错误信息传到前端显示
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<JsonResult> GetAllOrders(string lang, int intervalCount, int size, string orderActive, string orderDirection)
        {
            ApiResult result;
            try
            {
                List<OrdersListViewModel> data = await _orderRepository.GetAllOrdersWithInterval(lang, intervalCount, size, orderActive, orderDirection);
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
            }
            catch (Exception e)
            {
                result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderById(long id, string lang)
        {
            ApiResult result;
            try
            {
                OrderViewModel data = await _orderRepository.GetOrderById(id, lang);
                result = new ApiResult() { Success = true, Msg = "OK", Type = "200", Data = data };
            }
            catch (Exception e)
            {
                result = new ApiResult() { Success = false, Msg = e.Message, Type = "500" };
            }
            return Json(result);
        }
    }
}