﻿using JLSDataModel.AdminViewModel;
using JLSDataModel.Models;
using JLSDataModel.Models.Website;
using JLSDataModel.ViewModels;
using JLSMobileApplication.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IReferenceRepository
    {
        Task<List<dynamic>> GetReferenceItemsByCategoryLabels(List<string> shortLabels, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsByCategoryIds(string categoryIds, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsById(long referenceId, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsByCode(string referencecode, string lang);


        Task<ReferenceCategory> GetReferenceCategoryByShortLabel(string ShortLabel);


        Task<List<ReferenceCategory>> GetAllCategoryList();

        Task<List<dynamic>> AdvancedSearchReferenceItem(string SearchText, long? ReferenceCategoryId, bool? Validity, long? ParentId, string Lang, bool? IgnoreProduct);

        Task<long> SaveReferenceItem(long ReferenceId, long CategoryId, string Code, long? ParentId, bool Validity, string Value, int? CreatedOrUpdatedBy);

        Task<long> SaveReferenceLabel(long ReferenceId, string Label, string Lang);
        /*
         * Admin zoom
         */

        Task<List<dynamic>> GetAllReferenceItemWithChildren(string Lang);

        Task<List<ReferenceItemViewModel>> GetReferenceItemsByCategoryLabelsAdmin(string shortLabels, string lang);
        Task<ListViewModelWithCount<ReferenceItemViewModel>> GetReferenceItemWithInterval(int intervalCount, int size, string orderActive, string orderDirection, string filter);


        Task<List<ReferenceCategory>> GetAllReferenceCategory();

        Task<List<ReferenceCategory>> GetAllValidityReferenceCategory();

 

        Task<int> CreatorUpdateCategory(ReferenceCategory category);

        Task<bool> CheckReferenceCodeExists(string Code);
        Task<List<WebsiteSlide>> GetWbesiteslides();
    }
}
