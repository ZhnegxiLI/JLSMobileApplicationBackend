using JLSDataModel.Models.Adress;
using JLSDataModel.Models.Analytics;
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


        Task<List<dynamic>> GetTopSaleProduct(string Lang, int Limit);

        List<BestClientWidget> GetBestClientWidget(int Limit);
    }
}
