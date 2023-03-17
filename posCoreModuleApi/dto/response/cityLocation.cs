using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CityLocation
    {
        public int cityID { get; set; }
        public int locationID { get; set; }
        public string locationName { get; set; }
        public string cityName { get; set; }
        public int isDeleted { get; set; }
    }
}