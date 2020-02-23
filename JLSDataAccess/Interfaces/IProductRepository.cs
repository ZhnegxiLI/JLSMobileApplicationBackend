using JLSDataModel.Models;
using JLSDataModel.Models.Product;
using JLSDataModel.ViewModels;
using Microsoft.AspNetCore.Http;
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


        Task<List<ProductComment>> GetProductCommentList();
        Task<List<ProductComment>> GetProductCommentListByProductId(long ProductId);
        Task<List<ProductComment>> GetProductCommentListByUserId(int UserId);
        /*
         *  Admin zoom
         */

        Task<List<ReferenceItemViewModel>> GetProductCategory(string lang);
        Task<List<ReferenceItemViewModel>> GetTaxRate();

        Task<int> SaveProduct(Product product, List<IFormFile> images, List<ReferenceLabel> labels);
        Task<List<ProductsListViewModel>> GetAllProduct(string lang, int intervalCount, int size, string orderActive, string orderDirection);
        Task<ProductViewModel> GetProductById(long id);
        Task<int> RemoveImageById(long id);
        Task<List<ProductsListViewModel>> SearchProducts(string lang, string filter);

    }
}
