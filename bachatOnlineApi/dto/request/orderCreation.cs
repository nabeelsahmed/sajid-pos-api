using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class OrderCreation
    {
        public int orderID { get; set; }
        public string orderDate { get; set; }
        public string customerName { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string status { get; set; }
        public string json { get; set; }
    }
}