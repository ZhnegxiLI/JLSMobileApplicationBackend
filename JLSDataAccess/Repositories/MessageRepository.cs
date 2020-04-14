using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.Models.User;
using JLSDataModel.Models.Message;

namespace JLSDataAccess.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly JlsDbContext db;

        public MessageRepository(JlsDbContext context)
        {
            db = context;
        }

        public async Task<long> CreateMessage(Message message, int? FromUser, int? ToUser)
        {
            await db.AddAsync(message);
            await db.SaveChangesAsync();
            if (message.Id >0 && (FromUser!=null || ToUser!=null ))
            {
                MessageDestination messageDestination = new MessageDestination();
                messageDestination.FromUserId =  FromUser != null ? FromUser: null;
                messageDestination.ToUserId = ToUser != null ? ToUser : null;

                await db.AddAsync(messageDestination);
                await db.SaveChangesAsync();
            }

            return message.Id;

        }

        
    }
}
