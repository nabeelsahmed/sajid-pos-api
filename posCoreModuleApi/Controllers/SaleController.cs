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
    public class SaleController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3, cmd4, cmd5, cmd6, cmd7;

        public SaleController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }
        
        [HttpGet("getProductInventory")]
        public IActionResult getProductInventory(int productID)
        {
            try
            {
                cmd = "SELECT sum(pqty)-sum(sqty) as Qty FROM product_inventory('"+productID+"')";

                var appMenu = dapperQuery.Qry<productInventory>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getSale")]
        public IActionResult getSale(int outletID)
        {
            try
            {
                cmd = "select * from invoice where outletid = " + outletID + " AND \"invoiceType\" = 'S' AND \"isDeleted\"::int = 0 order by \"invoiceNo\" Desc";

                var appMenu = dapperQuery.Qry<Sale>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getSaleDetail")]
        public IActionResult getSaleDetail(int invoiceNo)
        {
            try
            {
                cmd = "select * from \"invoiceDetail\" where \"invoiceNo\" = " + invoiceNo + " AND \"isDeleted\"::int = 0 AND \"productID\" is not NULL order by \"invoiceDetailID\" Asc";

                var appMenu = dapperQuery.Qry<SaleDetail>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        
        [HttpGet("getSaleReturn")]
        public IActionResult getSaleReturn(int invoiceNo)
        {
            try
            {
                cmd = "SELECT id.\"productID\", sum(case when i.\"invoiceType\" = 'SR' then id.qty * (-1) else id.qty end )as qty FROM invoice i JOIN \"invoiceDetail\" id ON id.\"invoiceNo\" = i.\"invoiceNo\" where i.\"invoiceNo\" = " + invoiceNo + " or i.\"refInvoiceNo\" = " + invoiceNo + " and id.\"productID\" is not null GROUP By id.\"productID\"";

                // cmd = "select * from \"view_saleReturn\" where \"invoiceNo\" = " + invoiceNo + " and \"isDeleted\"::int = 0 and \"productID\" is not null";

                var appMenu = dapperQuery.Qry<SaleReturn>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveSales")]
        public IActionResult saveSales(InvoiceCreation obj)
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
                int rowAffected6 = 0;

                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();
                // List<Invoice> appMenuBarcode = new List<Invoice>();
                var total = 0.0;
                var boxPrice = 0;


                if (obj.partyID == 0)
                {
                    //In case of partyID is null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"branchID\",\"outletid\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceDate + "', '" + time + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + obj.branchID + "', "+obj.outletid+",'" + curDate + "', " + obj.userID + ", B'0')";
                }
                else
                {
                    //In case of partyID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"branchID\",\"outletid\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceDate + "', '" + time + "', '" + obj.partyID + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'S', '" + obj.description + "', '" + obj.branchID + "',"+obj.outletid+",'" + curDate + "', " + obj.userID + ", B'0')";
                }

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
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"qty\", \"costPrice\", \"salePrice\", \"boxprice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', '" + item.boxprice + "', 0, '" + ((item.qty * item.salePrice) + (item.boxprice)) + "', '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0')";

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }

                        var curQty = (item.availableqty - item.qty);
                        cmd7 = "update public.\"productPrice\" set \"availableqty\" = '" + curQty + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"pPriceID\" = " + item.pPriceID + ";";
                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd7);
                        }

                        total +=((item.qty * item.salePrice) + item.boxprice);
                        // boxPrice = item.boxPrice
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

                    var change = 0.0;
                    //in case of cash payment
                    if (obj.cashReceived > 0)
                    {
                        change = total - obj.cashReceived;

                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + (obj.cashReceived + change) + "', 0, '2', '" + curDate + "', " + obj.userID + ", B'0')";

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

        [HttpPost("saveSaleReturn")]
        public IActionResult saveSaleReturn(InvoiceCreation obj)
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

                //getting invoice date from current saved invoice
                cmd2 = "SELECT \"invoiceDate\" FROM public.invoice where \"invoiceNo\" = " + obj.refInvoiceNo + " order by \"invoiceNo\" desc limit 1";
                appMenuInvoice = (List<Invoice>)dapperQuery.QryResult<Invoice>(cmd2, _dbCon);

                var invoiceDate = appMenuInvoice[0].invoiceDate;


                if (obj.partyID == 0)
                {
                    //In case of partyID is null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"refInvoiceNo\", \"refInvoiceDate\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"branchID\",\"outletid\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceDate + "', '" + time + "','" + obj.refInvoiceNo + "','" + obj.refInvoiceDate + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'SR', '" + obj.description + "', '" + obj.branchID + "',"+obj.outletid+", '" + curDate + "', " + obj.userID + ", B'0')";
                }
                else
                {
                    //In case of partyID is not null
                    cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\", \"cashReceived\", \"discount\", \"change\", \"invoiceType\", \"description\", \"branchID\",\"outletid\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceDate + "', '" + time + "', '" + obj.partyID + "', " + obj.cashReceived + ", " + obj.discount + ", '" + obj.change + "', 'SR', '" + obj.description + "', '" + obj.branchID + "',"+obj.outletid+", '" + curDate + "', " + obj.userID + ", B'0')";
                }

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
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"locationID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.locationID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', '" + item.qty * item.salePrice + "', 0, '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0')";

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

                        cmd6 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', 0, '" + obj.discount + "', '3', '" + curDate + "', " + obj.userID + ", B'0')";

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

                if (rowAffected > 0 && rowAffected2 > 0 && rowAffected3 > 0)
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

        [HttpPost("deleteSales")]
        public IActionResult deleteSales(InvoiceCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                var response = "";

                // cmd = "update product set \"isDeleted\" = B'1', \"modifiedOn\" = '"+ curDate +"', \"modifiedBy\" = "+ obj.userID +" where \"productID\" = "+ obj.productID +";";

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
                    response = "Invalid Input Error";
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("savePortalPrice")]
        public IActionResult savePortalPrice(PortalPriceCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;

                var response = "";
                cmd = "update public.\"productPrice\" set availableqty = '" + obj.availableqty + "', \"salePrice\" = '" + obj.salePrice + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"pPriceID\" = " + obj.pPriceID + ";";
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