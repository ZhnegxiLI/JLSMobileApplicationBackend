using JLSDataModel.Models.Adress;
using JLSDataModel.Models.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.ViewModels
{
    public class OrderDetailViewModelMobile
    {
        public OrderDetailViewModelMobile()
        {
            this.OrderInfo = new OrderInfo();
            this.FacturationAdress = new Adress();
            this.ShippingAdress = new Adress();
            this.ProductList = new List<ProductDetailViewModelMobile>();
            this.Status = new ReferenceItemViewModel();
        }
        public ReferenceItemViewModel Status { get; set; }
        public OrderInfo OrderInfo { get; set; }
        public Adress FacturationAdress { get; set; }

        public List<ProductDetailViewModelMobile> ProductList { get; set; }
        public Adress ShippingAdress { get; set; }
    }
}
