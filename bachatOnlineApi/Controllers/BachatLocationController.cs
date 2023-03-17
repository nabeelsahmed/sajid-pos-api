using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using bachatOnlineModuleApi.Services;
using Microsoft.Extensions.Options;
using bachatOnlineModuleApi.Configuration;
using bachatOnlineModuleApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace bachatOnlineApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class BachatLocationController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public BachatLocationController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getPortalCity")]
        public IActionResult getPortalCity()
        {
            try
            {
                cmd = "select * from public.\"city\" where \"isDeleted\"::int = 0 ";
                var appMenu = dapperQuery.Qry<PortalCity>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }
        [HttpGet("getPortalCityLocation")]
        public IActionResult getPortalCityLocation(int cityID)
        {
            try
            {
                cmd = "select * from public.\"cityLocation\" where \"cityID\" = " + cityID + " AND \"isDeleted\"::int = 0 ";
                var appMenu = dapperQuery.Qry<PortalCityLocation>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

    }
}