using JLSDataAccess;
using JLSMobileApplication.Heplers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JLSMobileApplication.Services
{
    public class ExportService : IExportService
        {
        /// <summary>
        /// 只用于测试目的请勿在生产环境中放入此代码
        /// </summary>
        private readonly AppSettings _appSettings;
        private readonly JlsDbContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExportService(IOptions<AppSettings> appSettings, JlsDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings.Value;
            db = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public MemoryStream ExportExcel(List<dynamic> List, string ExportName)
        {
            if (List != null && List.Count()>0)
            {
                /*Step0: Create file */ 
                var newFile = "Exports/" + ExportName + ".xls";
                if (System.IO.File.Exists(newFile))
                {
                    System.IO.File.Delete(newFile);
                }

                using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                {

                    /* Step1: Get export model */
                    var ExportConfiguration = db.ExportConfiguration.Where(p => p.ExportName == ExportName).FirstOrDefault();
                    List<ExportModel> ExportConfigurationModel = null;
                    if (ExportConfiguration!=null && ExportConfiguration.ExportModel!=null && ExportConfiguration.ExportModel!="")
                    {

                        ExportConfigurationModel = JsonConvert.DeserializeObject<List<ExportModel>>(ExportConfiguration.ExportModel);
                    }
                   /* Step2: Calcul the targeted Column */

                    // Get columns title from first object in the list
                    var columns = List[0].GetType().GetProperties();
                    var targetColumns = new List<string>();
                    foreach (var item in columns)
                    {
                        if (ExportConfigurationModel != null)
                        {
                            var temp = ExportConfigurationModel.Where(p => p.Name == item.Name).FirstOrDefault();
                            if (temp != null)
                            {
                                targetColumns.Add(temp.Name);
                            }
                        }
                        else
                        {
                            targetColumns.Add(item.Name);
                        }
                    }
                    /*Step3: Create Excel flow */
                    IWorkbook workbook = new XSSFWorkbook();
                    var sheet = workbook.CreateSheet(ExportName);
                    var header = sheet.CreateRow(0);
                    /*Step4: Add headers*/
                    int columnsCounter = 0;
                    foreach (var item in targetColumns)
                    {
                        if (ExportConfigurationModel!=null)
                        {
                            var temp = ExportConfigurationModel.Where(p => p.Name == item).Select(p => p.DisplayName).FirstOrDefault();
                            header.CreateCell(columnsCounter).SetCellValue(temp != null ? temp : item);
                        }
                        else
                        {
                            header.CreateCell(columnsCounter).SetCellValue(item);
                        }
                        columnsCounter++;
                    }
                    /*Step5: Add body */
                    var rowIndex = 1;
                    foreach (var item in List)
                    {
                        var datarow = sheet.CreateRow(rowIndex);

                        columnsCounter = 0;
                        foreach (var column in targetColumns)
                        {
                            string valueFormatted = null;
                            var value = item.GetType().GetProperty(column).GetValue(item,null);
                            if (value != null)
                            {
                                var valueType = value.GetType();
                               
                                if (valueType.Name=="Boolean")
                                {
                                    valueFormatted = value ? "OUI" : "NON";
                                }
                                else if (valueType.Name == "DateTime")
                                {
                                    valueFormatted = value.ToString();
                                }
                                if (column.Contains("Path"))
                                {
                                    value = _httpContextAccessor.HttpContext.Request.Host  + _httpContextAccessor.HttpContext.Request.PathBase + "/"+ value;
                                }
                                if (column.Contains("Price"))
                                {
                                    value = value + "€";
                                }
                            }
                            else
                            {
                                value = "";
                            }

                            datarow.CreateCell(columnsCounter).SetCellValue(valueFormatted!=null? valueFormatted: value);
                            columnsCounter++;
                        }

                        rowIndex++;
                    }
                    workbook.Write(fs);
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(newFile, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                return memory;
            }

            return null;
        }
    }
    }
