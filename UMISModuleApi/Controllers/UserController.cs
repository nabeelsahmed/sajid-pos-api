using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMISModuleAPI.Models;
using UMISModuleAPI.Services;
using UMISModuleAPI.Entities;
using Microsoft.Extensions.Options;
using UMISModuleAPI.Configuration;
using UMISModuleApi.dto.response;
using UMISModuleAPI.Entities;

namespace UMISModuleAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private string cmd;
        private readonly IOptions<conStr> _dbCon;

        public UserController(IUserService userService, IOptions<conStr> dbCon)
        {
            _userService = userService;
            _dbCon = dbCon;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            if (response == null)
                return BadRequest(new { message = "user name or password is incorrect" });

            return Ok(response);
        }

        [HttpGet("getPin")]
        public IActionResult getPin(int pin, int userID)
        {
            try
            {
                cmd = "SELECT \"userID\" FROM users WHERE \"isDeleted\" = 0 And \"userID\" = '" + userID + "' and pincode = '" + pin + "';";
                var appMenu = dapperQuery.Qry<User>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getTestData")]
        public IActionResult getTestData()
        {
            return Ok("OK");
        }
    }
}