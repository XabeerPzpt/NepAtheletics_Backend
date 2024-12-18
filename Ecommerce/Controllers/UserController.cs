using Microsoft.AspNetCore.Mvc;
using Ecommerce.Repository;
using Ecommerce.Entities;
using Ecommerce.DTO;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Controllers 
{

    [Route("api/Users")]
    [ApiController]
    [Authorize]

    //This is user controller: 
    public class UserController : ControllerBase
    {
        public readonly ICompanyRepository _repository;
        public UserController(ICompanyRepository repository) => _repository = repository;


        //Get all Users
        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repository.GetUsers();
            return Ok(users);
        }

        //Get all Users For Admin
        [HttpGet("getUsersForAdmin")]
        public async Task<IActionResult> GetUsersForAdmin()
        {
            var users = await _repository.GetUsersForAdmin();
            return Ok(users);
        }


        //Register new user
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUser(Users user)
        {
            try
            {
                var result = await _repository.AddUser(user);
                if (result.Code == "409")
                {
                    return BadRequest(new
                    {
                        Code = "409",
                        Message = "User already exists!",
                    });
                }
                else if (result.Code == "400")
                {
                    return BadRequest(new
                    {
                        Code = "400",
                        Message = "Failed to register user!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Code = "000",
                        Message = "Registered succesfully",
                        Token = result.Token
                    });
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Code = "500",
                    Message = message
                });
            }
        }




        // get user by token
        [HttpGet]
        [Route("userprofile")]
        public async Task<IActionResult> GetUser()
        {
            var result = await _repository.GetUser();
            return (result.StatusCode == HttpStatusCode.OK)? Ok(new
            {
                Code = result.StatusCode,
                Message = result.StatusMessage,
                User = result.registers
            }) :  NotFound(new
            {
                Code = result.StatusCode,
                Message = result.StatusMessage
            });

            
        }





        //For Login:
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginDTO user)

        {
            RegisterList result = await _repository.Authenticate(user);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(new
                {
                    Code = result.StatusCode,
                    Message = result.StatusMessage,
                    Token = result.Token,
                    Role = result.role
                });
            }
            else
            {
                return Unauthorized(new
                {
                    Code = result.StatusCode,
                    Message = result.StatusMessage
                });
            }
        }



        // Password Reset
        [HttpPut("reset_password")]
        public async Task<IActionResult> ChangePassword(PasswordChangeDTO passwords)
        {
                var result = await _repository.ChangePassword(passwords);
                if (result.Code == "401")
                    return Ok(new
                    {
                        Code = "401",
                        Message = "Old password donot match"
                    });
                else if (result.Code == StatusMapper.PassWordRepeat)
                    return Ok(new
                    {
                        Code = "409",
                        Message = "New Password is the same as old password"
                    });
                else if (result.Code == StatusMapper.TechinicalErrror)
                    return Ok(new
                    {
                        Code = "400",
                        Message = "Something went wrong"
                    });
                else
                    return Ok(new
                    {
                        Code = "000",
                        Message = "Password changed successfully"
                    });
          
        }




        // Get Total Users Per Month 
        [HttpGet("getUsersPerMonth/{year}")]
        public async Task<IActionResult> GetUserPerMonth(int year)
        {
            var total_users = await _repository.GetTotalUsersPerMonth(year);
            return Ok(total_users);
        }


    }








}



