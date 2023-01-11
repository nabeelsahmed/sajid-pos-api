using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class AvailProduct
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public string applicationedoc { get; set; }
        public int pPriceID { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float availableqty { get; set; }
        public int outletid { get; set; }
        public string invoiceDate { get; set; }
        public int categoryID { get; set; }
        public string categoryName { get; set; }
    }
}