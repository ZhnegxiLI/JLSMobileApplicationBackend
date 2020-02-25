using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IAdressRepository
    {
        Task<long> CreateOrUpdateAdress(Adress adress);
        Task<long> CreateOrUpdateUserShippingAdress(long adressId, int userId);

        Task<Adress> GetUserFacturationAdress(int userId);
        Task<Adress> GetAdressByIdAsync(long Id);
        Task<List<Adress>> GetUserShippingAdress(int userId);
    }
}
