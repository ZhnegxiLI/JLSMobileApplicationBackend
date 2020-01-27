using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace JLSDataModel.Models.User
{
    public class User: IdentityUser
    {
        public User()
        {

        }
        public string Siret { get; set; }
        public string EntrepriseName { get; set; }
        public string EntrepriseAdress { get; set; }
    }
}
