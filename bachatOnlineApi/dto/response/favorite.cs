using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachatOnlineModuleApi.Entities
{
    public class Favorite
    {
        public int favoriteID { get; set; }
        public int userID { get; set; }
        public int productID { get; set; }
    }
}