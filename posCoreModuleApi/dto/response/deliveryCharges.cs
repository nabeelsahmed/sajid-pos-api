using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class DeliveryCharges
    {
        public int deliveryChargesID { get; set; }
        public float amount { get; set; }
        public string description { get; set; }
    }
}