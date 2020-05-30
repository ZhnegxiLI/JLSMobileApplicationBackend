using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using JLSDataModel.Models.User;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    // TODO add authorize
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserController(UserManager<User> userManager,IMapper mapper, IUserRepository user)
        {
            _mapper = mapper;
            _userRepository = user;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<bool> CheckUserIsAlreadyExistAsync(string Username)
        {
            return await _userRepository.CheckUserIsAlreadyExist(Username);
        }
        [HttpGet]
        public async Task<long> InsertSubscribeEmail(string Email)
        {
            return await _userRepository.InsertSubscribeEmail(Email);
        }

        /* Auth zoom */
        [Authorize]
        [HttpGet]
        public async Task<JsonResult> GetUserById(int UserId)
        {
            try
            {
                return Json(await _userRepository.GetUserById(UserId));
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public class UpdateUserInfoCriteria
        {
            public int UserId { get; set; }

            public string EntrepriseName { get; set; }

            public string Siret { get; set; }

            public string PhoneNumber { get; set; }

            public long? DefaultShippingAddressId { get; set; }
        }
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> UpdateUserInfo(UpdateUserInfoCriteria criteria)
        {
            try
            {
                var result = await _userRepository.UpdateUserInfo(criteria.UserId, criteria.EntrepriseName, criteria.Siret, criteria.PhoneNumber, criteria.DefaultShippingAddressId);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public  class UpdatePasswordCriteria
        {
            public int UserId { get; set; }
            public string PreviousPassword { get; set; }

            public string NewPassword { get; set; }
        }
        [Authorize]
        [HttpPost]
        public async Task<int> UpdatePassword(UpdatePasswordCriteria criteria)
        {
            try
            {
                User user = _userManager.FindByIdAsync(criteria.UserId.ToString()).Result;
                if (user != null && await _userManager.CheckPasswordAsync(user, criteria.PreviousPassword))
                {

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var result = await _userManager.ResetPasswordAsync(user, token, criteria.NewPassword);
                    if (result.Succeeded)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                  
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
