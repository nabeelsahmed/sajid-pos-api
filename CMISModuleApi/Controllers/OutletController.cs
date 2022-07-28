using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMISModuleApi.Entities;
using CMISModuleApi.dto.request;
using CMISModuleApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMISModuleApi.Services;
using Microsoft.Extensions.Options;
using Dapper;
using System.Data;
using Npgsql;

namespace CMISModuleApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OutletController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public OutletController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getOutlet")]
        public IActionResult getOutlet()
        {
            try
            {
                cmd = "select * from public.\"outlet\" where \"isDeleted\"::int = 0 ";
                var appMenu = dapperQuery.Qry<outlet>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }
        [HttpPost("saveOutlet")]
        public IActionResult saveOutlet(outletCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");
                var found = false;
                var outlet1 = "";

                List<outlet> appMenuOutlet = new List<outlet>();
                cmd2 = "SELECT \"outletName\" from outlet WHERE \"isDeleted\"=\'0\' and \"outletName\"='"+obj.outletName+"'";
                appMenuOutlet = (List<outlet>)dapperQuery.QryResult<outlet>(cmd2, _dbCon);

                if (appMenuOutlet.Count > 0)
                    outlet1 = appMenuOutlet[0].outletName;

                int rowAffected = 0;
                var response = "";

                if(obj.outletID==0)
                {
                    if(outlet1=="")
                    {
                        cmd = "insert into public.outlet (\"outletName\",\"outletShortName\",\"outletAddress\",\"contactPerson\",\"phoneNo\",\"mobileNo\",\"email\",\"createdOn\",\"createdBy\",\"isDeleted\") Values ('" + obj.outletName + "', '" + obj.outletShortName + "', '" + obj.outletAddress + "','"+obj.contactPerson+"','"+obj.phoneNo+"','"+obj.mobileNo+"','"+obj.email+"','"+curDate+"',"+obj.userID+",B'0')";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "UPDATE INTO public.outlet set \"outletName\" = '"+obj.outletName+"', \"outletShortName\"='"+obj.outletShortName+"', \"outletAddress\"='"+obj.outletAddress+"',\"contactPerson\"='"+obj.contactPerson+"',\"phoneNo\"='"+obj.phoneNo+"',\"mobileNo\"='"+obj.mobileNo+"',\"email\"='"+obj.email+"',\"modifiedOn\"='"+curDate+"',\"modifiedBy\"="+obj.userID+"";
                }

                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd);
                    }
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                }
                else
                {
                    if (found == true)
                    {
                        response = "Record already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }

                return Ok(new { message = response });
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }
        [HttpPost("deleteCity")]
        public IActionResult deleteOutlet(outletCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.outlet set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"outletID\" = " + obj.outletID + ";";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Server Issue";
                }

                return Ok(new { message = response });
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }
    }
}