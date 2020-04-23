using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public interface ISendEmailAndMessageService
    {
        Task<long> CreateOrUpdateOrderAsync(long OrderId, string Type);

        int ResetPasswordOuConfirmEmailLink(int UserId, string Link, string Type);
        int AfterResetPasswordOuConfirmEmailLink(int UserId, string Type);
        int SendAdvertisement(string Type);
    }
}
