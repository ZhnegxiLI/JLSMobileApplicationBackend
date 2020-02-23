using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.ViewModels
{
    public class OrderListViewModelMobile
    {
        public OrderListViewModelMobile()
        {
            this.ShippingAdress = new Adress();
        }
        public long Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public float? TotalPrice { get; set; }
        public long ShippingAdressId { get; set; }
        public Adress ShippingAdress { get; set; }
    }
}
