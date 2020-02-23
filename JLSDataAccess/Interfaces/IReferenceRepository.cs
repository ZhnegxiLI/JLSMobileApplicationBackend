﻿using JLSDataModel.AdminViewModel;
using JLSDataModel.Models;
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
        Task<List<ReferenceItemViewModelMobile>> GetReferenceItemsByCategoryLabels(string shortLabel, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsByCategoryIds(string categoryIds, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsById(long referenceId, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsByCode(string referencecode, string lang);


        /*
         * Admin zoom
         */
        Task<List<ReferenceItemViewModel>> GetReferenceItemsByCategoryLabelsAdmin(string shortLabels, string lang);
        Task<ListViewModelWithCount<ReferenceItemViewModel>> GetReferenceItemWithInterval(int intervalCount, int size, string orderActive, string orderDirection, string filter);


        Task<List<ReferenceCategory>> GetAllReferenceCategory();

        Task<List<ReferenceCategory>> GetAllValidityReferenceCategory();

        Task<int> CreatorUpdateItem(ReferenceItem item, List<ReferenceLabel> labels);

        Task<int> CreatorUpdateCategory(ReferenceCategory category);
        List<ReferenceLabel> CheckLabels(List<ReferenceLabel> labels, long referenceItemId);
    }
}
