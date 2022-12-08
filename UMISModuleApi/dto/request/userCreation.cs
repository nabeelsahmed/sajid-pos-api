using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMISModuleApi.Entities
{
    public class UserCreation
    {
        public int userID { get; set; }
        public string empName { get; set; }
        public string loginName { get; set; }
        public string Password { get; set; }
        public int outletid { get; set; }
        public int roleId { get; set; }
    }
}