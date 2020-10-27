using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Message;
using JLSMobileApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    [Authorize]
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class MessageController : Controller
    {

        private readonly IMessageRepository _message;
        private readonly ISendEmailAndMessageService _email;

        public MessageController(IMessageRepository messageRepository, ISendEmailAndMessageService sendMessageService)
        {
            _message = messageRepository;
            _email = sendMessageService;
        }


        public class SaveMessageCriteria
        {
            public int? FromUserId { get; set; }

            public int? ToUserId { get; set; }

            public Message Message { get; set; }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<long> SaveMessage([FromBody]SaveMessageCriteria criteria)
        {
            try
            {
                var result = await _message.CreateMessage(criteria.Message, criteria.FromUserId, criteria.ToUserId);
                if (criteria.ToUserId == null)
                {
                    await _email.ClientMessageToAdminAsync(criteria.Message.SenderEmail, criteria.Message.Body);
                }
              
                return result;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetMessageByUserAndStatus(int UserId, bool? Status, int Step, int Begin)
        {
            try
            {
                var result = await _message.GetMessageByUserAndStatus(UserId, Status);
                return  Json(new
                {
                    TotalCount = result.Count,
                    List = result.Skip(Begin * Step).Take(Step).ToList()
                });
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        
        public class UpdateMessageStatusCriteria
        {
            public long MessageId { get; set; }
            public bool Status { get; set; }
            public int? UserId { get; set; }
        }
        [HttpPost]
        public async Task<long> UpdateMessageStatus([FromBody]UpdateMessageStatusCriteria criteria)
        {
            try
            {
                return await _message.UpdateMessageStatus(criteria.MessageId, criteria.Status, criteria.UserId);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        [HttpGet]
        public async Task<int> GetNoReadMessageCount(int UserId)
        {
            try
            {
                var result = await _message.GetMessageByUserAndStatus(UserId, false);
                return result.Count();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

    }

}