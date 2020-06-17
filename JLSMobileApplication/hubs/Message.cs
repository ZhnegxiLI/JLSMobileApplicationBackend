using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.hubs
{
    public class Message
    {
        public int clientuniqueid { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }

        public int? fromUser { get; set; }
        public int? toUser { get; set; }
    }
}
