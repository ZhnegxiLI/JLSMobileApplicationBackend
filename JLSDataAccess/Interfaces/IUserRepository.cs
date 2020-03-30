using JLSDataModel.Models.Adress;
using JLSDataModel.Models.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JLSDataAccess.Interfaces
{
    public interface IUserRepository
    {

        Task<bool> CheckUserIsAlreadyExist(string Username);

        Task<Adress> GetUserFacturationAdress(int userId);

        Task<List<Adress>> GetUserShippingAdress(int userId);

        Task<List<User>> GetUserListByRole(List<string> Roles);


        Task<List<dynamic>> AdvancedUserSearch(int? UserType, bool? Validity, string Username);

        Task<List<dynamic>> GetUserRoleList();
        Task<dynamic> GetUserById(int UserId);

        Task<dynamic> CreateOrUpdateUser(int UserId, string Email, string Password, int RoleId, bool Validity);
    }
}
