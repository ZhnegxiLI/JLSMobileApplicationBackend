using System;
using System.Collections.Generic;
using System.Text;

namespace JLSMobileApplication.Resources
{
    public class UserRegistrationView
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string Siret { get; set; }
        public string EntrepriseName { get; set; }

        public string FirstLineAddress { get; set; }

        public string SecondLineAddress { get; set; }

        public string  Country { get; set; }

        public string ZipCode { get; set; }
        public bool? UseSameAddress { get; set; }


    }
}
