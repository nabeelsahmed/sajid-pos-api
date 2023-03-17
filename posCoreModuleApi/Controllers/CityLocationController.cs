using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using posCoreModuleApi.Services;
using Microsoft.Extensions.Options;
using posCoreModuleApi.Configuration;
using posCoreModuleApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace posCoreModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CityLocationController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public CityLocationController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getCityLocation")]
        public IActionResult getCityLocation()
        {
            try
            {
                cmd = "select * from public.\"cityLocation\" where \"isDeleted\"::int = 0 ";
                var appMenu = dapperQuery.Qry<CityLocation>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveCityLocation")]
        public IActionResult saveCityLocation(CityLocationCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");
                var found = false;
                var location = "";

                List<CityLocation> appMenuCityLocation = new List<CityLocation>();
                cmd2 = "select \"locationName\" from \"cityLocation\" where \"isDeleted\"::int = 0 and \"locationName\" = '" + obj.locationName + "'";
                appMenuCityLocation = (List<CityLocation>)dapperQuery.QryResult<CityLocation>(cmd2, _dbCon);

                if (appMenuCityLocation.Count > 0)
                    location = appMenuCityLocation[0].locationName;

                int rowAffected = 0;
                var response = "";

                if (obj.locationID == 0)
                {
                    if (location == "")
                    {
                        cmd = "insert into public.\"cityLocation\" (\"locationName\", \"cityID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.locationName + "','" + obj.cityID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"cityLocation\" set \"locationName\" = '" + obj.locationName + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"locationID\" = " + obj.locationID + ";";
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
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("deleteCityLocation")]
        public IActionResult deleteCityLocation(CityLocationCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.\"cityLocation\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"locationID\" = " + obj.locationID + ";";

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
            catch (Exception e)
            {
                return Ok(e);
            }

        }

    }
}