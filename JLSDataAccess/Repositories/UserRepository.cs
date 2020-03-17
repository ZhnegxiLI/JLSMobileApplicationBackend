using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.Models.User;
using Microsoft.AspNetCore.Identity;

namespace JLSDataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JlsDbContext db;
        private readonly UserManager<User> _userManager;

        public UserRepository(JlsDbContext jlsDbContext, UserManager<User> userManager)
        {
            db = jlsDbContext;
            _userManager = userManager;
        }
        public async Task<Adress> GetUserFacturationAdress(int userId)
        {
            var result = (from a in db.Adress
                          join u in db.Users on a.Id equals u.FacturationAdressId
                          select a);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<List<Adress>> GetUserShippingAdress(int userId)
        {
            var result = (from a in db.Adress
                          join ua in db.UserShippingAdress on a.Id equals ua.ShippingAdressId
                          join u in db.Users on ua.UserId equals u.Id
                          where ua.UserId == userId
                          select a);
            return await result.ToListAsync();
        }

        public async Task<List<User>> GetUserListByRole(List<string> Roles)
        {
            var result = await (from u in db.Users
                          join userRole in db.UserRoles on u.Id equals userRole.UserId
                          join role in db.Roles on userRole.RoleId equals role.Id
                          select u).ToListAsync<User>();
            return  result;
        }

        public async Task<List<dynamic>> AdvancedUserSearch(int? UserType, bool? Validity, string Username)
        {
            var result = await (from u in db.Users
                          join ur in db.UserRoles on u.Id equals ur.UserId
                          join r in db.Roles on ur.RoleId equals r.Id
                          where (UserType == null || ur.RoleId == UserType)
                          && (Validity == null || u.Validity == Validity)
                          && (Username == "" || u.UserName == Username)
                          select new
                          {
                              Id = u.Id,
                              Username = u.UserName,
                              EntrepriseName = u.EntrepriseName,
                              Validity = u.Validity,
                              Telephone = u.PhoneNumber,
                              UserRoleId = r.Id,
                              UserRoleName = r.Name,
                              CreatedOn = u.CreatedOn,
                              UpdatedOn = u.UpdatedOn
                          }).ToListAsync<dynamic>();

            return result;
        }

        public async Task<List<dynamic>> GetUserRoleList()
        {
            var result = await (from r in db.Roles
                                select new
                                {
                                   Id = r.Id,
                                   Name = r.Name
                                }).ToListAsync<dynamic>();
            return result;
        }

        public async Task<dynamic> GetUserById(int UserId)
        {
            var result = await (from u in db.Users
                                join ur in db.UserRoles on u.Id equals ur.UserId
                                join r in db.Roles on ur.RoleId equals r.Id
                                where u.Id == UserId
                                select new
                                {
                                    Id = u.Id,
                                    RoleId = r.Id,
                                    Email = u.Email,
                                    Validity = u.Validity
                                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<int> CreateOrUpdateUser(int UserId, string Email , string Password, int RoleId, bool Validity)
        {
            User UserToCreateOrUpdate = null;
            if (UserId==0)
            {
                UserToCreateOrUpdate = new User();
                UserToCreateOrUpdate.CreatedOn = DateTime.Now;
                UserToCreateOrUpdate.Email = Email;
            }
            else
            {
                UserToCreateOrUpdate = await db.Users.FindAsync(UserId);
            }
            UserToCreateOrUpdate.Validity = Validity;
       
            if(UserId == 0)
            {
                await _userManager.CreateAsync(UserToCreateOrUpdate, Password);
            }
            else
            {
                await _userManager.UpdateAsync(UserToCreateOrUpdate);
                var token = await _userManager.GeneratePasswordResetTokenAsync(UserToCreateOrUpdate);
                await _userManager.ResetPasswordAsync(UserToCreateOrUpdate, token, Password);
            }

            //UserRole by RoleId
            var role = await db.Roles.Where(r=>r.Id == RoleId).FirstOrDefaultAsync();
            //Remove all role for user 
            var userRoleToRemove = await db.UserRoles.Where(p => p.UserId == UserToCreateOrUpdate.Id).ToListAsync();

            db.UserRoles.RemoveRange(userRoleToRemove);
            await db.SaveChangesAsync();
            await _userManager.AddToRoleAsync(UserToCreateOrUpdate, role.Name);

            return UserToCreateOrUpdate.Id;
        }


    }
}
