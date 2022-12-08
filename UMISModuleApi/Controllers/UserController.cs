using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMISModuleAPI.Models;
using UMISModuleAPI.Configuration;
using UMISModuleApi.Entities;
using UMISModuleAPI.Entities;
using UMISModuleAPI.Services;
using Microsoft.Extensions.Options;
using UMISModuleApi.dto.response;
using UMISModuleAPI.Entities;
using Newtonsoft.Json;
using Dapper;
using Npgsql;
using System.Data;
using System.Collections.Generic;

namespace UMISModuleAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private string cmd,cmd2,cmd3,cmd4;
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

        [HttpGet("getUsers")]
        public IActionResult getUsers()
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

        [HttpPost("saveUser")]
        public IActionResult saveUser(UserCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var userName = "";
                var newUserID = 0;
                var newUserRoleID = 0;


                List<UserCreation> appMenuUser = new List<UserCreation>();
                cmd = "select \"loginName\" from users where \"loginName\"='" + obj.loginName + "'";
                appMenuUser = (List<UserCreation>)dapperQuery.Qry<UserCreation>(cmd, _dbCon);

                List<UserCreation> appMenuUserID = new List<UserCreation>();
                cmd3 = "select \"userID\" from users ORDER BY \"userID\" DESC LIMIT 1";
                appMenuUserID = (List<UserCreation>)dapperQuery.Qry<UserCreation>(cmd3, _dbCon);
                if(appMenuUserID.Count == 0)
                    {
                        newUserID = 1;    
                    }else{
                        newUserID = appMenuUserID[0].userID+1;
                    }

                List<UserRoleCreation> appMenuUserRoleID = new List<UserRoleCreation>();
                cmd3 = "select \"userRoleId\" from user_roles ORDER BY \"userRoleId\" DESC LIMIT 1";
                appMenuUserRoleID = (List<UserRoleCreation>)dapperQuery.Qry<UserRoleCreation>(cmd3, _dbCon);
                if(appMenuUserRoleID.Count == 0)
                    {
                        newUserRoleID = 1;    
                    }else{
                        newUserRoleID = appMenuUserRoleID[0].userRoleId+1;
                    }

                
                if (appMenuUser.Count > 0)
                        userName = appMenuUser[0].loginName;

                if(userName=="")
                {
                    cmd2 = "insert into public.\"users\" (\"userID\",\"empName\", \"loginName\", \"Password\", \"outletid\", \"createdOn\", \"isDeleted\") values ("+newUserID+",'" + obj.empName + "','" + obj.loginName + "','" + obj.Password + "'," + obj.outletid + ", '" + curDate + "', 0)";
                    //cmd4 = "insert into public.\"user_roles\" (\"userRoleId\",\"roleId\", \"userId\",\"createdOn\", \"isDeleted\") VALUES ('"+newRoleID+"',1,'" +newUserID+ "','" + curDate + "', 0)";
                }
                else
                {
                    found=true;
                }
                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd2);
                    }
                    
                }

                if (rowAffected > 0 )
                {
                    cmd4 = "insert into public.\"user_roles\" (\"userRoleId\",\"roleId\", \"userId\",\"createdOn\", \"isDeleted\") VALUES ("+newUserRoleID+"," + obj.roleId + "," + newUserID + ",'" + curDate + "', 0)";
                    
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd4);
                    }

                    if (rowAffected > 0 && rowAffected2 > 0)
                    {
                        response = "Success";
                       return Ok(new { message = response });
                    }
                    else
                    {
                        response = "Server Issue";
                       return Ok(new { message = response });
                    }
                    

                }
                else
                {
                    if (found == true)
                    {
                        response = "Login name already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                return BadRequest(new { message = response });

                }

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