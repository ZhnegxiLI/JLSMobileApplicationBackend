using JLSDataAccess;
using JLSDataModel.Models;
using JLSDataModel.Models.Message;
using JLSDataModel.Models.User;
using JLSMobileApplication.Heplers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public class SendEmailAndMessageService
    {
        private readonly AppSettings _appSettings;
        private readonly JlsDbContext db;
        private readonly IEmailService _email;
        private readonly UserManager<User> _userManager;

        public SendEmailAndMessageService(IOptions<AppSettings> appSettings, JlsDbContext context,IEmailService email, UserManager<User> userManager)
        {
            _appSettings = appSettings.Value;
            db = context;
            _email = email;
            _userManager = userManager;
        }

        public long CreateOrUpdateOrder(long OrderId, string Type)
        {
            var adminEmails = (from u in db.Users
                               join ur in db.UserRoles on u.Id equals ur.UserId
                               join r in db.Roles on ur.RoleId equals r.Id
                               where (r.Name == "Admin" || r.Name == "SuperAdmin") && u.Validity == true 
                               select u.Email).ToList();

            var emailModelClient = db.EmailTemplate.Where(p => p.Name == Type +"_Client").FirstOrDefault();
            var emailModelAdmin = db.EmailTemplate.Where(p => p.Name == Type +"_Admin").FirstOrDefault();
            var order = db.OrderInfo.Find(OrderId);
            if (order!=null && emailModelClient != null && emailModelAdmin!=null)
            {
                var orderType = db.ReferenceItem.Where(p => p.Id == order.OrderTypeId).FirstOrDefault();
                var customerInfo = db.CustomerInfo.Where(p => p.Id == order.CustomerId).FirstOrDefault();
                if (customerInfo !=null )
                {
                    // TODO : Replace the email here
                    var emailClientTemplate = emailModelClient.Body;
                    var emailAdminTemplate = emailModelAdmin.Body;
                    if (Type == "CreateNewOrder")
                    {
                        // todo
                    }
                    else if (Type == "UpdateOrder" )
                    {
                        // todo replace
                    }

                    // todo 客户自下单,发送信息至后台
                    if (orderType.Code == "OrderType_External")
                    {
          
                    }
                    // 发送邮件
                    /* 1.发给客户 */
                    if (customerInfo.Email!=null)
                    {
                        _email.SendEmail(customerInfo.Email, emailModelClient.Title, emailClientTemplate);
                    }
                    /* 2.发给内部人员 */
                    foreach(var admin in adminEmails)
                    {
                        if (admin!=null)
                        {
                            _email.SendEmail(admin, emailModelAdmin.Title, emailAdminTemplate);
                        }
                    }
                    return order.Id;
                }
            }
            return 0;
        }

        public int ResetPasswordOuConfirmEmailLink(int UserId, string Link, string Type)
        {
            EmailTemplate emailModelClient = null;

            db.EmailTemplate.Where(p => p.Name == Type).FirstOrDefault();
            var user = db.Users.Find(UserId);

            if (emailModelClient!=null && user != null && user.Email !=null)
            {
                var emailClientTemplate = emailModelClient.Body;
                if (Type== "ResetPassword")
                {
                    // TODO: replace email here 
                }
                else if (Type == "EmailConfirmation")
                {
                    // TODO: replace email here 
                }

                _email.SendEmail(user.Email, emailModelClient.Title, emailClientTemplate);
                return user.Id;
            }
            return 0;
        }


        public int AfterResetPasswordOuConfirmEmailLink(int UserId,string Type)
        {
            EmailTemplate emailModelClient = null;

            db.EmailTemplate.Where(p => p.Name == Type).FirstOrDefault();
            var user = db.Users.Find(UserId);

            if (emailModelClient != null && user != null && user.Email != null)
            {
                var emailClientTemplate = emailModelClient.Body;
                if (Type == "AfterResetPassword")
                {
                    // TODO: replace email here 
                }
                else if (Type == "AfterEmailConfirmation")
                {
                    // TODO: replace email here 
                }
                // todo send internal message
                var Message = new Message();

                _email.SendEmail(user.Email, emailModelClient.Title, emailClientTemplate);
                return user.Id;
            }
            return 0;
        }


        // todo think of how to define the template more efficiencly
        public int SendAdvertisement(string Type)
        {
            EmailTemplate emailModelClient = null;

            db.EmailTemplate.Where(p => p.Name == Type).FirstOrDefault();
            var clients = ( from u in db.Users
                            join ur in db.UserRoles on u.Id equals ur.UserId
                            join r in db.Roles on ur.RoleId equals r.Id
                            where u.Validity == true 
                            select u.Email).ToList();

            if (emailModelClient != null && clients != null && clients.Count()>0)
            {
                var emailClientTemplate = emailModelClient.Body;
                // todo send internal message
                var Message = new Message();
                foreach (var c in clients)
                {
                    _email.SendEmail(c, emailModelClient.Title, emailClientTemplate);
                }
                return 1;
            }
            return 0;
        }


    }
}
