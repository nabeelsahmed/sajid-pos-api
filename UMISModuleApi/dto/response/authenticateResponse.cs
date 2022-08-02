using System.Collections.Generic;
using UMISModuleAPI.Entities;

namespace UMISModuleAPI.Models
{
    public class AuthenticateResponse
    {
        public int userLoginId { get; set; }
        public int roleId { get; set; }

        public string fullName { get; set; }
        public string loginName { get; set; }
        public string password { get; set; }
        public string isPinCode { get; set; }
        public string token { get; set; }
        public int outletid { get; set; }


        public AuthenticateResponse(List<User> user, string userToken)
        {
            userLoginId = user[0].userLoginId;
            roleId = user[0].roleId;
            fullName = user[0].fullName;
            loginName = user[0].loginName;
            password = user[0].password;
            isPinCode = user[0].isPinCode;
            outletid = user[0].outletid;
            token = userToken;
        }
    }
}