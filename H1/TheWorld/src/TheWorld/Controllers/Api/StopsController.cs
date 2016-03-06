using AutoMapper;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("api/trips/{tripName}/stops")]
    public class StopsController : ApiController
    {
        private readonly IWorldRepository worldRepository;
        private readonly ILogger<StopsController> stopsLogger;
        private readonly CoordService coordService;

        public StopsController(IWorldRepository worldRepository, ILogger<StopsController> stopsLogger, CoordService coordService)
        {
            this.worldRepository = worldRepository;
            this.stopsLogger = stopsLogger;
            this.coordService = coordService;
        }

        [HttpGet]
        public IActionResult Get(string tripName)
        {
            try
            {
                var results = worldRepository.GetTripByName(tripName);
                if (results == null)
                {
                    return Ok();
                }
                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(x => x.Order)));
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to retrieve trip by name {tripName}";
                stopsLogger.LogError(errorMessage, e);
                return BadRequest(errorMessage);
            }
        }

        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var stop = Mapper.Map<Stop>(viewModel);
                    var coordResult = await coordService.Lookup(stop.Name);
                    if (!coordResult.Success)
                    {
                        ModelState.AddModelError(coordResult.Message, "");
                        return BadRequest(ModelState);
                    }
                    stop.Latitude = coordResult.Latitude;
                    stop.Longitude = coordResult.Longitude;
                    worldRepository.AddStop(tripName, stop);

                    if (worldRepository.SaveAll())
                    {
                        viewModel = Mapper.Map<StopViewModel>(stop);
                        return Created("", viewModel);
                    }
                }
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                const string errorMessage = "Failed to save a new stop";
                stopsLogger.LogError(errorMessage, e);
                ModelState.AddModelError(errorMessage, e.Message);
                return BadRequest(ModelState);
            }
        }
    }
}