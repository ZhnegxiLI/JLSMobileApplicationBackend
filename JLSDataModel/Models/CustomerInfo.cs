using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models
{
    public class CustomerInfo: BaseObject
    {
        public string Email { get; set; }
        public string EntrepriseName { get; set; }

        public string PhoneNumber { get; set; }

        public string Siret { get; set; }

        public int? UserId { get; set; }
    }
}
