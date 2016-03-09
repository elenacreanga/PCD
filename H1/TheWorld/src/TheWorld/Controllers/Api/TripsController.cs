using AutoMapper;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.Authorization;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips")]
    public class TripsController : ApiController
    {
        private readonly IWorldRepository worldRepository;
        private readonly ILogger<TripsController> tripLogger;

        public TripsController(IWorldRepository worldRepository, ILogger<TripsController> tripLogger)
        {
            this.worldRepository = worldRepository;
            this.tripLogger = tripLogger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var trips = worldRepository.GetUserTripsWithStops(User.Identity.Name);
            var results = Mapper.Map<IEnumerable<TripViewModel>>(trips);
            return Ok(results);
        }

        [HttpPost("")]
        public IActionResult Post([FromBody]TripViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newTrip = Mapper.Map<Trip>(vm);
                    newTrip.Username = User.Identity.Name;

                    tripLogger.LogInformation("Attempting to save a new trip");
                    worldRepository.AddTrip(newTrip);
                    if (worldRepository.SaveAll())
                    {
                        var trip = Mapper.Map<TripViewModel>(newTrip);
                        return Created("", trip);
                    }
                }
            }
            catch (Exception e)
            {
                const string errorMessage = "Failed saving new trip";
                tripLogger.LogError(errorMessage, e);
                ModelState.AddModelError(errorMessage, e.Message);
                return BadRequest(ModelState);
            }
            return BadRequest(false.ToString());
        }
    }
}