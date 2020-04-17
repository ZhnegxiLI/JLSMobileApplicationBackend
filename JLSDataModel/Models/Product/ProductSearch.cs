using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Product
{
    public class ProductSearchCount: BaseObject
    {
        public int? UserId { get; set; }
        public string SearchText { get; set; }

        public string SearchCondition { get; set; } // json
        public int? Count { get; set; }
    }
}
