using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class WeightLossCreation
    {
        public int invoiceNo { get; set; }
        public int productID { get; set; }
        public int qty { get; set; }
        public float costPrice { get; set; }
        public float salePrice { get; set; }
        public float laborCost { get; set; }
        public float freightCharges { get; set; }
        public int userID { get; set; }
    }
}