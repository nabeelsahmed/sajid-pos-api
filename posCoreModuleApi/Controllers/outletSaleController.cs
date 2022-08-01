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
    public class outletSaleController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3, cmd4, cmd5, cmd6;

        public outletSaleController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpPost("saveOutletSales")]
        public IActionResult saveOutletSales(outletSaleCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                int rowAffected4 = 0;
                int rowAffected5 = 0;
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();
                // List<Invoice> appMenuBarcode = new List<Invoice>();
                var total = 0.0;


                //In case of partyID is not null
                cmd = "insert into public.invoice (\"invoiceDate\",\"partyID\", \"invoicetime\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"branchID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"outletid\") values ('" + obj.invoiceDate + "',"+obj.partyID+", '" + time + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + obj.branchID + "', '" + curDate + "', " + obj.userID + ", B'0',"+obj.outletid+")";
                    
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                //confirmation of data saved in invoice
                if (rowAffected > 0)
                {

                    //getting last saved invoice no
                    cmd2 = "SELECT \"invoiceNo\" FROM public.invoice order by \"invoiceNo\" desc limit 1";
                    appMenuInvoice = (List<Invoice>)dapperQuery.QryResult<Invoice>(cmd2, _dbCon);

                    var invoiceNo = appMenuInvoice[0].invoiceNo;

                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<InvoiceDetailCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', 0, '" + item.qty * item.salePrice + "', '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }

                        total += item.salePrice;
                    }

                    total -= obj.discount;

                    //in case of giving discount to over all bill
                    if (obj.discount > 0)
                    {

                        cmd6 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + obj.discount + "', 0, '3', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected5 = con.Execute(cmd6);
                        }

                    }

                    //in case of loan (udhaar) payment where partyID is not null
                    if (obj.partyID > 0 && (total - obj.cashReceived) > 0)
                    {
                        total -= obj.cashReceived;

                        cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + total + "', 0, '6', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected4 = con.Execute(cmd5);
                        }
                    }

                    //in case of cash payment
                    if (obj.cashReceived > 0)
                    {

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + obj.cashReceived + "', 0, '2', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected3 = con.Execute(cmd4);
                        }
                    }

                }

                if (rowAffected > 0 && rowAffected2 > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Server Issue";
                }

                return Ok(new { message = response, invoiceNo = appMenuInvoice[0].invoiceNo });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveOutletSalesReturn")]
        public IActionResult saveOutletSalesReturn(outletSaleCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                int rowAffected4 = 0;
                int rowAffected5 = 0;
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();
                // List<Invoice> appMenuBarcode = new List<Invoice>();
                var total = 0.0;


                //In case of partyID is not null
                cmd = "insert into public.invoice (\"invoiceDate\",\"partyID\", \"invoicetime\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"branchID\", \"createdOn\", \"createdBy\", \"isDeleted\",\"outletid\") values ('" + obj.invoiceDate + "',"+obj.partyID+", '" + time + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + obj.branchID + "', '" + curDate + "', " + obj.userID + ", B'0',"+obj.outletid+")";
                    
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                //confirmation of data saved in invoice
                if (rowAffected > 0)
                {

                    //getting last saved invoice no
                    cmd2 = "SELECT \"invoiceNo\" FROM public.invoice order by \"invoiceNo\" desc limit 1";
                    appMenuInvoice = (List<Invoice>)dapperQuery.QryResult<Invoice>(cmd2, _dbCon);

                    var invoiceNo = appMenuInvoice[0].invoiceNo;

                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<InvoiceDetailCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', 0, '" + item.qty * item.salePrice + "', '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }

                        total += item.salePrice;
                    }

                    total -= obj.discount;

                    //in case of giving discount to over all bill
                    if (obj.discount > 0)
                    {

                        cmd6 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + obj.discount + "', 0, '3', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected5 = con.Execute(cmd6);
                        }

                    }

                    //in case of loan (udhaar) payment where partyID is not null
                    if (obj.partyID > 0 && (total - obj.cashReceived) > 0)
                    {
                        total -= obj.cashReceived;

                        cmd5 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', 0, '" + total + "', '6', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected4 = con.Execute(cmd5);
                        }
                    }

                    //in case of cash payment
                    if (obj.cashReceived == 0)
                    {

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', 0, '" + -1 * obj.change + "', '2', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected3 = con.Execute(cmd4);
                        }
                    }

                }

                if (rowAffected > 0 && rowAffected2 > 0)
                {
                    response = "Success";
                }
                else
                {
                    response = "Server Issue";
                }

                return Ok(new { message = response, invoiceNo = appMenuInvoice[0].invoiceNo });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }
    }
}
