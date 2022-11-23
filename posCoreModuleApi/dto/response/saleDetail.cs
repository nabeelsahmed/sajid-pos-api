using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class SaleDetail
    {
        public int invoiceNo { get; set; }
        public int invoiceDetailID { get; set; }
        public int productID { get; set; }
        public int locationID { get; set; }
        public float qty { get; set; }
        public int schemeQty { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float debit { get; set; }
        public float credit { get; set; }
        public float productdiscount { get; set; }
        public string expiryDate { get; set; }
        public string mfgDate { get; set; }
        public int batchNo { get; set; }
        public string batchStatus { get; set; }
        public float productQty { get; set; }
        public string productName { get; set; }
        public int coaID { get; set; }
        public float laborcost { get; set; }
        public int noofboxes { get; set; }
        public float freightcharges { get; set; }
    }
}