using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JLSMobileApplication.Controllers.AdminService
{
    [Authorize]
    [Route("admin/[controller]/{action}")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly JlsDbContext db;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, IMapper mapper, JlsDbContext jlsDbContext, UserManager<User> userManager)
        {
            db = jlsDbContext;
            _userManager = userManager;
            this._userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<JsonResult> GetUserListByRole([FromBody]List<string> Roles) // todo change take other param
        {
            try
            {
                var result = await _userRepository.GetUserListByRole(Roles);

                return Json(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public class AdvancedUserSearchCriteria{
            public int? UserType { get; set; }
            public bool? Validity { get; set; }
            public string Username { get; set; }
            public int begin { get; set; }
            public int step { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult> AdvancedUserSearch(AdvancedUserSearchCriteria criteria)
        {
            try
            {
                var result = await _userRepository.AdvancedUserSearch(criteria.UserType, criteria.Validity, criteria.Username);
                var totalCount = result.Count();
                var list = result.Skip(criteria.begin * criteria.step).Take(criteria.step);
                return Json(new {
                    UserList = list,
                    TotalCount = totalCount
                });
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUserRoleList()
        {
            try
            {
                var result = await _userRepository.GetUserRoleList();
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUserById(int UserId)
        {
            try
            {
                var result = await _userRepository.GetUserById(UserId);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public class CreateOrUpdateUserCriteria
        {
            public int? CreatedOrUpdatedBy { get; set; }
            public int UserId { get; set; }
            public string Email { get; set; }

            public string Password { get; set; }
            public bool Validity { get; set; }
            public int RoleId { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult> CreateOrUpdateUser([FromBody]CreateOrUpdateUserCriteria criteria)
        {
            try
            {
                //var result = await _userRepository.CreateOrUpdateUser(criteria.UserId, criteria.Email, criteria.Password, criteria.RoleId, criteria.Validity);
                var role = await db.Roles.Where(r => r.Id == criteria.RoleId).FirstOrDefaultAsync();

                User UserToCreateOrUpdate = null;
                if (criteria.UserId == 0)
                {
                    UserToCreateOrUpdate = new User();
                    UserToCreateOrUpdate.CreatedOn = DateTime.Now;
                    UserToCreateOrUpdate.Email = criteria.Email;
                    UserToCreateOrUpdate.UserName = criteria.Email;
                    if (role.Name == "Admin")
                    {
                        UserToCreateOrUpdate.EmailConfirmed = true;
                    }
                }
                else
                {
                    UserToCreateOrUpdate = await db.Users.FindAsync(criteria.UserId);
                }
                UserToCreateOrUpdate.Validity = criteria.Validity;
                if (criteria.Validity == false)
                {
                    var refreshToken = await db.TokenModel.Where(p => p.UserId == criteria.UserId).ToListAsync();
                    db.TokenModel.RemoveRange(refreshToken);
                    await db.SaveChangesAsync();
                }
                if (criteria.UserId == 0)
                {
                    var result = await _userManager.CreateAsync(UserToCreateOrUpdate, criteria.Password);
                    if (result.Succeeded == false)
                    {
                        return Json(result);
                    }
                }
                else
                {
                    await _userManager.UpdateAsync(UserToCreateOrUpdate);
                    if (criteria.Password != "")
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(UserToCreateOrUpdate);
                        await _userManager.ResetPasswordAsync(UserToCreateOrUpdate, token, criteria.Password);
                    }
                }


                //Remove all role for user 
                var userRoleToRemove = await db.UserRoles.Where(p => p.UserId == UserToCreateOrUpdate.Id).ToListAsync();

                db.UserRoles.RemoveRange(userRoleToRemove);
                await db.SaveChangesAsync();
                await _userManager.AddToRoleAsync(UserToCreateOrUpdate, role.Name);

                return Json(UserToCreateOrUpdate.Id);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}