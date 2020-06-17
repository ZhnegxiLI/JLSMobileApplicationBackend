using JLSDataAccess.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.hubs
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _message;
        private readonly IUserRepository _userRepository;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _message = messageRepository;
            _userRepository = userRepository;
        }
        public async Task NewMessage(Message msg)
        {
            var username = Context.User.Identity.Name;
            await Clients.All.SendAsync("MessageReceived", msg);

            await _userRepository.InsertDialog(msg.message, (int)msg.fromUser, msg.toUser);
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }
    }
}
