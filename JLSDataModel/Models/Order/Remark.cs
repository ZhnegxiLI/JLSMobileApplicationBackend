﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Order
{
    public class Remark: BaseObject
    {
        public int? UserId { get; set; }
        public string Text { get; set; }
    }
}
