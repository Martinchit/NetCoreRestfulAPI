using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
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
    public class VehiclesController : Controller
    {
        private ApiDbContext _apiDbContext;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(ILogger<VehiclesController> logger, ApiDbContext apiDbContext)
        {
            _logger = logger;
            _apiDbContext = apiDbContext;
        }


        //[HttpGet]
        //public IActionResult Get()
        //{
        //var vehicles = _apiDbContext.Vehicles;
        //    return Ok(vehicles);
        //}

        [HttpGet("{id}")]
        public IActionResult GetByVehicleId(int id)
        {
            var vehicle = _apiDbContext.Vehicles.Find(id);
            if (vehicle != null)
            {
                return Ok(vehicle);
            }
            else
            {
                return NotFound("No record found against this vehicle ID");
            }
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "categoryId")]int categoryId)
        {
            if (categoryId == 0)
            {
                var vehicles = _apiDbContext.Vehicles;
                return Ok(vehicles);
            } else
            {
                var vehicles = _apiDbContext.Vehicles
                                .Where(v => v.CategoryId == categoryId)
                                .Select(v => new
                                {
                                    v.Id,
                                    v.Title,
                                    v.Price,
                                    v.Location,
                                    v.DatePosted,
                                    v.IsFeatured,
                                    v.Images.FirstOrDefault().ImageUrl
                                })
                                .OrderBy(v => v.Id);
                return Ok(vehicles);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Vehicle vehicle)
        {
            try
            {
                var jwtEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var u = _apiDbContext.Users.FirstOrDefault(u => u.Email == jwtEmail);
                if (u == null)
                {
                    return NotFound("Incorrect email.");
                }
                var vehicleObj = new Vehicle()
                {
                    Title = vehicle.Title,
                    Description = vehicle.Description,
                    Price = vehicle.Price,
                    Model = vehicle.Model,
                    Engine = vehicle.Engine,
                    Color = vehicle.Color,
                    Company = vehicle.Company,
                    DatePosted = vehicle.DatePosted,
                    IsFeatured = false,
                    IsHotAndNew = false,
                    Location = vehicle.Location,
                    Condition = vehicle.Condition,
                    CategoryId = vehicle.CategoryId,
                    UserId = u.Id
                };
                _apiDbContext.Vehicles.Add(vehicleObj);
                _apiDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status201Created, new { vehicleId = vehicleObj.Id, message = "Vehicle Created" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]")]
        public IActionResult HotAndNewAds()
        {
            var vehicles = _apiDbContext.Vehicles
                               .Where(v => v.IsHotAndNew == true)
                               .Select(v => new
                               {
                                   v.Id,
                                   v.Title,
                                   v.Images.FirstOrDefault().ImageUrl
                               })
                               .OrderBy(v => v.Id);
            return Ok(vehicles);
        }

        [HttpGet("[action]")]
        public IActionResult Search(string search)
        {
            var vehicles = _apiDbContext.Vehicles
                               .Where(v => v.Title.ToLower().StartsWith(search.ToLower()))
                               .Select(v => new
                               {
                                   v.Id,
                                   v.Title,
                               })
                               .OrderBy(v => v.Id);
            return Ok(vehicles);
        }

        [HttpGet("[action]")]
        public IActionResult VehicleDetails(int id)
        {
            try
            {
                var v = _apiDbContext.Vehicles.Find(id);
                if (v == null)
                {
                    return NotFound();
                } else
                {
                    var vehicle = _apiDbContext.Vehicles
                                        .Where(v => v.Id == id)
                                        .Join(
                                            _apiDbContext.Users,
                                            v => v.UserId,
                                            u => u.Id,
                                            (v, u) => new
                                            {
                                               v.Id,
                                               v.Title,
                                               v.Description,
                                               v.Price,
                                               v.Model,
                                               v.Engine,
                                               v.Color,
                                               v.Company,
                                               v.DatePosted,
                                               v.Condition,
                                               v.Location,
                                               v.Images,
                                               u.Email,
                                               u.Phone,
                                               u.ImageUrl
                                            }
                                        );
                    return Ok(vehicle);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]")]
        public IActionResult MyAds()
        {   
            try
            {
                var jwtEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var u = _apiDbContext.Users.FirstOrDefault(u => u.Email == jwtEmail);
                if (u == null)
                {
                    return NotFound("Incorrect email.");
                }
                var vehicles = _apiDbContext.Vehicles
                                .Where(v => v.UserId == u.Id)
                                .Select(v => new
                                {
                                    v.Id,
                                    v.Title,
                                    v.Price,
                                    v.Location,
                                    v.DatePosted,
                                    v.IsFeatured,
                                    v.Images.FirstOrDefault().ImageUrl
                                })
                                .OrderBy(v => v.Id);
                return Ok(vehicles);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
