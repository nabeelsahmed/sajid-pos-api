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
    public class PartyController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2;

        public PartyController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getParty")]
        public IActionResult getParty()
        {
            try
            {
                cmd = "SELECT * FROM view_party order by \"partyID\" desc";
                var appMenu = dapperQuery.Qry<Party>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getSaleParty")]
        public IActionResult getSaleParty()
        {
            try
            {
                cmd = "SELECT * FROM view_saleParty order by \"partyID\" desc";
                var appMenu = dapperQuery.Qry<Party>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getAllParties")]
        public IActionResult getAllParties()
        {
            try
            {
                cmd = "select * from public.party where \"isDeleted\"::int = 0";
                var appMenu = dapperQuery.Qry<Party>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getPartyOutlet")]
        public IActionResult getPartyOutlet()
        {
            try
            {
                cmd = "select \"partyID\", \"partyName\", outletid, \"type\" from party  where \"type\" = 'outlet' AND \"isDeleted\"::int = 0";
                var appMenu = dapperQuery.Qry<partyOutlet>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveParty")]
        public IActionResult saveParty(PartyCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";
                var found = false;
                var cnic = "";

                List<Party> appMenuParty = new List<Party>();
                cmd2 = "select cnic from party where \"isDeleted\"::int = 0 AND cnic = '" + obj.cnic + "' AND (\"type\" = 'supplier' OR \"type\" = 'customer')";
                appMenuParty = (List<Party>)dapperQuery.QryResult<Party>(cmd2, _dbCon);

                if (appMenuParty.Count > 0)
                    cnic = appMenuParty[0].cnic;

                if (obj.partyID == 0)
                {
                    if (cnic == "")
                    {
                        if(obj.type=="outlet")
                        {
                            cmd = "insert into public.\"party\" (\"cityID\", \"partyName\", \"partyNameUrdu\", \"address\", \"addressUrdu\", \"phone\", \"mobile\", \"type\", \"description\", \"cnic\", \"focalperson\", \"createdOn\", \"createdBy\", \"isDeleted\",\"outletid\") values ('" + obj.cityID + "', '" + obj.partyName + "', '" + obj.partyNameUrdu + "', '" + obj.address + "', '" + obj.addressUrdu + "', '" + obj.phone + "', '" + obj.mobile + "', '" + obj.type + "', '" + obj.description + "', '" + obj.cnic + "', '" + obj.focalPerson + "', '" + curDate + "', " + obj.userID + ", B'0',"+obj.outletid+")";
                        }
                        else
                        {
                            cmd = "insert into public.\"party\" (\"cityID\", \"partyName\", \"partyNameUrdu\", \"address\", \"addressUrdu\", \"phone\", \"mobile\", \"type\", \"description\", \"cnic\", \"focalperson\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + obj.cityID + "', '" + obj.partyName + "', '" + obj.partyNameUrdu + "', '" + obj.address + "', '" + obj.addressUrdu + "', '" + obj.phone + "', '" + obj.mobile + "', '" + obj.type + "', '" + obj.description + "', '" + obj.cnic + "', '" + obj.focalPerson + "', '" + curDate + "', " + obj.userID + ", B'0')";
                        }
                    }
                    else
                    {
                        found = true;
                    }
                }
                else
                {
                    cmd = "update public.\"party\" set \"cityID\" = '" + obj.cityID + "', \"partyName\" = '" + obj.partyName + "', \"partyNameUrdu\" = '" + obj.partyNameUrdu + "', \"address\" = '" + obj.address + "', \"addressUrdu\" = '" + obj.addressUrdu + "', \"phone\" = '" + obj.phone + "', \"mobile\" = '" + obj.mobile + "', \"type\" = '" + obj.type + "', \"description\" = '" + obj.description + "', \"cnic\" = '" + obj.cnic + "', \"focalperson\" = '" + obj.focalPerson + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + ", \"outletid\"="+obj.outletid+" where \"partyID\" = " + obj.partyID + ";";
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
                        response = "CNIC already exist";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                }
                
                return Ok(new { message = response});
                
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("deleteParty")]
        public IActionResult deleteParty(PartyCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.\"party\" set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"partyID\" = " + obj.partyID + ";";

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