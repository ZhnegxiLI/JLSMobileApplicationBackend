﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Audit
{
    public class DeletedRecords: BaseObject
    {
        public string TableName { get; set; }
        public long? DeletedId { get; set; }
        public DateTime? DeletedOn { get; set; }
        public long? DeletedBy { get; set; }
    }
}
