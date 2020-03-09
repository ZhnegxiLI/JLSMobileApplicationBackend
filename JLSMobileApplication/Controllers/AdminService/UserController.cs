using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JLSDataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JLSMobileApplication.Controllers.AdminService
{
    [Route("admin/[controller]/{action}")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
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
    }
}