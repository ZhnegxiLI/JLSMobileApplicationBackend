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
    public class AdressRepository: IAdressRepository
    {
        private readonly JlsDbContext db;

        public AdressRepository(JlsDbContext context)
        {
            db = context;
        }

        public async Task<long> CreateOrUpdateAdress(Adress adress)
        {
            if (adress.Id>0)
            {
                db.Update(adress);
            }
            else
            {
               await db.AddAsync(adress);
            }
            await db.SaveChangesAsync();
            return adress.Id;
        }

        public async Task<long> CreateUserShippingAdress(long adressId,int userId)
        {
            var userShippingAdress = await db.UserShippingAdress.Where(p => p.ShippingAdressId == adressId && p.UserId == userId).FirstOrDefaultAsync();
            if (userShippingAdress == null)
            {
                var userShippingAdressToCreate = new UserShippingAdress();
                userShippingAdressToCreate.UserId = userId;
                userShippingAdressToCreate.ShippingAdressId = adressId;
                userShippingAdressToCreate.CreatedOn = DateTime.Now;
                await db.AddAsync(userShippingAdressToCreate);
                await db.SaveChangesAsync();
                return userShippingAdressToCreate.Id;
            }
            else
            {
                return userShippingAdress.Id;
            }
        }

        public async Task<long> CreateFacturationAdress(long adressId, int userId)
        {
            var User = await db.Users.Where(p =>  p.Id == userId).FirstOrDefaultAsync();
            if (User!=null)
            {
                if (User.FacturationAdressId != adressId)
                {
                    User.FacturationAdressId = adressId;
                    db.Update(User);
                    await db.SaveChangesAsync();

                }
                return User.FacturationAdressId;
            }
            else
            {
                return 0;
            }
           
        }

        public async Task<Adress> GetAdressByIdAsync(long Id)
        {
            var result = await db.Adress.FindAsync(Id);
            return result;
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
                          orderby a.IsDefaultAdress == true
                          select a);
            return await result.ToListAsync();
        }

        public async Task<Adress> GetUserDefaultShippingAdress(int userId)
        {
            var result = (from a in db.Adress
                          join ua in db.UserShippingAdress on a.Id equals ua.ShippingAdressId
                          join u in db.Users on ua.UserId equals u.Id
                          where ua.UserId == userId
                          orderby a.IsDefaultAdress == true
                          select a);
            return await result.FirstOrDefaultAsync();
        }
    }
}
