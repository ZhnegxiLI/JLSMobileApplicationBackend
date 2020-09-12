﻿using JLSDataAccess;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models;
using JLSDataModel.Models.Message;
using JLSDataModel.Models.User;
using JLSMobileApplication.Heplers;
using JLSMobileApplication.Services.EmailTemplateModel;
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
    public class SendEmailAndMessageService: ISendEmailAndMessageService
    {
        private readonly AppSettings _appSettings;
        private readonly JlsDbContext db;
        private readonly IEmailService _email;
        private readonly UserManager<User> _userManager;
        private readonly IMessageRepository _messageRepository;

        public readonly IExportService _exportService;

        private IViewRenderService _view = null;

        public SendEmailAndMessageService(IOptions<AppSettings> appSettings, JlsDbContext context,IEmailService email, UserManager<User> userManager, IMessageRepository messageRepository, IExportService export, IViewRenderService view)
        {
            _appSettings = appSettings.Value;
            db = context;
            _email = email;
            _userManager = userManager;
            _messageRepository = messageRepository;
            _exportService = export;

            _view = view;
        }

        public async Task<long> CreateOrUpdateOrderAsync(long OrderId, string Type)
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
                    var messageClientTemplate = emailModelClient.MessageBody;
                    var emailClientTemplate = emailModelClient.Body;
                    var emailAdminTemplate = emailModelAdmin.Body;
                    if (Type == "CreateNewOrder")
                    {
                        ///* Replace client email */
                        //emailClientTemplate = emailClientTemplate.Replace("{numerodecommande}", order.Id.ToString());
                        //emailClientTemplate = emailClientTemplate.Replace("{username}", customerInfo.Email);
                        ///* Replace admin email */
                        //emailAdminTemplate = emailAdminTemplate.Replace("{numerodecommande}", order.Id.ToString());

                        ///* Replace client notice*/
                        messageClientTemplate = messageClientTemplate.Replace("{numerodecommande} ", order.Id.ToString());

                        emailClientTemplate = await _view.RenderToStringAsync("EmailTemplate/NewOrderClient", new OrderEmailModel()
                        {
                     
                            Username = customerInfo.Email,
                            OrderNumber = order.Id.ToString()
                        });


                        emailAdminTemplate = await _view.RenderToStringAsync("EmailTemplate/NewOrderAdmin", new OrderEmailModel()
                        {

                            Username = customerInfo.Email,
                            OrderNumber = order.Id.ToString()
                        });


                    }
                    else if (Type == "UpdateOrder" )
                    {
                        /* Replace client email */

                        emailClientTemplate = await _view.RenderToStringAsync("EmailTemplate/ModifyOrderClient", new OrderEmailModel()
                        {

                            Username = customerInfo.Email,
                            OrderNumber = order.Id.ToString()
                        });

                        emailAdminTemplate = await _view.RenderToStringAsync("EmailTemplate/ModifyOrderAdmin", new OrderEmailModel()
                        {
                            Username = customerInfo.Email,
                            OrderNumber = order.Id.ToString()
                        });
                    }

                    // todo 客户自下单,发送信息至后台
                    if (orderType.Code == "OrderType_External")
                    {
                        var Message = new Message();
                        Message.Title = emailModelClient.Title;
                        Message.Body = messageClientTemplate;
                        Message.IsReaded = false;
                        await this._messageRepository.CreateMessage(Message,null, order.UserId);
                    }

                    // Generate invoice pdf 
                    string pdfPath = await _exportService.ExportPdf(order.Id, "Fr"); // todo make language configurable 
                    
                    // todo: 改变发送逻辑, 目前的发送方式导致下单过慢,可加入一表格中之后定时发送
                    // 发送邮件
                    /* 1.发给客户 */
                    if (customerInfo.Email != null)
                    {
                        //_email.SendEmail(customerInfo.Email, emailModelClient.Title, emailClientTemplate);
                        await PushEmailIntoDb(customerInfo.Email, emailModelClient.Title, emailClientTemplate, pdfPath); 
                    }
                    /* 2.发给内部人员 */
                    foreach (var admin in adminEmails)
                    {
                        if (admin != null)
                        {
                            await PushEmailIntoDb(admin, emailModelAdmin.Title, emailAdminTemplate, pdfPath);
                            //_email.SendEmail(admin, emailModelAdmin.Title, emailAdminTemplate);
                        }
                    }
                    return order.Id;
                }
            }
            return 0;
        }

        public async Task<int> ResetPasswordOuConfirmEmailLinkAsync(int UserId, string Link, string Type)
        {
            EmailTemplate emailModelClient = null;

            emailModelClient = db.EmailTemplate.Where(p => p.Name == Type).FirstOrDefault();
            var user = db.Users.Find(UserId);

            if (emailModelClient!=null && user != null && user.Email !=null)
            {
                var emailClientTemplate = emailModelClient.Body;
                emailClientTemplate = emailClientTemplate.Replace("{email}", user.Email);
                emailClientTemplate = emailClientTemplate.Replace("{username}", user.Email);
                emailClientTemplate = emailClientTemplate.Replace("{Link}", Link);
                if (Type == "EmailConfirmation")
                {
                    emailClientTemplate = await _view.RenderToStringAsync("EmailTemplate/ActiverMonCompte", new ActiverMonCompteModel()
                    {
                        ConfirmationLink = Link,
                        Username =  user.Email,
                        Entreprise = user.EntrepriseName,
                        Phone = user.PhoneNumber
                    });
                }
                else if (Type == "ResetPassword")
                {
                    emailClientTemplate = await _view.RenderToStringAsync("EmailTemplate/ActiverMonCompte", new ActiverMonCompteModel()
                    {
                        ConfirmationLink = Link,
                        Username = user.Email,
                        Entreprise = user.EntrepriseName,
                        Phone = user.PhoneNumber
                    });
                }
                // todo same for reset password
   
                await PushEmailIntoDb(user.Email, emailModelClient.Title, emailClientTemplate, null);
                //_email.SendEmail(user.Email, emailModelClient.Title, emailClientTemplate);
                return user.Id;
            }
            return 0;
        }


        public async Task<int> AfterResetPasswordOuConfirmEmailLinkAsync(int UserId,string Type)
        {
            EmailTemplate emailModelClient = null;

            db.EmailTemplate.Where(p => p.Name == Type).FirstOrDefault();
            var user = db.Users.Find(UserId);

            if (emailModelClient != null && user != null && user.Email != null)
            {
                var emailClientTemplate = emailModelClient.Body;
                if (Type == "AfterResetPassword")
                {
                    emailClientTemplate = await _view.RenderToStringAsync("EmailTemplate/AfterReinitialiserPassword", null);
                }
                else if (Type == "AfterEmailConfirmation")
                {
                    emailClientTemplate = await _view.RenderToStringAsync("EmailTemplate/AfterActiverMonCompte", null);
                }
                // todo send internal message
                var Message = new Message();

                await PushEmailIntoDb(user.Email, emailModelClient.Title, emailClientTemplate,null);
                //_email.SendEmail(user.Email, emailModelClient.Title, emailClientTemplate);
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
                    _email.SendEmail(c, emailModelClient.Title, emailClientTemplate, null);
                }
                return 1;
            }
            return 0;
        }

        public async Task<int> PushEmailIntoDb(string ToEmail, string Title, string Body, string AttachmentPath)
        {
            var Email = new EmailToSend();
            Email.ToEmail = ToEmail;
            Email.Title = Title;
            Email.Body = Body;
            Email.Attachment = AttachmentPath;
            Email.IsSended = false;
            await db.AddAsync(Email);
            await db.SaveChangesAsync();

            return 1;
        }

        public void SendEmailInDb()
        {
            var EmailsToSend = db.EmailToSend.Where(p => p.IsSended == false || p.IsSended == null).ToList();
            if (EmailsToSend.Count()>0)
            {
                foreach (var Email in EmailsToSend)
                {
                    _email.SendEmail(Email.ToEmail, Email.Title, Email.Body, Email.Attachment);
                    Email.IsSended = true;
                    db.Update(Email);
                }
                db.SaveChanges();
            }
       
        }


    }
}
