using System;
using System.Collections.Generic;
using System.Text;
using DNVGL.Authorization.UserManagement.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNVGL.Authorization.UserManagement.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUser _userRepository;

        public UsersController(IUser userRepository)
            => _userRepository = userRepository;
    }
}
