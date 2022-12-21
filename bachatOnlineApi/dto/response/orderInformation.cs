using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachatOnlineModuleApi.Entities
{
    public class OrderInformation
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public int orderID { get; set; }
        public string customerName { get; set; }
        public string applicationedoc { get; set; }
        public string status { get; set; }
        public string mobile { get; set; }
        public int qty { get; set; }
        public string price { get; set; }
        public string address { get; set; }
        public string orderDate { get; set; }
    }
}