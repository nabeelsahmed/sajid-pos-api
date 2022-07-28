using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMISModuleApi.dto.request
{
    public class outletCreation
    {
        public int outletID { get; set; }
        public string outletName { get; set; }
        public string outletShortName { get; set; }
        public string outletAddress { get; set; }
        public string contactPerson { get; set; }
        public string phoneNo { get; set; }
        public string mobileNo { get; set; }
        public string email { get; set; }
        public int userID { get; set; }
    }
}