using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Purchase
    {
        public int invoiceNo { get; set; }
        public string invoiceDate { get; set; }
        public int partyID { get; set; }
        public int bankID { get; set; }
        public int refInvoiceNo { get; set; }
        public string refInvoiceDate { get; set; }
        public float cashReceived { get; set; }
        public float discount { get; set; }
        public float change { get; set; }
        public string fbrInvoiceNo { get; set; }
        public string fbrCode { get; set; }
        public string fbrResponse { get; set; }
        public int approvedBy { get; set; }
        public string approvedOn { get; set; }
        public string creditCardNo { get; set; }
        public string description { get; set; }
        public int bookerID { get; set; }
        public int branchID { get; set; }
        public int outletid { get; set; }
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