using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImagesController : Controller
    {
        private ApiDbContext _apiDbContext;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ILogger<ImagesController> logger, ApiDbContext apiDbContext)
        {
            _logger = logger;
            _apiDbContext = apiDbContext;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Image imageModel)
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
                    var stream = new MemoryStream(imageModel.ImageArray);
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
                        var ImageObj = new Image()
                        {
                            ImageUrl = file,
                            VehicleId = imageModel.VehicleId
                        };
                        _apiDbContext.Images.Add(ImageObj);
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
