using JLSDataModel.Models.Adress;
using Magicodes.ExporterAndImporter.Pdf;
using System;
using System.Collections.Generic;

namespace JLSMobileApplication.HtmlToPdf
{
    [PdfExporter(Name = "Commande")]
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
        public string Siret { get; set; }
        public string ClientRemark { get; set; }

        public float Tax { get; set; }

        public float TotalPrice { get; set; }

        public float TaxRate { get; set; }

        public float TotalPriceWithoutTax { get; set; }

        public Adress ShipmentAddress { get; set; }
        public Adress FacturationAddress { get; set; }

        public List<ReceiptProductList> ProductList { get; set; }
    }

    public class ReceiptProductList
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public int Quantity { get; set; }
        public int? QuantityPerParcel { get; set; }
        public int Colissage { get; set; }
        public float Price { get; set; }
        public string PhotoPath { get; set; }
        public bool? IsModifiedPriceOrBox { get; set; }
    }
}