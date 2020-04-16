using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using LjWebApplication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers
{
    // TODO add authorize
    [Route("api/[controller]/{action}/{id?}")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserController(IMapper mapper, IUserRepository user)
        {
            _mapper = mapper;
            _userRepository = user;
        }

        [HttpGet]
        public async Task<bool> CheckUserIsAlreadyExistAsync(string Username)
        {
            return await _userRepository.CheckUserIsAlreadyExist(Username);
        }

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
        }
        [HttpPost]
        public async Task<JsonResult> UpdateUserInfo(UpdateUserInfoCriteria criteria)
        {
            try
            {
                var result = await _userRepository.UpdateUserInfo(criteria.UserId, criteria.EntrepriseName, criteria.Siret, criteria.PhoneNumber);
                return Json(result);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
