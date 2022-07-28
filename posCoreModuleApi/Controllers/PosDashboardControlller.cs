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
using Newtonsoft.Json;

namespace posCoreModuleApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PosDashboardController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3;

        public PosDashboardController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }
        
        [HttpGet("getTodaySaleTransaction")]
        public IActionResult getTodaySaleTransaction()
        {
            try
            {
                cmd = "select * from public.\"view_todaySaleTransaction\"";
                var appMenu = dapperQuery.Qry<SaleTransactionDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getTodaySaleAmount")]
        public IActionResult getTodaySaleAmount()
        {
            try
            {
                cmd = "select * from public.\"view_todaySaleAmount\"";
                var appMenu = dapperQuery.Qry<SaleAmountDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getTopSales")]
        public IActionResult getTopSales()
        {
            try
            {
                cmd = "select * from public.\"view_topSellingItem\"";
                var appMenu = dapperQuery.Qry<TopSalesDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getCoaTypeSummary")]
        public IActionResult getCoaTypeSummary(string fromDate, string toDate)
        {
            try
            {
                cmd = "select * from fn_dash_coa_type_summary('" + fromDate + "', '" + toDate + "')";
                var appMenu = dapperQuery.Qry<COATypeSummaryDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getUnderStock")]
        public IActionResult getUnderStock()
        {
            try
            {
                cmd = "select * from fn_dash_under_stock(Current_date)";
                var appMenu = dapperQuery.Qry<UnderStockDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getMonthlyExpense")]
        public IActionResult getMonthlyExpense(string fromDate, string toDate)
        {
            try
            {
                cmd = "select * from public.fn_dash_monthly_expense_income('" + fromDate + "', '" + toDate + "')";
                var appMenu = dapperQuery.Qry<MonthlyExpenseDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySales")]
        public IActionResult getDailySales(string fromDate, string toDate)
        {
            try
            {
                cmd = "select * from public.fn_dash_daily_sale('" + fromDate + "', '" + toDate + "')";
                var appMenu = dapperQuery.Qry<DailySalesDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getMonthlySales")]
        public IActionResult getMonthlySales(string fromDate, string toDate)
        {
            try
            {
                cmd = "select * from public.fn_dash_monthly_sale('" + fromDate + "', '" + toDate + "')";
                var appMenu = dapperQuery.Qry<MonthlySalesDashboard>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
    }
}