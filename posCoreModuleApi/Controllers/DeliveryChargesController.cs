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
    public class DeliveryChargesController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public DeliveryChargesController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getDelvieryCharges")]
        public IActionResult getDelvieryCharges()
        {
            try
            {
                cmd = "select * from public.\"tbl_deliveryCharges\" where \"isDeleted\"::int = 0 ";
                var appMenu = dapperQuery.Qry<DeliveryCharges>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveDeliveryCharges")]
        public IActionResult saveDeliveryCharges(DeliveryChargesCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");
                var found = false;
                var delivery = "";

                List<DeliveryCharges> appMenuDelivery = new List<DeliveryCharges>();
                cmd2 = "select \"description\" from \"tbl_deliveryCharges\" where \"isDeleted\"::int = 0";
                appMenuDelivery = (List<DeliveryCharges>)dapperQuery.QryResult<DeliveryCharges>(cmd2, _dbCon);

                if (appMenuDelivery.Count > 0)
                    delivery = appMenuDelivery[0].description;

                int rowAffected = 0;
                var response = "";

                if (obj.deliveryChargesID == 0)
                {
                    if (delivery == "")
                    {
                        cmd = "insert into public.\"tbl_deliveryCharges\" (\"amount\", \"description\", \"isDeleted\") values ('" + obj.amount + "', '" + obj.description + "', B'0')";
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"tbl_deliveryCharges\" set \"amount\" = '" + obj.amount + "', \"description\" = '" + obj.description + "' where \"deliveryChargesID\" = " + obj.deliveryChargesID + ";";
                }

                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd);
                    }
                }

                if (rowAffected > 0)
                {
                    response = "Success";
                }
                else
                {
                    if (found == true)
                    {
                        response = "Record already exist";
                    }
                    else
                    {
                        response = "Server Issue";
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