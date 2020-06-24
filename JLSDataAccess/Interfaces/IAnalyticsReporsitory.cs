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

        Task<List<dynamic>> GetTeamMemberSalesPerformance();

        Task<List<dynamic>> GetInternalExternalSalesPerformance(string Lang);

        Task<List<dynamic>> GetSalesPerformanceByStatus(string Lang);

        Task<List<dynamic>> GetSalesPerformanceByYearMonth();
    }
}
