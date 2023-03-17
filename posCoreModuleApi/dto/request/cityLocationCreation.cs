using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class CityLocationCreation
    {
        public int cityID { get; set; }
        public int locationID { get; set; }
        public string locationName { get; set; }
        public int userID { get; set; }
    }
}