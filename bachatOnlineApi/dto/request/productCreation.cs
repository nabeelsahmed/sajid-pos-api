using System;
using System.Text.Json.Serialization;


namespace bachatOnlineModuleApi.Entities
{
    public class ProductCreation
    {
        public string id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string inventory { get; set; }
        // public string picEDoc { get; set; }
        // public string picEDocPath { get; set; }
        // public string picEdocExtenstion { get; set; }
    }
}