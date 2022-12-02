using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class DailySales
    {
        public int invoiceNo { get; set; }
        public string invoiceDate { get; set; }
        public int productID { get; set; }
        public string productName { get; set; }
        public float qty { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
    }
}