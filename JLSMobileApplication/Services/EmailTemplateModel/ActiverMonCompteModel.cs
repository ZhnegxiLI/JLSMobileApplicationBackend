﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Services.EmailTemplateModel
{
    public class ActiverMonCompteModel
    {
        public string Username { get; set; }
        public string ConfirmationLink { get; set; }

        public string Entreprise { get; set; }
        public string Phone { get; set; }
    }
}
