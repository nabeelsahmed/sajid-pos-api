using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachatOnlineModuleApi.Entities
{
    public class ProductDetail
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public float salePrice { get; set; }
        public string applicationedoc { get; set; }
        public string categoryName { get; set; }
        public int invoiceNo { get; set; }
        public int invoiceDetailID { get; set; }
        public int qty { get; set; }
        public int isrecommended { get; set; }
        public int availQty { get; set; }
        public string productdetail { get; set; }
    }
}