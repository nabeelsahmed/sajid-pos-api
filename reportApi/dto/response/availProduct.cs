using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class AvailProduct
    {
        public string productName { get; set; }
        public float availableqty { get; set; }
        public float inventorysenthm { get; set; }
        public float saleqty { get; set; }
        public float returnohm { get; set; }
    }
}