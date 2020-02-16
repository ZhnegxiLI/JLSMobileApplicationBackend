﻿using JLSMobileApplication.Heplers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public class EmailService:IEmailService
        {
        /// <summary>
        /// 只用于测试目的请勿在生产环境中放入此代码
        /// </summary>
        private readonly AppSettings _appSettings;
        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public string SendEmail(string ToEmail,string Subjet, string Message)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential(_appSettings.EmailAccount , _appSettings.EmailPassword);
                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_appSettings.EmailAccount),
                    Subject = Subjet,
                    Body = Message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(ToEmail));
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
                return "Email Sent Successfully!"; //todo change to code 
            }
            catch (System.Exception e)
            {
                return e.Message; // todo change to code 
            }
        }
    }
    }