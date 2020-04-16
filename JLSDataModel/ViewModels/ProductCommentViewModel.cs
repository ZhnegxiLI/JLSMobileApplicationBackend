using JLSDataModel.Models;
using JLSDataModel.Models.Product;
using JLSDataModel.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.ViewModels
{
    public class ProductCommentViewModel
    {
        public ProductCommentViewModel()
        {
            this.ProductComment = new ProductComment();
            this.User = new User();
        }
        public ProductComment ProductComment { get; set; }
        public User User { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? UserId { get; set; }

        public string Label { get; set; }
        public string PhotoPath { get; set; }
    }
}
