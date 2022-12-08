using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class OverAllStock
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public float IN { get; set; }
        public float OUT { get; set; }
    }
}