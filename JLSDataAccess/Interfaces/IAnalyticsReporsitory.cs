using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IAnalyticsReporsitory
    {
        Task<List<dynamic>> GetAdminSalesPerformanceDashboard(string Lang);
    }
}
