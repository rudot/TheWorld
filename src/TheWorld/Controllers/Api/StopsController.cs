using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("/api/trips/{tripName}/stops")]
    public class StopsController:Controller
    {
        private GeoCoordsService _coordsService;
        private ILogger<StopsController> _logger;
        private IWorldRepository _repository;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger,
         GeoCoordsService coordsService )
        {
            _repository = repository;
            _logger = logger;
            _coordsService = coordsService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName, User.Identity.Name);

                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s=> s.Order).ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get stops : {0}", ex);
            }

            return BadRequest("Failed to get stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                //if the vm is valid

                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    //lookup the geocodes

                    var result = await _coordsService.GetCoordsAsync(newStop.Name);

                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;
                        //save to db
                        _repository.AddStop(tripName, User.Identity.Name, newStop);

                        if (await _repository.SaveChangeAsync())
                        {
                            return Created($"/api/trips/{tripName}/stops/{newStop.Name}",
                            Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new Stop: [0]", ex);
            }

            return BadRequest("Failed to save new stop");
        }
    }
}
