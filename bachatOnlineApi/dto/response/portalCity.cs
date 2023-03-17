using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class PortalCity
    {
        public int cityID { get; set; }
        public string cityName { get; set; }
        public int isDeleted { get; set; }
    }
}