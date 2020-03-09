using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.Models.User;

namespace JLSDataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JlsDbContext db;

        public UserRepository(JlsDbContext jlsDbContext)
        {
            db = jlsDbContext;
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


    }
}
