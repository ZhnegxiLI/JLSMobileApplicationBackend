﻿using JLSDataModel.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductCategoryViewModel>> GetProductMainCategory(string Lang);

        Task<List<ProductCategoryViewModel>> GetProductSecondCategory(long MainCategoryReferenceId, string Lang);

        Task<ProductListViewModel> GetProductListBySecondCategory(long SecondCategoryReferenceId, string Lang, int begin, int step);
    }
}
