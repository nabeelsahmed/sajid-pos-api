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
    public class PaymentController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3, cmd4, cmd5;

        public PaymentController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getPayments")]
        public IActionResult getPayments()
        {
            try
            {
                cmd = "select * from view_payment ORDER BY \"invoiceNo\" DESC";
                var appMenu = dapperQuery.Qry<Payment>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getPaymentDetail")]
        public IActionResult getPaymentDetail(int invoiceNo)
        {
            try
            {
                cmd = "select \"invoiceNo\", debit, credit, \"coaID\" from \"invoiceDetail\" where \"isDeleted\"::int = 0 and \"invoiceNo\" = " + invoiceNo + " ";
                var appMenu = dapperQuery.Qry<PaymentDetail>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("savePayment")]
        public IActionResult savePayment(PaymentCreation obj)
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
                var response = "";
                List<Invoice> appMenuInvoice = new List<Invoice>();

                var payType = "";
                var total = 0.0;

                if (obj.type == "1")
                {
                    payType = "payment";
                }
                else
                {
                    payType = "receipt";
                }

                cmd = "insert into public.invoice (\"invoiceDate\", \"invoicetime\", \"partyID\", \"cashReceived\", \"discount\", \"invoiceType\", \"description\", \"branchID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceDate + "', '" + time + "', '" + obj.partyID + "', " + obj.amount + ", " + obj.discount + ", '" + payType + "', '" + obj.description + "', '" + obj.branchID + "', '" + curDate + "', " + obj.userID + ", B'0')";

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

                    //in case of giving discount to over all bill
                    if (obj.discount > 0)
                    {
                        if (obj.type == "1")
                        {
                            cmd2 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', 0, '" + obj.discount + "', '3', '" + curDate + "', " + obj.userID + ", B'0')";
                        }
                        else
                        {
                            cmd2 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + obj.discount + "', 0, '3', '" + curDate + "', " + obj.userID + ", B'0')";
                        }

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd2);
                        }

                    }

                    total = obj.amount - obj.discount;
                    if (obj.type == "1")
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', 0, '" + total + "', '" + obj.categoryID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + obj.amount + "', 0, '" + obj.coaID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }
                    else
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + total + "', 0, '" + obj.categoryID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', 0, '" + obj.amount + "', '" + obj.coaID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected3 = con.Execute(cmd3);
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected4 = con.Execute(cmd4);
                    }
                }

                if (rowAffected > 0 && rowAffected3 > 0 && rowAffected4 > 0)
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


        [HttpPost("updatePayment")]
        public IActionResult updatePayment(PaymentCreation obj)
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

                var payType = "";
                var total = 0.0;

                if (obj.type == "1")
                {
                    payType = "payment";
                }
                else
                {
                    payType = "receipt";
                }

                cmd = "update public.\"invoice\" set \"invoiceDate\" = '" + obj.invoiceDate + "', \"partyID\" = '" + obj.partyID + "', \"cashReceived\" = '" + obj.amount + "', \"discount\" = '" + obj.discount + "', \"invoiceType\" = '" + payType + "', \"description\" = '" + obj.description + "', \"branchID\" = '" + obj.branchID + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"invoiceNo\" = " + obj.invoiceNo + ";";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                //confirmation of data saved in invoice
                if (rowAffected > 0)
                {

                    cmd5 = "update public.\"invoiceDetail\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"invoiceNo\" = " + obj.invoiceNo + ";";
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected5 = con.Execute(cmd5);
                    }

                    //in case of giving discount to over all bill
                    if (obj.discount > 0)
                    {
                        if (obj.type == "1")
                        {
                            cmd2 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceNo + "', 0, '" + obj.discount + "', '3', '" + curDate + "', " + obj.userID + ", B'0')";
                        }
                        else
                        {
                            cmd2 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceNo + "', '" + obj.discount + "', 0, '3', '" + curDate + "', " + obj.userID + ", B'0')";
                        }

                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd2);
                        }

                    }

                    total = obj.amount - obj.discount;
                    if (obj.type == "1")
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceNo + "', 0, '" + total + "', '" + obj.categoryID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceNo + "', '" + obj.amount + "', 0, '" + obj.coaID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }
                    else
                    {
                        cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceNo + "', '" + total + "', 0, '" + obj.categoryID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                        cmd4 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"debit\", \"credit\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.invoiceNo + "', 0, '" + obj.amount + "', '" + obj.coaID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected3 = con.Execute(cmd3);
                    }

                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected4 = con.Execute(cmd4);
                    }
                }

                if (rowAffected > 0 && rowAffected3 > 0 && rowAffected4 > 0)
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

        [HttpPost("deletePayment")]
        public IActionResult deletePayment(PaymentCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";

                cmd = "update public.\"invoice\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"invoiceNo\" = " + obj.invoiceNo + ";";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                cmd2 = "update public.\"invoiceDetail\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"invoiceNo\" = " + obj.invoiceNo + ";";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected2 = con.Execute(cmd2);
                }

                if (rowAffected > 0 && rowAffected2 > 0)
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