﻿using System;
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




    }
}
