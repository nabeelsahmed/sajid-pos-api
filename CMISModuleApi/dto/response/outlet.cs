using System;
using System.Text.Json.Serialization;


namespace CMISModuleApi.Entities
{
    public class outlet
    {
        public int outletID { get; set; }
        public string outletName { get; set; }
        public string outletShortName { get; set; }
        public string outletAddress { get; set; }
        public string contactPerson { get; set; }
        public string phoneNo { get; set; }
        public string mobileNo { get; set; }
        public string email { get; set; }

    }
}