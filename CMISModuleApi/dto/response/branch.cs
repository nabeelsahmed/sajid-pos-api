using System;
using System.Text.Json.Serialization;


namespace CMISModuleApi.Entities
{
    public class Branch
    {
        public int branchID { get; set; }
        public string branchName { get; set; }
        public int companyID { get; set; }
    }
}