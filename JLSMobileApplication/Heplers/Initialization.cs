using JLSDataAccess;
using JLSDataModel.Models.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Heplers
{
    public class Initialization
    {
 

        public Initialization()
        {
        }

        public static void  AddAdminUser(UserManager<User> userManager, JlsDbContext db)
        {
            var adminUser = db.Users.Where(p => p.UserName == "Admin@jls.com").FirstOrDefault();
            if (adminUser == null)
            {
                User u = new User();
                u.UserName = "Admin@jls.com";
                u.Email = "Admin@jls.com";
                u.EmailConfirmed = true;

                u.Validity = true;
                var result = userManager.CreateAsync(u,"12345678").Result; // only for test use
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(u, "SuperAdmin").Wait();
                }
            
            }

        }
    }
}
