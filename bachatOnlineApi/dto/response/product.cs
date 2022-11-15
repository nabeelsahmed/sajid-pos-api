using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class Product
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public float salePrice { get; set; }
        public string applicationedoc { get; set; }
        public string categoryName { get; set; }
        public int invoiceNo { get; set; }
        public int invoiceDetailID { get; set; }
        public int qty { get; set; }
        // public int availQty { get; set; }
        
        // public int productID { get; set; }
        // public string productName { get; set; }
        // public string applicationedoc { get; set; }
        public int pPriceID { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float availableqty { get; set; }
        public int outletid { get; set; }
        // public string invoiceDate { get; set; }
    }
}