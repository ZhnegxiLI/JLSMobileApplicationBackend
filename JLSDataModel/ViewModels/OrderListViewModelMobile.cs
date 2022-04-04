﻿using JLSDataModel.Models.Adress;
using System;

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
        public string StatusCode { get; set; }
        public string StatusLabel { get; set; }

        public int? NumberOfProduct { get; set; }
    }
}