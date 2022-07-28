using System.Collections.Generic;
using gatewayapi.Entities;

namespace gatewayapi.Models
{
    public class AuthenticateResponse
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string loginName { get; set; }
        public string userType { get; set; }
        public string region { get; set; }
        public string token { get; set; }

        public AuthenticateResponse(List<User> user, string userToken)
        {
            id = user[0].id;
            fullName = user[0].fullName;
            loginName = user[0].loginName;
            userType = user[0].userType;
            region = user[0].region;
            token = userToken;
        }
    }
}