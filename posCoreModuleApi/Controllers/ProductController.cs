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
    public class ProductController : ControllerBase
    {
        private readonly IOptions<conStr> _dbCon;
        private string cmd, cmd2, cmd3, cmd4, cmd5;

        public ProductController(IOptions<conStr> dbCon)
        {
            _dbCon = dbCon;
        }

        [HttpGet("getProduct")]
        public IActionResult getProduct()
        {
            try
            {
                cmd = "SELECT * FROM view_product order by \"productID\" desc";
                var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getProductParent")]
        public IActionResult getProductParent()
        {
            try
            {
                cmd = "SELECT * FROM \"view_allProduct\" where \"parentProductID\" is null order by \"productID\" desc";
                var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getAllProduct")]
        public IActionResult getAllProduct()
        {
            try
            {
                cmd = "SELECT * FROM \"view_allProduct\" order by \"productID\" desc";
                var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpGet("getAvailProduct")]
        public IActionResult getAvailProduct(int outletID)
        {
            try
            {
                cmd = "select * from \"view_saleAvailableProduct\" where outletid = " + outletID + ";";
                var appMenu = dapperQuery.Qry<AvailProduct>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        // [HttpGet("getTopTKProducts")]
        // public IActionResult getTopTKProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc offset 1200 limit 2500";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        // [HttpGet("getTopOKProducts")]
        // public IActionResult getTopOKProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc  offset 600 limit 1200";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        // [HttpGet("getTopFHProducts")]
        // public IActionResult getTopFHProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc offset 100 limit 600";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        // [HttpGet("getTopProducts")]
        // public IActionResult getTopProducts()
        // {
        //     try
        //     {
        //         cmd = "SELECT * FROM view_product order by \"productID\" desc limit 100";
        //         var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
        //         return Ok(appMenu);
        //     }
        //     catch (Exception e)
        //     {
        //         return Ok(e);
        //     }

        // }

        [HttpGet("getProductByCategory")]
        public IActionResult getProductByCategory(int categoryID)
        {
            try
            {
                cmd = "SELECT * FROM view_product where \"categoryID\" = " + categoryID + " order by \"productID\" desc";
                var appMenu = dapperQuery.Qry<Product>(cmd, _dbCon);
                return Ok(appMenu);
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveProduct")]
        public IActionResult saveProduct(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                var response = "";
                List<Product> appMenuProduct = new List<Product>();
                List<Product> appMenuBarcode = new List<Product>();
                var found = false;
                var productName = "";

                cmd2 = "select \"productName\" from product where \"isDeleted\"::int = 0 AND \"productName\" = '" + obj.productName + "'";
                appMenuProduct = (List<Product>)dapperQuery.QryResult<Product>(cmd2, _dbCon);

                if (appMenuProduct.Count > 0)
                    productName = appMenuProduct[0].productName;

                // var colorID = "";
                // var sizeID = "";
                // if(obj.colorID == 0){
                //     colorID = "";
                // }else{
                //     colorID = obj.colorID.ToString();
                // }
                // if(obj.sizeID == 0){
                //     sizeID = "";
                // }else{
                //     sizeID = obj.sizeID.ToString();
                // }
                // obj.applicationEDoc
                if (productName == "")
                {
                    cmd = "insert into public.product (\"uomID\", \"productName\", \"categoryID\", \"productNameUrdu\", \"ROL\", \"maxLimit\", \"quickSale\", \"pctCode\", \"applicationedoc\", \"parentProductID\", \"createdOn\", \"createdBy\", \"isDeleted\") values (" + obj.uomID + ", '" + obj.productName + "', '" + obj.categoryID + "', '" + obj.productNameUrdu + "', '" + obj.reOrderLevel + "', '" + obj.maxLimit + "', '" + obj.quickSale + "', '" + obj.pctCode + "', '" + obj.applicationEDoc + "', '" + obj.parentProductID + "', '" + curDate + "', " + obj.userID + ", B'0')";
                }
                else
                {
                    found = true;
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
                    cmd2 = "SELECT \"productID\" FROM public.product order by \"productID\" desc limit 1";
                    appMenuProduct = (List<Product>)dapperQuery.QryResult<Product>(cmd2, _dbCon);

                    var prodID = appMenuProduct[0].productID;

                    if (obj.barcode1 == "")
                    {

                        cmd3 = "select \"barcodeID\" from public.barcode order by \"barcodeID\" desc limit 1";
                        appMenuBarcode = (List<Product>)dapperQuery.QryResult<Product>(cmd3, _dbCon);

                        var barcodeID = 0;
                        if (appMenuBarcode.Count > 0)
                        {
                            barcodeID = appMenuBarcode[0].barcodeID;
                        }
                        else
                        {
                            barcodeID = 1;
                        }

                        cmd4 = "INSERT INTO public.barcode(\"productID\", \"barcode1\", \"createdOn\", \"createdBy\", \"isDeleted\") values (" + prodID + ", '" + barcodeID + "', '" + curDate + "', " + obj.userID + ", B'0')";

                    }
                    else
                    {
                        cmd4 = "INSERT INTO public.barcode(\"productID\", \"barcode1\", \"barcode2\", \"barcode3\", \"createdOn\", \"createdBy\", \"isDeleted\") values (" + prodID + ", '" + obj.barcode1 + "', '" + obj.barcode2 + "', '" + obj.barcode3 + "', '" + curDate + "', " + obj.userID + ", B'0')";

                    }


                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd4);
                    }
                    // cmd5 = "insert into public.\"productPrice\" (\"productID\", \"costPrice\", \"salePrice\", \"retailPrice\", \"wholeSalePrice\", \"gst\", \"et\", \"packing\", \"packingSalePrice\", \"createdOn\", \"createdBy\", \"isDeleted\") values (" + prodID + ", " + obj.costPrice + ", " + obj.salePrice + ", " + obj.retailPrice + ", " + obj.wholeSalePrice + ", " + obj.gst + ", " + obj.et + ", " + obj.packingQty + ", " + obj.packingSalePrice + ", '" + curDate + "', " + obj.userID + ", B'0')";
                    // using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    // {
                    //     rowAffected3 = con.Execute(cmd5);
                    // }

                    if (obj.applicationEDocPath != null && obj.applicationEDocPath != "")
                    {
                        dapperQuery.saveImageFile(
                            obj.applicationEDoc,
                            prodID.ToString(),
                            obj.applicationEDocPath,
                            obj.applicationEdocExtenstion);
                    }
                }
                
                // if (rowAffected > 0 && rowAffected2 > 0 && rowAffected3 > 0)
                if (rowAffected > 0 && rowAffected2 > 0)
                {
                    response = "Success";
                }
                else
                {
                    if (found == true)
                    {
                        response = "Product name already exist";
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

        [HttpPost("updateProduct")]
        public IActionResult updateProduct(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                int rowAffected3 = 0;
                var response = "";

            if(obj.applicationEDoc == ""){
                cmd = "update product set \"uomID\" = " + obj.uomID + ", \"parentProductID\" = '" + obj.parentProductID + "', \"productName\" = '" + obj.productName + "', \"quickSale\" = '" + obj.quickSale + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";
            }else{
                cmd = "update product set \"uomID\" = " + obj.uomID + ", \"parentProductID\" = '" + obj.parentProductID + "', applicationedoc = '" + obj.applicationEDoc + "', \"productName\" = '" + obj.productName + "', \"quickSale\" = '" + obj.quickSale + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";
            }

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    cmd3 = "update public.barcode set \"barcode1\" = '" + obj.barcode1 + "', \"barcode2\" = '" + obj.barcode2 + "', \"barcode3\" = '" + obj.barcode3 + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"barcodeID\" = " + obj.barcodeID + ";";

                    using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    {
                        rowAffected2 = con.Execute(cmd3);
                    }

                    // cmd4 = "update public.\"productPrice\" set \"costPrice\" = '" + obj.costPrice + "', \"salePrice\" = '" + obj.salePrice + "', \"retailPrice\" = '" + obj.retailPrice + "', \"wholeSalePrice\" = '" + obj.wholeSalePrice + "', \"gst\" = '" + obj.gst + "', \"et\" = '" + obj.et + "', \"packing\" = '" + obj.packingQty + "', \"packingSalePrice\" = '" + obj.packingSalePrice + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"pPriceID\" = " + obj.pPriceID + ";";
                    // using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                    // {
                    //     rowAffected3 = con.Execute(cmd4);
                    // }
                }

                if (obj.applicationEDocPath != null && obj.applicationEDocPath != "")
                {
                    dapperQuery.saveImageFile(
                        obj.applicationEDoc,
                        obj.productID.ToString(),
                        obj.applicationEDocPath,
                        obj.applicationEdocExtenstion);
                }
                // if (rowAffected > 0 && rowAffected2 > 0 && rowAffected3 > 0)
                if (rowAffected > 0 && rowAffected2 > 0)
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

        [HttpPost("updateProductPrice")]
        public IActionResult updateProductPrice(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                int rowAffected2 = 0;
                var response = "";

                cmd = "update product set \"brandID\" = " + obj.brandID + " where \"productID\" = " + obj.productID + ";";
                    
                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                // cmd2 = "update public.\"productPrice\" set \"costPrice\" = '" + obj.costPrice + "', \"salePrice\" = '" + obj.salePrice + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";

                // using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                // {
                //     rowAffected2 = con.Execute(cmd2);
                // }

                // if (rowAffected > 0 && rowAffected2 > 0)
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

        [HttpPost("updateProductImage")]
        public IActionResult updateProductImage(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                var response = "";

                cmd = "update product set  \"applicationedoc\" = '" + obj.applicationEDocPath + "', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";

                using (NpgsqlConnection con = new NpgsqlConnection(_dbCon.Value.dbCon))
                {
                    rowAffected = con.Execute(cmd);
                }

                if (rowAffected > 0)
                {
                    dapperQuery.saveImageFile(
                        obj.applicationEDocPath,
                        obj.productID.ToString(),
                        obj.applicationEDoc,
                        obj.applicationEdocExtenstion);
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

        [HttpPost("deleteProduct")]
        public IActionResult deleteProduct(ProductCreation obj)
        {
            try
            {
                DateTime curDate = DateTime.Today;
                int rowAffected = 0;
                var response = "";

                cmd = "update product set \"isDeleted\" = B'1', \"modifiedOn\" = '" + curDate + "', \"modifiedBy\" = " + obj.userID + " where \"productID\" = " + obj.productID + ";";

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
    }
}