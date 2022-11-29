using System;
using System.Text.Json.Serialization;


namespace reportApi.Entities
{
    public class CurrentStock
    {
        public string productName { get; set; }
        public float openingbalance { get; set; }
        public float saleqty { get; set; }
    }
}