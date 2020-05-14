﻿using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IAdressRepository
    {
        Task<long> CreateOrUpdateAdress(Adress adress);
        Task<long> CreateUserShippingAdress(long adressId, int userId);
        Task<long> CreateFacturationAdress(long adressId, int userId);

        Task<Adress> GetUserFacturationAdress(int userId);
        Task<Adress> GetAdressByIdAsync(long Id);
        Task<List<Adress>> GetUserShippingAdress(int userId);
        Task<Adress> GetUserDefaultShippingAdress(int userId);

        Task<Adress> GetAddressById(long AddressId);

        Task<int> RemoveShippingAddress(long AddressId);
    }
}
