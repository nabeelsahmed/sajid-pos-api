using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class RouteCreation
    {
        public int rootID { get; set; }
        public string rootName { get; set; }
        public int userID { get; set; }
    }
}