using System;
using System.Text.Json.Serialization;


namespace posCoreModuleApi.Entities
{
    public class Route
    {
        public int rootID { get; set; }
        public string rootName { get; set; }
        public int isDeleted { get; set; }
    }
}