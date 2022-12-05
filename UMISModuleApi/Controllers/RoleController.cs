using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UMISModuleAPI.Configuration;
using UMISModuleApi.Entities;
using UMISModuleAPI.Services;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UMISModuleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private IUserService _userService;
        private string cmd,cmd2,cmd3,cmd4,cmd5,cmd6;
        private readonly IOptions<conStr> _dbCon;

        public RoleController(IUserService userService, IOptions<conStr> dbCon)
        {
            _userService = userService;
            _dbCon = dbCon;
        }


        [HttpPost("saveRole")]
        public IActionResult saveRole(RoleCreation obj)
        {
            try{
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;
                
                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                var found = false;
                var roleTitle = "";
                var newRoleID = 0;
                var newRoleDetailID = 0;

                List<RoleCreation> appMenuUser = new List<RoleCreation>();
                cmd = "select \"roleTitle\" from roles where \"roleTitle\"='" + obj.roleTitle + "'";
                appMenuUser = (List<RoleCreation>)dapperQuery.Qry<RoleCreation>(cmd, _dbCon);

                List<RoleCreation> appMenuRoleID = new List<RoleCreation>();
                cmd3 = "select \"roleID\" from roles ORDER BY \"roleID\" DESC LIMIT 1";
                appMenuRoleID = (List<RoleCreation>)dapperQuery.Qry<RoleCreation>(cmd3, _dbCon);

                
                
                if(appMenuRoleID.Count == 0)
                    {
                        newRoleID = 1;    
                    }else{
                        newRoleID = appMenuRoleID[0].roleID+1;
                    }

                if (appMenuUser.Count > 0)
                        roleTitle = appMenuUser[0].roleTitle;

                
                if(roleTitle == "")
                {   

                    cmd2 = "insert into public.\"roles\" (\"roleID\", \"roleTitle\", \"roleDescription\", \"createdBy\", \"createdOn\", \"isDeleted\") values (" + newRoleID + ", '" + obj.roleTitle + "', '" + obj.roleDescription + "', " + obj.userID + ", '" + curDate + "', 0)";
                    
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
                    var invObject = JsonConvert.DeserializeObject<List<RoleDetailCreation>>(obj.json);

                    foreach (var item in invObject)
                    {
                        List<RoleDetailCreation> appMenuRoleDetailID = new List<RoleDetailCreation>();
                        cmd4 = "select \"roleDetailId\" from roles_detail ORDER BY \"roleDetailId\" DESC LIMIT 1";
                        appMenuRoleDetailID = (List<RoleDetailCreation>)dapperQuery.Qry<RoleDetailCreation>(cmd4, _dbCon); 

                        if(appMenuRoleDetailID.Count == 0)
                        {
                            newRoleDetailID = 1;    
                        }else{
                            newRoleDetailID = appMenuRoleDetailID[0].roleDetailID+1;
                        }

                        cmd6 = "insert into public.\"roles_detail\" (\"roleDetailId\", \"menuId\", \"roleId\", \"createdOn\", \"createdBy\", \"isDeleted\", \"read\", \"write\", \"delete\") values (" + newRoleDetailID + ", '" + item.menuId + "', " + newRoleID + ", '" + curDate + "', " + obj.userID + ", 0," + item.read + "," + item.write + "," + item.delete + ")";
                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd6);
                        }

                     
                    }
                    

                }
                if(rowAffected > 0 && rowAffected2 > 0 )
                {
                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    if (found == true)
                    {
                        response = "Phone number already exist";
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
        
    }
}