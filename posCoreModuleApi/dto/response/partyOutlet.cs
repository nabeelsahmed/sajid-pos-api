using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace posCoreModuleApi.Entities
{
    public class partyOutlet
    {
        public int partyID { get; set; }
        public string partyName { get; set; }
        public int outletid { get; set; }
        public string type { get; set; }
    }
}