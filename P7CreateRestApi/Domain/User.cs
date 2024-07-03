using Microsoft.AspNetCore.Identity;

namespace P7CreateRestApi.Domain
{
    public class User : IdentityUser<int>
    {     
        public string Fullname { get; set; }
        public string Role { get; set; }
    }
}