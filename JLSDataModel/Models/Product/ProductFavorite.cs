using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Product
{
    public class ProductFavorite: BaseObject
    {
        public int UserId { get; set; }

        public long ProductId { get; set; }
    }
}
