using Ecommerce.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Ecommerce.Services
{


    //Interface:
    public interface IAuthenticationService
    {
        string GenerateJwtToken(RegisterDTO user);
    }


    //Class:
    public class AuthenticationService: IAuthenticationService
    {
        private readonly IConfiguration _configuration;  //IConfiguration interface used to access configuration settings from appsettings
        //It is made private to encapsulate the configuration data and ensure that it is only accessed through the class's methods  
        
        public AuthenticationService(IConfiguration configuration) => _configuration = configuration; // when a class has a private field it needs to be initialized, typically through the constructor to be used later within the class


        //Method to generate token by asking username and password:
        public string GenerateJwtToken(RegisterDTO user) { 
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
                 {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.Phone),
                new Claim(ClaimTypes.StreetAddress, user.Address)
            };
            var token = new JwtSecurityToken(
               issuer: _configuration["Jwt:Issuer"],
               audience: _configuration["Jwt:Audience"],
               claims: claims,
               expires: DateTime.Now.AddMinutes(300),
               signingCredentials: credentials
           );
           return new JwtSecurityTokenHandler().WriteToken(token);

        }


  }
}
