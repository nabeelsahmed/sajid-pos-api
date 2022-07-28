using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMISModuleApi.Entities;
using CMISModuleApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using posCoreModuleApi.Services;
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
        
    }
}