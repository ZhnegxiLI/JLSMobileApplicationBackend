using JLSDataModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IReferenceRepository
    {
        Task<List<dynamic>> GetReferenceItemsByCategoryLabels(string shortLabel, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsByCategoryIds(string categoryIds, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsById(long referenceId, string lang);

        Task<List<ReferenceItem>> GetReferenceItemsByCode(string referencecode, string lang);
    }
}
