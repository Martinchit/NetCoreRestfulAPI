using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Api.Data;
using Api.Models;
using Api.Models.UserDetailsUpdate;
using AuthenticationPlugin;
using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {

        private ApiDbContext _apiDbContext;
        private readonly ILogger<AuthController> _logger;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public AuthController(ILogger<AuthController> logger, ApiDbContext apiDbContext, IConfiguration configuration)
        {
            _logger = logger;
            _apiDbContext = apiDbContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        public IActionResult SignUp([FromBody]User user)
        {
            try
            {
                var userRegistered = _apiDbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault();
                if (userRegistered != null)
                {
                    return BadRequest("Email is already registered with another user.");
                }
                else
                {
                    string hashedPassword = SecurePasswordHasherHelper.Hash(user.Password);
                    var userObj = new User()
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Password = hashedPassword
                    };
                    _apiDbContext.Users.Add(userObj);
                    _apiDbContext.SaveChanges();
                    return StatusCode(StatusCodes.Status201Created, "User Registered");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult LogIn([FromBody]User user)
        {
            try
            {
                var u = _apiDbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault();
                if (u == null)
                {
                    return NotFound("Incorrect email or password.");
                }
                else
                {
                    bool passwordValid = SecurePasswordHasherHelper.Verify(user.Password, u.Password);
                    if (!passwordValid)
                    {
                        return Unauthorized("Incorrect email or password.");
                    }

                    var claims = new[]
                    {
                       new Claim(JwtRegisteredClaimNames.Email, user.Email),
                       new Claim(ClaimTypes.Email, user.Email),
                    };

                    var token = _auth.GenerateAccessToken(claims);

                    return new ObjectResult(new
                    {
                        access_token = token.AccessToken,
                        expires_in = token.ExpiresIn,
                        token_type = token.TokenType,
                        creation_Time = token.ValidFrom,
                        expiration_Time = token.ValidTo,
                        user_id = u.Id,
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword([FromBody]ChangePasswordModel changePasswordModel)
        {
            try
            {
                var jwtEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var u = _apiDbContext.Users.FirstOrDefault(u => u.Email == jwtEmail);
                if (u == null)
                {
                    return NotFound("Incorrect email.");
                } else
                {
                    bool passwordValid = SecurePasswordHasherHelper.Verify(changePasswordModel.OldPassword, u.Password);
                    if (!passwordValid)
                    {
                        return Unauthorized("Incorrect password.");
                    }

                    u.Password = SecurePasswordHasherHelper.Hash(changePasswordModel.NewPassword);
                    _apiDbContext.SaveChanges();

                    return StatusCode(StatusCodes.Status201Created, "Password Updated");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditPhoneNumber([FromBody]ChangePhoneModel changePhoneModel)
        {
            try
            {
                var jwtEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var u = _apiDbContext.Users.FirstOrDefault(u => u.Email == jwtEmail);
                if (u == null)
                {
                    return NotFound("Incorrect email.");
                }
                else
                {
                    u.Phone = changePhoneModel.PhoneNumber;
                    _apiDbContext.SaveChanges();

                    return StatusCode(StatusCodes.Status201Created, "Phone Number Updated");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditProfileImage([FromBody]byte[] ImageArray)
        {
            try
            {
                var jwtEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var u = _apiDbContext.Users.FirstOrDefault(u => u.Email == jwtEmail);
                if (u == null)
                {
                    return NotFound("Incorrect email.");
                }
                else
                {
                    var stream = new MemoryStream(ImageArray);
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";
                    var folder = "wwwroot";
                    var response = FilesHelper.UploadImage(stream, folder, file);
                    if (!response)
                    {
                        return BadRequest("Bad Image");
                    }
                    else
                    {
                        u.ImageUrl = file;
                        _apiDbContext.SaveChanges();

                        return StatusCode(StatusCodes.Status201Created, "Profile Image Updated");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
