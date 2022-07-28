using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMISModuleApi.Services;
using Microsoft.Extensions.Options;
using CMISModuleApi.Configuration;
using CMISModuleApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace CMISModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd;

        public BranchController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getBranch")]
        public IActionResult getBranch()
        {
            try
            {
                cmd = "SELECT * FROM public.branch where \"isDeleted\"::int = 0";
                var appMenu = dapperQuery.Qry<Branch>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

    }
}