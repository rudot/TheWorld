using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWorld.Models;

namespace TheWorld.Controllers.Api
{
    public class TripsController : Controller
    {
        [HttpGet("api/trips")]
        public IActionResult Get()
        {
            if (true)
                return BadRequest("Bad things happen");

            return Ok(new Trip()
            {
                Name = "My Trip"
            });
        }
    }
}
