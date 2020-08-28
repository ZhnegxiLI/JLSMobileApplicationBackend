using JLSDataAccess;
using JLSDataModel.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLSMobileApplication.Heplers
{
    public class Initialization
    {
        public static  AppSettings _appSettings;

        public Initialization(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public static void  AddAdminUser(UserManager<User> userManager, JlsDbContext db)
        {
            var superAdminList = _appSettings.SuperAdminList.Split(';').ToList(); ; 
            foreach (var item in superAdminList)
            {
                var adminUser = db.Users.Where(p => p.UserName == item).FirstOrDefault();
                if (adminUser == null)
                {
                    User u = new User();
                    u.UserName = item;
                    u.Email = item;
                    u.EmailConfirmed = true;

                    u.Validity = true;
                    var result = userManager.CreateAsync(u, _appSettings.AdminInitialPassword).Result; 
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(u, "SuperAdmin").Wait();
                    }
                }
            }

        }
    }
}
