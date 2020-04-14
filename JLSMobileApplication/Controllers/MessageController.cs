using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class MessageController : Controller
    {

        private readonly IMessageRepository _message;

        public MessageController(IMessageRepository messageRepository)
        {
            _message = messageRepository;
        }


        public class SaveMessageCriteria
        {
            public int? FromUserId { get; set; }

            public int? ToUserId { get; set; }

            public Message Message { get; set; }
        }
        [HttpPost]
        public async Task<long> SaveMessage([FromBody]SaveMessageCriteria criteria)
        {
            try
            {
               return await  _message.CreateMessage(criteria.Message, criteria.FromUserId, criteria.ToUserId);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }

}