using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : Controller
    {

        private ApiDbContext _apiDbContext;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ILogger<CategoriesController> logger, ApiDbContext apiDbContext)
        {
            _logger = logger;
            _apiDbContext = apiDbContext;
        }
        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            var categories = _apiDbContext.Categories;
            return Ok(categories);
        }
    }
}
