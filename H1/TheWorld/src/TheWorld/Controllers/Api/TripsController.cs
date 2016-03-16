using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
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
            var results = Mapper.Map<IList<TripViewModel>>(trips);
            foreach (var trip in results)
            {
                trip.Links = GenerateLinks(trip);
            }
            return Ok(results);
        }

        //[HttpGet("")]
        //public IActionResult Get(string name)
        //{
        //    var trip = worldRepository.GetTripByName(User.Identity.Name, name);
        //    var viewModel = Mapper.Map<TripViewModel>(trip);
        //    viewModel.Links = GenerateTripLinks(viewModel);
        //    return Ok(viewModel);
        //}

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
                        trip.Links = GenerateLinks(trip);
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

        private static List<Link> GenerateLinks(TripViewModel newTrip)
        {
            var links = new List<Link>
            {
                new Link()
                {
                    Rel = "trip/GetAllStops",
                    Uri = $"api/trips/{newTrip.Name}/stops"
                },
                new Link()
                {
                    Rel = "trip/GetByName",
                    Uri = $"api/trips/{newTrip.Name}"
                },
            };
            return links;
        }
    }
}