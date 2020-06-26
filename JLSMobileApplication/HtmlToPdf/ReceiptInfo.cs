using JLSDataModel.Models.Adress;
using JLSDataModel.Models.Order;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.HtmlToPdf
{
    [PdfExporter(Name = "Facture")]
    public class ReceiptInfo
    {
        public ReceiptInfo()
        {
            this.ShipmentAddress = new Adress();
            this.FacturationAddress = new Adress();
            this.ProductList = new List<ReceiptProductList>();
        }
        public long OrderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Username { get; set; }
        public string Entreprise { get; set; }
        public string PhoneNumber { get; set; }

        public double Tax { get; set; }
        public double TotalPrice { get; set; }

        public Adress ShipmentAddress { get; set; }
        public Adress FacturationAddress { get; set; }

        public List<ReceiptProductList> ProductList { get; set; }
    }

    public class ReceiptProductList
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
