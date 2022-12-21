using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachatOnlineModuleApi.Entities
{
    public class Notification
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public int orderID { get; set; }
        public string customerName { get; set; }
        public string applicationedoc { get; set; }
        public string status { get; set; }
        public string mobile { get; set; }
    }
}