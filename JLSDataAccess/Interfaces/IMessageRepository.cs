using JLSDataModel.Models.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IMessageRepository
    {

        Task<long> CreateMessage(Message message, int? FromUser, int? ToUser);

        Task<List<dynamic>> GetMessageByUserAndStatus(int ToUserId, bool? IsReaded);

        Task<long> UpdateMessageStatus(long MessageId, bool Status, int? UserId);
    }
}
