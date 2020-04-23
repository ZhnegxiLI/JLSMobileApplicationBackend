using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public interface ISendEmailAndMessageService
    {
        Task<long> CreateOrUpdateOrderAsync(long OrderId, string Type);

        Task<int> ResetPasswordOuConfirmEmailLinkAsync(int UserId, string Link, string Type);
        Task<int> AfterResetPasswordOuConfirmEmailLinkAsync(int UserId, string Type);
        int SendAdvertisement(string Type);

        void SendEmailInDb();
    }
}
