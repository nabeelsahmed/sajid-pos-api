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
        
        [HttpGet("getDailySalesOutletProductWise")]
        public IActionResult getDailySalesOutletProductWise( string fromDate, string toDate,int outletID)
        {
            try
            {
                // if(outletID == 1){
                //     cmd = "SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'S' AND outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";
                // }else{
                cmd = "SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'S' AND outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";
                // }
            

                var appMenu = dapperQuery.Qry<DailySales>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySalesProductWise")]
        public IActionResult getDailySalesProductWise( string fromDate, string toDate,int outletID)
        {
            try
            {
                    cmd = "SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'SO' AND ivd.\"productID\" IS NOT NULL AND \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";
                // cmd = "SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'SO' AND ivd.\"productID\" IS NOT NULL where outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";

                var appMenu = dapperQuery.Qry<DailySales>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySalesOutletInvoiceWise")]
        public IActionResult getDailySalesOutletInvoiceWise( string fromDate, string toDate,int outletID)
        {
            try
            {
                // if(outletID == 1){
                //     cmd = "SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'S' AND outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";
                // }else{
                cmd = "select inv.\"invoiceDate\", inv.\"invoiceNo\", sum(det.qty) as \"qty\", sum(det.qty * det.\"salePrice\") as \"salePrice\" from invoice as inv inner join \"invoiceDetail\" as det on inv.\"invoiceNo\" = det.\"invoiceNo\" where inv.outletid = " + outletID + " and inv.\"invoiceType\" = 'S' group by inv.\"invoiceDate\",inv.\"invoiceNo\" having (inv.\"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "')";

                // SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'S' AND outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";
                // }
            

                var appMenu = dapperQuery.Qry<DailySales>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getDailySalesInvoiceWise")]
        public IActionResult getDailySalesInvoiceWise( string fromDate, string toDate,int outletID)
        {
            try
            {
                    cmd = "select inv.\"invoiceDate\", inv.\"invoiceNo\", sum(det.qty) as \"qty\", sum(det.qty * det.\"salePrice\") as \"salePrice\" from invoice as inv inner join \"invoiceDetail\" as det on inv.\"invoiceNo\" = det.\"invoiceNo\" where inv.\"invoiceType\" = 'SO' group by inv.\"invoiceDate\",inv.\"invoiceNo\" having (inv.\"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "')";
                    // SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'SO' AND ivd.\"productID\" IS NOT NULL AND \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";
                // cmd = "SELECT ivd.\"productID\", ivd.\"productName\", round(sum(ivd.qty)::numeric, 2) AS qty, round(sum(ivd.qty * ivd.\"salePrice\")::numeric, 2) AS \"salePrice\" FROM invoice i JOIN \"invoiceDetail\" ivd ON ivd.\"invoiceNo\" = i.\"invoiceNo\" GROUP BY i.outletid, i.\"invoiceType\", ivd.\"productID\", ivd.\"productName\", i.\"invoiceDate\" HAVING i.\"invoiceType\" = 'SO' AND ivd.\"productID\" IS NOT NULL where outletid = " + outletID + " and \"invoiceDate\" BETWEEN '" + fromDate + "' AND '" + toDate + "' order by \"productID\" Asc";

                var appMenu = dapperQuery.Qry<DailySales>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getOverAllStockOutlet")]
        public IActionResult getOverAllStockOutlet( string fromDate, string toDate,int outletID)
        {
            try
            {
                    cmd = "SELECT distinct det.\"productID\", det.\"productName\", (select sum(ivd.qty) as qty from invoice i join \"invoiceDetail\" ivd on i.\"invoiceNo\" = ivd.\"invoiceNo\" AND ivd.\"productID\" = det.\"productID\" where (i.\"invoiceType\" = 'P' OR i.\"invoiceType\" = 'PO') and i.outletid = " + outletID + ") as \"IN\", (COALESCE((select sum(ivd.qty) as qty from invoice i join \"invoiceDetail\" ivd on i.\"invoiceNo\" = ivd.\"invoiceNo\" AND ivd.\"productID\" = det.\"productID\" where i.\"invoiceType\" = 'S' and i.outletid = " + outletID + "), 0)) as \"OUT\" FROM invoice inv inner join \"invoiceDetail\" det on inv.\"invoiceNo\" = det.\"invoiceNo\" where inv.outletid = " + outletID + " AND inv.\"invoiceDate\" Between '" + fromDate + "' AND '" + toDate + "' group by det.\"productID\", det.\"productName\", inv.\"invoiceType\" order by det.\"productID\" ASC";

                var appMenu = dapperQuery.Qry<OverAllStock>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getOverAllStock")]
        public IActionResult getOverAllStock( string fromDate, string toDate,int outletID)
        {
            try
            {
                    cmd = "SELECT distinct det.\"productID\", det.\"productName\", (COALESCE((select sum(ivd.qty) as qty from invoice i join \"invoiceDetail\" ivd on i.\"invoiceNo\" = ivd.\"invoiceNo\" AND ivd.\"productID\" = det.\"productID\" where i.\"invoiceType\" = 'SO'), 0)) as \"OUT\" FROM invoice inv inner join \"invoiceDetail\" det on inv.\"invoiceNo\" = det.\"invoiceNo\" where inv.\"invoiceDate\" Between '" + fromDate + "' AND '" + toDate + "' group by det.\"productID\", det.\"productName\", inv.\"invoiceType\" order by det.\"productID\" ASC";

                var appMenu = dapperQuery.Qry<OverAllStock>(cmd, _dbCon);
                return Ok(appMenu);
            
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }


    }
}