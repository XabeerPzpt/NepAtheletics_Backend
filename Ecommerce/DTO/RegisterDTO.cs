using Ecommerce.DTO;
using System.Net;

namespace Ecommerce.DTO
{ 
    public class RegisterDTO
    { 
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
        public string Email { get; set; }
        public string Address { get; set; }  
        public string Phone { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Profile_Image_URL { get; set; }
    }
    
    public class LoginDTO 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Common
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
}

public class RegisterList : Common
{
    public IQueryable<RegisterDTO> registers { get; set; }
    public string role { get; set; }    
    public string Token { get; set; }
    //public string refreshToken { get; set; }
}