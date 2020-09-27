using JLSDataAccess;
using JLSDataAccess.Interfaces;
using JLSMobileApplication.Heplers;
using JLSMobileApplication.HtmlToPdf;
using Magicodes.ExporterAndImporter.Pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
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
        private readonly AppSettings _appSettings;
        private readonly JlsDbContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;

        public ExportService(IOptions<AppSettings> appSettings, JlsDbContext context, IHttpContextAccessor httpContextAccessor, IOrderRepository order)
        {
            _appSettings = appSettings.Value;
            db = context;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = order;
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
                            if (value is IList && value.GetType().IsGenericType)
                            {
                                valueFormatted = "";
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

        public async Task<string> ExportPdf(long OrderId, string Lang)
        {
            try
            {
                /* File name */
                string fileName = "Exports/" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Invoice.pdf";
                /* Get template path */
                var tplPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "HtmlToPdf",
                 "receipt.cshtml");
                var tpl = System.IO.File.ReadAllText(tplPath);

                /* GetOrderInfo todo change */
                var orderInfo = await this._orderRepository.GetOrdersListByOrderId(OrderId, Lang); // todo change 

                /* Get order basic info */
                var receipt = new ReceiptInfo();

                var order = orderInfo.GetType().GetProperty("OrderInfo").GetValue(orderInfo, null);
                if (order != null)
                {
                    receipt.OrderId = order.Id;
                    receipt.CreatedOn = order.CreatedOn;
                    receipt.TotalPrice = Convert.ToDouble(order.TotalPrice).ToString("0.00");
                }
                var customer = orderInfo.GetType().GetProperty("CustomerInfo").GetValue(orderInfo, null);
                if (customer != null)
                {
                    receipt.Username = customer.Email;
                    receipt.PhoneNumber = customer.PhoneNumber;
                    receipt.Entreprise = customer.EntrepriseName;
                    receipt.Siret = customer.Siret;
                }

                /* Get order tax info */
                var tax = orderInfo.GetType().GetProperty("TaxRate").GetValue(orderInfo, null);

                receipt.TaxRate = (string)tax.GetType().GetProperty("Value").GetValue(tax, null);

                /* Get order product list info */
                var productList = orderInfo.GetType().GetProperty("ProductList").GetValue(orderInfo, null);
                if (productList != null)
                {
                    foreach (var item in productList)
                    {
                        receipt.ProductList.Add(new ReceiptProductList()
                        {
                            Code = item.GetType().GetProperty("Code").GetValue(item, null),
                            Colissage = item.GetType().GetProperty("QuantityPerBox").GetValue(item, null),
                            PhotoPath = _appSettings.WebSiteUrl + item.GetType().GetProperty("DefaultPhotoPath").GetValue(item, null),
                            Label = item.GetType().GetProperty("Label").GetValue(item, null),
                            Price = Convert.ToDouble(item.GetType().GetProperty("Price").GetValue(item, null)).ToString("0.00"),
                            Quantity = item.GetType().GetProperty("Quantity").GetValue(item, null),
                        });

                        receipt.TotalPriceWithoutTax = Convert.ToDouble(receipt.TotalPriceWithoutTax + item.GetType().GetProperty("QuantityPerBox").GetValue(item, null) * Convert.ToDouble(item.GetType().GetProperty("Price").GetValue(item, null)) * item.GetType().GetProperty("Quantity").GetValue(item, null)).ToString("0.00");
                    }

                    if (tax != null)
                    {
                        receipt.Tax = Convert.ToDouble((Convert.ToDouble(receipt.TotalPriceWithoutTax) * double.Parse(tax.GetType().GetProperty("Value").GetValue(tax, null)) * 0.01)).ToString("0.00");
                    }
                }

                /* Get facturation address */
                var facturationAddress = orderInfo.GetType().GetProperty("ShippingAdress").GetValue(orderInfo, null);
                if (facturationAddress != null)
                {
                    receipt.FacturationAddress = facturationAddress;
                }
                /* Get shipping address */
                var shippingAddress = orderInfo.GetType().GetProperty("FacturationAdress").GetValue(orderInfo, null);
                if (shippingAddress != null)
                {
                    receipt.ShipmentAddress = shippingAddress;
                }

                /* Generate pdf */
                var exporter = new PdfExporter();
                var result = await exporter.ExportByTemplate(fileName, receipt
                   , tpl);

                return fileName;
            }
            catch(Exception e)
            {
                throw e;
            }
         
        }
    }
    }
