using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Message
{
    public class Message:BaseObject
    {
        public long? OrderId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
