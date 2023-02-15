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
    public class ProductController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3, cmd4, cmd5;

        public ProductController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getTestData")]
        public IActionResult getTestData()
        {
            return Ok("OK");
        }

        [HttpGet("getAvailProduct")]
        public IActionResult getAvailProduct(int outletID,int categoryID)
        {
            try
            {
                if (categoryID == 0)
                {
                    cmd = "select * from \"view_saleAvailableProduct\" where outletid = " + outletID + " and \"parentProductID\" is not null and \"parentProductID\" != 0;";   
                }
                else
                {
                    cmd = "select * from \"view_saleAvailableProduct\" where \"categoryID\" = " + categoryID + " and outletid = " + outletID + " and \"parentProductID\" is not null and \"parentProductID\" != 0;";
                }
                var appMenu = dapperQuery.Qry<AvailProduct>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getDeliveryCharges")]
        public IActionResult getDeliveryCharges(int deliveryChargesID)
        {
            try
            {
                if (deliveryChargesID == 0)
                {
                    cmd = "select * from \"tbl_deliveryCharges\" where \"isDeleted\" = B'0'";    
                }
                else
                {
                    cmd = "select * from \"tbl_deliveryCharges\" where \"deliveryChargesID\" = " + deliveryChargesID + "";
                }

                var appMenu = dapperQuery.Qry<DeliveryCharges>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getPortalAvailProduct")]
        public IActionResult getPortalAvailProduct(int outletID)
        {
            try
            {
                cmd = "select * from \"view_portalSaleAvailableProduct\" where outletid = " + outletID + " and \"parentProductID\" is not null and \"parentProductID\" != 0;";
                var appMenu = dapperQuery.Qry<AvailProduct>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getOnlineProduct")]
        public IActionResult getOnlineProduct()
        {
            try
            {
                cmd = "select * from public.\"view_storeProducts\"";
                var appMenu = dapperQuery.QryResult<Product>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getCheckQty")]
        public IActionResult getCheckQty(int pPriceID, int qty)
        {
            try
            {
                cmd = "SELECT availableqty FROM \"productPrice\" where \"pPriceID\" = '" + pPriceID + "' AND \"isDeleted\"::int = 0";
                var appMenu = (List<Product>)dapperQuery.QryResult<Product>(cmd, _dbCon);

                var availQty = appMenu[0].availableqty - qty;

                if (availQty >= 0)
                {
                    return Ok(new { msg = true });
                }
                else
                {
                    return Ok(new { msg = false, qty = appMenu[0].availableqty });
                }
                // return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getPlaceOrder")]
        public IActionResult getPlaceOrder()
        {
            try
            {
                cmd = "select * from public.\"Order\" where status = 'pend' OR status is null or status = '' order by \"orderDate\" Desc";
                var appMenu = dapperQuery.QryResult<Order>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getOrderDetail")]
        public IActionResult getOrderDetail(int orderID,int userID)
        {
            try
            {
                cmd = "select * from public.\"view_OrderDetail\" where \"orderID\" = " + orderID + " ";
                var appMenu = dapperQuery.QryResult<OrderInformation>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getAllOrder")]
        public IActionResult getAllOrder(int userID)
        {
            try
            {
                cmd = "select * from public.\"view_OrderDetail\" where \"createdBy\" = " + userID + "";
                var appMenu = dapperQuery.QryResult<OrderInformation>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpGet("getOrderHistory")]
        public IActionResult getOrderHistory(string fromDate, string toDate)
        {
            try
            {
                cmd = "select * from public.\"Order\" where status = 'comp' AND \"orderDate\" between '" + fromDate + "' AND '" + toDate + "'";
                var appMenu = dapperQuery.QryResult<Order>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getPlacedNotification")]
        public IActionResult getPlacedNotification(int userID)
        {
            try
            {
                cmd = "select * from public.\"view_notify\" where \"createdBy\"="+userID+"";
                var appMenu = dapperQuery.QryResult<Notification>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("checkout")]
        public IActionResult checkout(OrderCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                var response = "";
                List<Order> appMenuOrder = new List<Order>();

                cmd = "insert into public.\"Order\" (\"orderDate\", \"customerName\", \"email\", \"mobile\", \"address\",\"status\", \"createdOn\", \"isDeleted\") values ('" + curDate + "', '" + obj.customerName + "', '" + obj.email + "', '" + obj.mobile + "', '" + obj.address + "','' ,'" + curDate + "', B'0')";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                //confirmation of data saved in order table
                if (rowAffected > 0)
                {

                    // //getting last saved invoice no
                    cmd2 = "SELECT \"orderID\" FROM public.\"Order\" order by \"orderID\" desc limit 1";
                    appMenuOrder = (List<Order>)dapperQuery.QryResult<Order>(cmd2, _dbCon);

                    var orderID = appMenuOrder[0].orderID;

                    //convert string to json data to insert in invoice detail table
                    var invObject = JsonConvert.DeserializeObject<List<OrderDetailCreation>>(obj.json);


                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        // cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"locationID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.locationID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', 0, '" + item.qty * item.salePrice + "', '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0')";
                        cmd3 = "INSERT INTO public.\"OrderDetail\"(\"orderID\", \"productID\", \"productName\", qty, price, \"createdOn\",\"createdBy\", \"isDeleted\") VALUES ('" + orderID + "', '" + item.productID + "',  '" + item.productName + "',  '" + item.qty + "',  '" + item.salePrice + "', '" + curDate + "', " + item.userID + " , B'0')";
                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd3);
                        }

                        // cmd4 = "SELECT \"invoiceDetailID\", qty FROM \"invoiceDetail\" where \"invoiceDetailID\" = '" + item.invoiceDetailID + "' AND \"isDeleted\"::int = 0";
                        // var appMenu = (List<Product>)dapperQuery.QryResult<Product>(cmd4, _dbCon);

                        var availQty = (item.availQty - item.qty);

                        cmd5 = "update public.\"productPrice\" set availableqty = " + availQty + " where \"pPriceID\" = " + item.pPriceID + ";";


                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected3 = con.Execute(cmd5);
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

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("placeOrder")]
        public IActionResult placeOrder(OrderCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                var response = "";
                //var orderObj = "";

                List<OrderDetailCreation> appMenuOrder = new List<OrderDetailCreation>();

                cmd = "insert into public.\"invoice\" (\"invoiceDate\", \"discount\", \"invoiceType\", \"orderID\",\"isDeleted\",\"invoicetime\") values ('" + curDate + "',0,'OS'," + obj.orderID + ", B'0','" + curTime + "')";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                cmd2 = "update public.\"Order\" set status = '" + obj.status + "' where \"orderID\" = " + obj.orderID + ";";

                // cmd = "insert into public.\"Order\" (\"orderDate\", \"customerName\", \"email\", \"mobile\", \"address\", \"createdOn\", \"isDeleted\") values ('" + obj.orderDate + "', '" + obj.customerName + "', " + obj.email + ", " + obj.mobile + ", '" + obj.address + "', '" + curDate + "', B'0')";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected2 = con.Execute(cmd2);
                }

                cmd3 = "SELECT \"invoiceNo\" FROM public.\"invoice\" where \"orderID\" = " + obj.orderID + "";
                    appMenuOrder = (List<OrderDetailCreation>)dapperQuery.QryResult<OrderDetailCreation>(cmd3, _dbCon);

                    var invoiceNo = appMenuOrder[0].invoiceNo;

                if (rowAffected > 0 && rowAffected2 > 0)
                {   

                    //convert string to json data to insert in invoice detail table
                    cmd3 = "select array_to_json(array_agg(row_to_json(\"OrderDetail\"))) as json from ( select rd.* from \"OrderDetail\" rd where rd.\"orderID\" = " + obj.orderID + ") \"OrderDetail\"";

                    // using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    //     {
                    //         orderObj = con.Execute(cmd3);
                    //     }
                    List<OrderCreation> appMenuInvoice = new List<OrderCreation>();

                    appMenuInvoice = (List<OrderCreation>)dapperQuery.Qry<OrderCreation>(cmd3, _dbCon);
                    
                    var orderObj = appMenuInvoice[0].json;

                    var invObject = JsonConvert.DeserializeObject<List<OrderDetailCreation>>(orderObj);

                    //saving json data one by one in invoice detail table
                    foreach (var item in invObject)
                    {
                        
                    
                        // cmd3 = "insert into public.\"invoiceDetail\" (\"invoiceNo\", \"productID\", \"locationID\", \"qty\", \"costPrice\", \"salePrice\", \"debit\", \"credit\", \"discount\", \"productName\", \"coaID\", \"createdOn\", \"createdBy\", \"isDeleted\") values ('" + invoiceNo + "', '" + item.productID + "', '" + item.locationID + "', '" + item.qty + "', '" + item.costPrice + "', '" + item.salePrice + "', 0, '" + item.qty * item.salePrice + "', '" + item.discount + "', '" + item.productName + "', '1', '" + curDate + "', " + obj.userID + ", B'0')";
                        if (item.productID > 0)
                        {
                            cmd4 = "INSERT INTO public.\"invoiceDetail\"(\"invoiceNo\", \"productID\", \"qty\",\"salePrice\", \"productQty\",\"productName\",\"createdOn\",\"createdBy\",\"isDeleted\") VALUES (" + invoiceNo + "," + item.productID + "," + item.qty + ",'" + item.salePrice + "'," + item.qty + ",'" + item.productName + "','" + curDate + "'," + item.userID + ",B'0')";
                        }
                        // else
                        // {
                        //     cmd4 = "INSERT INTO public.\"invoiceDetail\"(\"invoiceNo\",\"salePrice\",\"createdOn\",\"createdBy\",\"isDeleted\",\"deliveryChargesID\") VALUES (" + invoiceNo + ",'" + item.salePrice + "','" + curDate + "'," + item.userID + ",B'0'," + item.deliveryChargesID + ")";
                        // }
                        
                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected3 = con.Execute(cmd4);
                        }
                    }

                    cmd4 = "INSERT INTO public.\"invoiceDetail\"(\"invoiceNo\",\"salePrice\",\"createdOn\",\"isDeleted\") VALUES (" + invoiceNo + ",'" + obj.deliveryCharges + "','" + curDate + "',B'0')";
                    
                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected3 = con.Execute(cmd4);
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

        [HttpPost("acceptOrDeclineOrder")]
        public IActionResult acceptOrDeclineOrder(OrderCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                var response = "";

                cmd = "update public.\"Order\" set status = '" + obj.status + "' where \"orderID\" = " + obj.orderID + ";";

                // cmd = "insert into public.\"Order\" (\"orderDate\", \"customerName\", \"email\", \"mobile\", \"address\", \"createdOn\", \"isDeleted\") values ('" + obj.orderDate + "', '" + obj.customerName + "', " + obj.email + ", " + obj.mobile + ", '" + obj.address + "', '" + curDate + "', B'0')";

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

        [HttpPost("cancelOrder")]
        public IActionResult cancelOrder(OrderCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                DateTime curTime = DateTime.Now;

                var time = curTime.ToString("HH:mm");

                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";

                cmd = "update public.\"Order\" set \"status\" = 'cancl' where \"orderID\" = " + obj.orderID + ";";

                // cmd = "insert into public.\"Order\" (\"orderDate\", \"customerName\", \"email\", \"mobile\", \"address\", \"createdOn\", \"isDeleted\") values ('" + obj.orderDate + "', '" + obj.customerName + "', " + obj.email + ", " + obj.mobile + ", '" + obj.address + "', '" + curDate + "', B'0')";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                    
                if (rowAffected > 0)
                {
                    cmd2 = "SELECT * FROM public.\"OrderDetail\" where \"orderID\" = " + obj.orderID + ";";
                    var appMenuOrder = (List<OrderDetailCreation>)dapperQuery.QryResult<OrderDetailCreation>(cmd2, _dbCon);

                    // //convert string to json data to insert in invoice detail table
                    // var orderDetailObject = JsonConvert.DeserializeObject<List<OrderDetailCreation>>(appMenuOrder);


                    //saving json data one by one in invoice detail table
                    foreach (var item in appMenuOrder)
                    {
                        cmd2 = "select availableqty from \"productPrice\" where \"productID\" = " + item.productID + " and outletid = 1";
                        var appMenuQty = (List<AvailProduct>)dapperQuery.QryResult<AvailProduct>(cmd2, _dbCon);

                        var availableQty = appMenuQty[0].availableqty;

                        var availQty = (availableQty + item.qty);

                        cmd5 = "update public.\"productPrice\" set availableqty = " + availQty + " where \"productID\" = " + item.productID + " and outletid = 1 ";


                        using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                        {
                            rowAffected2 = con.Execute(cmd5);
                        }

                    }

                    if(rowAffected2 > 0){
                        response = "Success";
                    }
                    else
                    {
                        response = "Order did not exists";
                    }
                }
                else
                {
                    response = "Order did not exists";
                }

                return Ok(new { message = response });
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getRecommended")]
        public IActionResult getRecommended()
        {
            try
            {
                cmd = "SELECT * FROM public.\"view_recommended\"";
                var appMenu = dapperQuery.QryResult<ProductDetail>(cmd, _dbCon);

                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("saveRecommended")]
        public IActionResult saveRecommended(int productID,int isRecommended)
        {
            try
            {
                int rowAffected = 0;
                var response = "";

                cmd = "update public.\"product\" set \"isrecommended\"="+isRecommended+" where \"productID\" = " + productID + " and \"isDeleted\"=B'0'";

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


        [HttpPost("saveDeliveryCharges")]
        public IActionResult saveDeliveryCharges(DeliveryChargesCreation obj)
        {
            try
            {
                int rowAffected = 0;
                var response = "";
                int newDeliveryChargesID = 0;

                List<DeliveryCharges> appMenuProduct = new List<DeliveryCharges>();
                cmd = "select \"deliveryChargesID\" from \"tbl_deliveryCharges\" ORDER BY \"deliveryChargesID\" DESC LIMIT 1";
                appMenuProduct = (List<DeliveryCharges>)dapperQuery.QryResult<DeliveryCharges>(cmd, _dbCon);

                if(appMenuProduct.Count == 0)
                    {
                        newDeliveryChargesID = 1;
                    }else{
                        newDeliveryChargesID = appMenuProduct[0].deliveryChargesID+1;
                    }

                cmd = "INSERT INTO public.\"tbl_deliveryCharges\"(\"deliveryChargesID\",\"deliveryChargesDate\",\"amount\",\"description\",\"isDeleted\") VALUES (" + newDeliveryChargesID + ",'" + obj.deliveryChargesDate + "','" + obj.amount + "','" + obj.description + "',B'0')";

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


        [HttpPost("deleteDeliveryCharges")]
        public IActionResult deleteDeliveryCharges(DeliveryChargesCreation obj)
        {
            try
            {
                int rowAffected = 0;
                var response = "";
                int newDeliveryChargesID = 0;

                // List<DeliveryCharges> appMenuProduct = new List<DeliveryCharges>();
                // cmd = "select \"deliveryChargesID\" from \"tbl_deliveryCharges\" ORDER BY \"deliveryChargesID\" DESC LIMIT 1";
                // appMenuProduct = (List<DeliveryCharges>)dapperQuery.QryResult<DeliveryCharges>(cmd, _dbCon);

                // if(appMenuProduct.Count == 0)
                //     {
                //         newDeliveryChargesID = 1;
                //     }else{
                //         newDeliveryChargesID = appMenuProduct[0].deliveryChargesID+1;
                //     }

                cmd = "update public.\"tbl_deliveryCharges\" set \"isDeleted\" = B'0' where \"deliveryChargesID\" = " + obj.deliveryChargesID + ";";

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