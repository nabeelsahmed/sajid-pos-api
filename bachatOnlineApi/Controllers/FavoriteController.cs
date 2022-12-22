using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using bachatOnlineModuleApi.Services;
using Microsoft.Extensions.Options;
using bachatOnlineModuleApi.Configuration;
using bachatOnlineModuleApi.Entities;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using MySqlConnector;
using Dapper;
using Newtonsoft.Json;

namespace bachatOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3;

        public FavoriteController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getfavorite")]
        public IActionResult getfavorite(int userID)
        {
            try
            {
                cmd = "SELECT * FROM \"view_userFavorite\" where \"userID\"="+userID+"";
                var appMenu = dapperQuery.QryResult<Product>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getProductById")]
        public IActionResult getProductById(int productID)
        {
            try
            {
                cmd = "SELECT * FROM view_product where \"productID\"="+productID+"";
                var appMenu = dapperQuery.QryResult<Product>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveFavorite")]
        public IActionResult saveFavorite(FavoriteCreation obj)
        {
            try
            {
                int rowAffected = 0;
                var response = "";
                var newFavoriteID = 0;
                var found = false;

                List<Favorite> appMenu = new List<Favorite>();
                List<FavoriteCreation> appMenuID = new List<FavoriteCreation>();

                cmd = "select \"favoriteID\" from favorite where \"productID\" = "+obj.productID+" AND \"userID\" = '" + obj.userID + "'";
                appMenu = (List<Favorite>)dapperQuery.QryResult<Favorite>(cmd, _dbCon);

                cmd2 = "select \"favoriteID\" from favorite ORDER BY \"favoriteID\" DESC LIMIT 1";
                appMenuID = (List<FavoriteCreation>)dapperQuery.QryResult<FavoriteCreation>(cmd2, _dbCon);
                if(appMenuID.Count == 0)
                {
                    newFavoriteID = 1;
                }
                else
                {
                    newFavoriteID = appMenuID[0].favoriteID+1;
                }

                if (appMenu.Count > 0)
                {
                    found=true;
                }
                else
                {
                    cmd3 = "insert into public.favorite (\"favoriteID\",\"userID\",\"productID\") values ("+newFavoriteID+","+obj.userID+","+obj.productID+")";
                }

                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd3);
                    }
                }

                if(rowAffected>0)
                {
                    response="Success";
                }
                else
                {
                    if(found==true)
                    {
                        response="Favorite already exist";
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

        // [HttpPost("deleteFavorite")]
        // public IActionResult getfavorite(int userID,int productID)
        // {
        //     try
        //     {
        //         cmd = "DELETE FROM favorite where \"userID\"="+userID+" and \"productID\"="+productID+"";
        //         var appMenu = dapperQuery.Qry<Favorite>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }
        // }

        [HttpPost("deleteFavorite")]
        public IActionResult deleteFavorite(FavoriteCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;

                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");
                var found = false;
                //var name = "";

                // List<Shop> appMenuShop = new List<Shop>();
                // cmd = "SELECT \"shopName\" from tbl_shops WHERE \"isDeleted\"=\'0\' and \"shopName\"='"+obj.shopName+"'";
                // appMenuShop = (List<Shop>)dapperQuery.QryResult<Shop>(cmd, _dbCon);

                // if (appMenuShop.Count > 0)
                //     name = appMenuShop[0].shopName;

                int rowAffected = 0;
                var response = "";

                
                if(obj.productID != 0)
                {
                    cmd3 = "DELETE FROM favorite where \"userID\"="+obj.userID+" and \"productID\"="+obj.productID+"";
                }
                else
                {
                    found = true;
                }
            

                if (found == false)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected = con.Execute(cmd3);
                    }

                }

                if (rowAffected > 0)
                {
                    response = "Success";
                    return Ok(new { message = response });
                }
                else
                {
                    if (found == true)
                    {
                        response = "Shop not found";
                    }
                    else
                    {
                        response = "Server Issue";
                    }
                    return BadRequest(new { message = response });
                }

                
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

    }
}