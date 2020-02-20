using JLSDataModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IOrderRepository
    {
        Task<long> SaveOrder(List<OrderProductViewModel> References, long ShippingAdressId, long FacturationAdressId, int UserId);
    }
}
