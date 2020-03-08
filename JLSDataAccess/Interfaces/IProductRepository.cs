using JLSDataModel.AdminViewModel;
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


        Task<List<ProductComment>> GetAllProductCommentList(int begin, int step);
        Task<List<ProductCommentViewModel>> GetProductCommentListByProductId(long ProductId,string Lang);
        Task<List<ProductComment>> GetProductCommentListByUserId(int UserId, int begin, int step);

        Task<long> SaveProductComment(long ProductId, string Title, string Body, int Level, int UserId);
        /*
         *  Admin zoom
         */
        Task<List<dynamic>> AdvancedProductSearchByCriteria(string ProductLabel, long MainCategoryReferenceId, List<long> SecondCategoryReferenceId, bool? Validity, string Lang);

        Task<ListViewModelWithCount<ProductsListViewModel>> GetAllProduct(string lang, int intervalCount, int size, string orderActive, string orderDirection, string filter);
        Task<dynamic> GetProductById(long id);
        Task<int> RemoveImageById(long id);
        Task<List<ProductsListViewModel>> SearchProducts(string lang, string filter);

    }
}
