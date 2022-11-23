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
    public class OpeningBalanceController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3;
    
        public OpeningBalanceController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getOpeningBalance")]
        public IActionResult getOpeningBalance()
        {
            try
            {
                cmd = "SELECT * FROM \"view_openingBalance\" order by \"invoiceNo\" desc";
                var appMenu = dapperQuery.Qry<OpeningBalance>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }
        
        [HttpGet("getOpeningBalanceProduct")]
        public IActionResult getOpeningBalanceProduct(int categoryID)
        {
            try
            {
                cmd = "SELECT * FROM \"view_openingBalance_product\" where \"categoryID\" = " + categoryID + " order by \"productID\" desc";
                var appMenu = dapperQuery.Qry<OpeningBalanceProduct>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveBalance")]
        public IActionResult saveBalance(OpeningBalanceCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                // DateTime curTime = DateTime.Now;
                var time = Convert.ToDateTime(obj.invoiceDate).ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();

                if(obj.invoiceNo == 0){
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"invoiceType\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('"+ obj.invoiceDate +"', '"+ time +"', 'OB', '"+ curDate +"', "+ obj.userID +", B'0')";

                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd);
                    }   

                    //confirmation of data saved in invoice
                    if(rowAffected > 0){

                        //getting last saved invoice no
                        cmd2 = "SELECT \"invoiceNo\" FROM public.invoice order by \"invoiceNo\" desc limit 1";
                        appMenuInvoice = (List<Invoice>)dapperQuery.QryResult<Invoice>(cmd2, _dbCon);

                        var invoiceNo = appMenuInvoice[0].invoiceNo;


                            cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('"+ invoiceNo +"', '"+ obj.productID +"', '"+ obj.qty +"', '"+ obj.costPrice +"', '"+ obj.salePrice +"', '"+ obj.debit +"', 0, '"+ obj.productName +"', '1', '"+ curDate +"', "+ obj.userID +", B'0')";
                            
                            using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                            {
                                rowAffected2 = con.Execute(cmd3);
                            }
        
                    }

                }   else{
                    cmd = "update public.\"invoiceDetail\" set \"qty\" = '" + obj.qty + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"invoiceNo\" = " + obj.invoiceNo + ";";
                    
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd);
                    }  
                }             

                if(obj.invoiceNo == 0){
                    if(rowAffected > 0 && rowAffected2 > 0){
                        response = "Success";
                    }else{
                        response = "Server Issue";
                    }
                    
                }else{
                    if(rowAffected > 0){
                        response = "Success";
                    }
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