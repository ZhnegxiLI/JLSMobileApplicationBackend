using JLSDataModel.Models.Adress;
using JLSDataModel.Models.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<Adress> GetUserFacturationAdress(int userId);

        Task<List<Adress>> GetUserShippingAdress(int userId);

        Task<List<User>> GetUserListByRole(List<string> Roles);
    }
}
