using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class PortalPriceCreation
    {
        public int pPriceID { get; set; }
        public int userID { get; set; }
        public float salePrice { get; set; }
        public float availableqty { get; set; }
    }
}