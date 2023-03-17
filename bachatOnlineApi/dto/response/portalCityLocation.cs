using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class PortalCityLocation
    {
        public int cityID { get; set; }
        public int locationID { get; set; }
        public string locationName { get; set; }
        public string cityName { get; set; }
        public int isDeleted { get; set; }
    }
}