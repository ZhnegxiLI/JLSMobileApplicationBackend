﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models
{
    public class CustomerAdvice:BaseObject
    {

        public long? OrderId { get; set; }
        public string Title { get; set; }

        public string Body { get; set; }

        public int UserId { get; set; }
    }
}