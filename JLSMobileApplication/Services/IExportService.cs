using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public interface IExportService
    {
        MemoryStream ExportExcel (List<dynamic> List, string ExportName);
    }
}
