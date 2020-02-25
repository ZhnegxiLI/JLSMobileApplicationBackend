using JLSDataModel.Models.Adress;
using JLSDataModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IOrderRepository
    {
        Task<long> SaveOrder(List<OrderProductViewModelMobile> References, Adress adress, long FacturationAdressId, int UserId);

        Task<List<OrdersListViewModel>> GetAllOrdersWithInterval(string lang, int intervalCount, int size, string orderActive, string orderDirection);
        Task<OrderViewModel> GetOrderById(long id, string lang);
    }
}
