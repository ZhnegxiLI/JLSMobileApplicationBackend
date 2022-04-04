using JLSDataModel.Models.Adress;
using JLSDataModel.Models.Order;
using System.Collections.Generic;

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
            this.StatusInfo = new object();
        }
        public ReferenceItemViewModel Status { get; set; }
        public OrderInfo OrderInfo { get; set; }
        public Adress FacturationAdress { get; set; }

        public dynamic StatusInfo { get; set; }

        public List<ProductDetailViewModelMobile> ProductList { get; set; }
        public Adress ShippingAdress { get; set; }
    }
}
