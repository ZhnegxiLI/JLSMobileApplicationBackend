using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.Message
{
    public class Message:BaseObject
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
