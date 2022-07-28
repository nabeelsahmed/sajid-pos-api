using System.ComponentModel.DataAnnotations;

namespace gatewayapi.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }
    }
}