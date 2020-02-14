using JLSDataModel.Models.Adress;
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
    }
}
