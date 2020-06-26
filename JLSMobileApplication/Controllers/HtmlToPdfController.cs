using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JLSDataAccess.Interfaces;
using JLSMobileApplication.HtmlToPdf;
using Magicodes.ExporterAndImporter.Pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class HtmlToPdfController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public HtmlToPdfController(IOrderRepository order)
        {
            _orderRepository = order;
        }

        public async Task<ActionResult> ExportPdf()
        {
            /* File name */
            string fileName = "Exports/" + DateTime.Now.Second.ToString()+"_Invoice.pdf";
            /* Get template path */
            var tplPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "HtmlToPdf",
             "receipt.cshtml");
            var tpl = System.IO.File.ReadAllText(tplPath);

            /* GetOrderInfo */
            var orderInfo = await this._orderRepository.GetOrdersListByOrderId(26, "fr"); // todo change 

            var receipt = new ReceiptInfo();
            var order = orderInfo.GetType().GetProperty("OrderInfo").GetValue(orderInfo, null);
            if (order != null)
            {
                receipt.OrderId = order.Id;
                receipt.CreatedOn = order.CreatedOn;
                receipt.TotalPrice = order.TotalPrice;
            }
            var tax = orderInfo.GetType().GetProperty("TaxRate").GetValue(orderInfo, null);
 
            if (tax != null)
            {
                receipt.Tax = receipt.TotalPrice *  double.Parse(tax.GetType().GetProperty("Value").GetValue(tax, null)) * 0.01;
            }

            var productList = orderInfo.GetType().GetProperty("ProductList").GetValue(orderInfo, null);

            if (productList != null)
            {
                foreach (var item in productList)
                {
                    receipt.ProductList.Add(new ReceiptProductList() { Code = item.GetType().GetProperty("Code").GetValue(item, null),
                                                                    Label = item.GetType().GetProperty("Label").GetValue(item, null),
                                                                    Price = item.GetType().GetProperty("Price").GetValue(item, null),
                                                                    Quantity  = item.GetType().GetProperty("Quantity").GetValue(item, null),
                    });
                }
            }
      


            /* Generate pdf */
            var exporter = new PdfExporter();
            var result = await exporter.ExportByTemplate(fileName, receipt
               , tpl);

            //new ReceiptInfo
            //{
            //    Amount = 22939.43M,
            //    Grade = "2019秋",
            //    IdNo = "43062619890622xxxx",
            //    Name = "张三",
            //    Payee = "湖南心莱信息科技有限公司",
            //    PaymentMethod = "微信支付",
            //    Profession = "运动训练",
            //    Remark = "学费",
            //    TradeStatus = "已完成",
            //    TradeTime = DateTime.Now,
            //    UppercaseAmount = "贰万贰仟玖佰叁拾玖圆肆角叁分",
            //    Code = "19071800001"
            //},

            /* Download file function */
            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return File(fileBytes, "application/x-msdownload", DateTime.Now.ToString() + "_Invoice.pdf");

        }
    }
}