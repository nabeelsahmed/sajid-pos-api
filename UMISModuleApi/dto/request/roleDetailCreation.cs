using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMISModuleApi.Entities
{
    public class RoleDetailCreation
    {
        public int roleDetailID { get; set; }
        public int menuId { get; set; }
        public int read { get; set; }
        public int write { get; set; }
        public int delete { get; set; }
    }
}