using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Resources
{
    public class ReferenceItemViewModel
    {
        public ReferenceItemViewModel()
        {
            this.ReferenceParent = new ReferenceItemViewModel();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
        public string Value { get; set; }
        public int? Order { get; set; }
        public long ReferenceCategoryId { get; set; }
        public string ReferenceCategoryLabel { get; set; }
        public string ReferenceCategoryLongLabel { get; set; }
        public string Label { get; set; }

        public ReferenceItemViewModel ReferenceParent { get; set; }
        public bool? Validity { get; set; }
    }
}
