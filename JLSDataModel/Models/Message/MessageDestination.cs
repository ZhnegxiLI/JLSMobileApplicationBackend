using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Message
{
    public class MessageDestination:BaseObject
    {
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
    }
}
