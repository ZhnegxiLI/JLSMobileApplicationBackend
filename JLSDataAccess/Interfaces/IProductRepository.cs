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

        Task<dynamic> GetProductListBySalesPerformance(string Lang, int begin, int step);

        Task<dynamic> GetProductPhotoPathById(long ProductId);

        Task<long> SavePhotoPath(long ProductId, string Path);

        Task<ProductListViewModel> GetProductListByPublishDate(string Lang, int begin, int step);


        Task<List<ProductComment>> GetAllProductCommentList(int begin, int step);
        Task<List<ProductCommentViewModel>> GetProductCommentListByCriteria(long? ProductId, long? UserId, string Lang);

        Task<long> RemoveProductCommentById(long ProductCommentId);

        Task<long> SaveProductComment(long ProductId, string Title, string Body, int Level, int UserId);
        /*
         *  Admin zoom
         */
        Task<List<dynamic>> AdvancedProductSearchByCriteria(string ProductLabel, long MainCategoryReferenceId, List<long> SecondCategoryReferenceId, bool? Validity, string Lang);

        Task<long> SaveProductInfo(long ProductId, long ReferenceId, int? QuantityPerBox, int? MinQuantity, float? Price, long? TaxRate, string Description, string Color, string Material, string Size, int? CreatedOrUpdatedBy);

        Task<ListViewModelWithCount<ProductsListViewModel>> GetAllProduct(string lang, int intervalCount, int size, string orderActive, string orderDirection, string filter);
        Task<dynamic> GetProductById(long id,string Lang);
        Task<int> RemoveImageById(long id);
        Task<List<ProductsListViewModel>> SearchProducts(string lang, string filter);

    }
}
