using JLSDataModel.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductCategoryViewModel>> GetProductMainCategory(string Lang);

        Task<List<ProductCategoryViewModel>> GetProductSecondCategory(long MainCategoryReferenceId, string Lang);

        Task<ProductListViewModel> GetProductListBySecondCategory(long SecondCategoryReferenceId, string Lang, int begin, int step);

        Task<List<ProductListData>> GetProductInfoByReferenceIds(List<long> ReferenceIds, string Lang);

        Task<ProductListViewModel> GetProductListBySalesPerformance(string Lang, int begin, int step);

        Task<ProductListViewModel> GetProductListByPublishDate(string Lang, int begin, int step);
    }
}
