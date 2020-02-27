using JLSDataModel.Models.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.ViewModels
{
    public class ProductDetailViewModelMobile: Product
    {
        public ProductDetailViewModelMobile()
        {
            this.Reference = new ReferenceItemViewModel();
            this.PhotoPaths = new List<ProductPhotoPath>();
        }
        public int Quantity { get; set; }
        public ReferenceItemViewModel Reference { get; set; }

        public List<ProductPhotoPath> PhotoPaths { get; set; }

    }
}
