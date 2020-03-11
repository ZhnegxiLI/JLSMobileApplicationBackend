using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Order
{
    public class OrderInfo:BaseObject
    {
        public string OrderReferenceCode { get; set; }

        public string PaymentInfo { get; set; }

        public string ClientRemark { get; set; }

        public string AdminRemark { get; set; }

        public float? TotalPrice { get; set; }

        public float? TaxRate { get; set; }

        public string OrderType { get; set; } // client or internal
        // Foreign key 
        public int UserId { get; set; }
        public User.User User { get; set; }

        public long StatusReferenceItemId { get; set; }
        public ReferenceItem StatusReferenceItem { get; set; }

        public long ShippingAdressId { get; set; }
        public Adress.Adress ShippingAdress { get; set; }

        public long FacturationAdressId { get; set; }
        public Adress.Adress FacturationAdress { get; set; }
    }
}
