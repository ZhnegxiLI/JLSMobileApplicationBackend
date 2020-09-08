using JLSMobileApplication.Heplers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;

using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public class MailkitEmailService : IEmailService
        {
    
        private readonly AppSettings _appSettings;
        public MailkitEmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public string SendEmail(string ToEmail,string Subjet, string Message, string AttachmentPath)
        {

            try
            {


                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("JLS IMPORT",
                _appSettings.EmailAccount);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(ToEmail,
                ToEmail);
                message.To.Add(to);

                message.Subject = Subjet;
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = Message;
               
                if (AttachmentPath != null)
                {
                    bodyBuilder.Attachments.Add(AttachmentPath);
                }

                message.Body = bodyBuilder.ToMessageBody();


                SmtpClient client = new SmtpClient();
                client.Connect(_appSettings.EmailHost, _appSettings.EmailPort, true);
                client.Authenticate(_appSettings.EmailAccount, _appSettings.EmailPassword);

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            
                return "Email Sent Successfully!"; //todo change to code 
            }
            catch (System.Exception e)
            {
                return e.Message; // todo change to code 
            }
        }
    }
    }
