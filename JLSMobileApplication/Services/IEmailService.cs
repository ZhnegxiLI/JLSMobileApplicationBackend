using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public interface IEmailService
    {
        string SendEmail(string ToEmail,string Subjet, string Message, string AttachmentPath);
    }
}
