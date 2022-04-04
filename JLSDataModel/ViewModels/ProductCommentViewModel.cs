using JLSDataModel.Models.Product;
using JLSDataModel.Models.User;
using System;

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
