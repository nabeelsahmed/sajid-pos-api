using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachatOnlineModuleApi.Entities
{
    public class DeliveryChargesCreation
    {
        public int deliveryChargesID { get; set; }
        public string deliveryChargesDate { get; set; }
        public float amount { get; set; }
        public string description { get; set; }
    }
}