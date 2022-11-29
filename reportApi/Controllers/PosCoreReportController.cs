using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using reportApi.Services;
using Microsoft.Extensions.Options;
using reportApi.Configuration;
using reportApi.Entities;
using Dapper;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace reportApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PosCoreReportController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public PosCoreReportController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }
        
        [HttpGet("getCurrentStock")]
        public IActionResult getCurrentStock(string currentDate, int outletID)
        {
            try
            {
            
                cmd = "select * from func_CurrentStockRpt('" + currentDate + "', " + outletID + ")";

                var appMenu = dapperQuery.Qry<CurrentStock>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySalesOutlet")]
        public IActionResult getDailySalesOutlet( string fromDate, string toDate,int outletID)
        {
            try
            {
                if(outletID == 1){
                    cmd = "select * from \"view_dailySales\" where \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "'";
                }else{
                    cmd = "select * from \"view_dailySalesOutlets\" where outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "'";
                }
            

                var appMenu = dapperQuery.Qry<DailySales>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySales")]
        public IActionResult getDailySales( string fromDate, string toDate,int outletID)
        {
            try
            {
                cmd = "select * from \"view_dailySalesOutlets\" where outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "'";

                var appMenu = dapperQuery.Qry<DailySales>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
    }
}