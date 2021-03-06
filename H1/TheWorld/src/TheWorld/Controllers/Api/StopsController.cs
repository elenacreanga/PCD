﻿using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
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
                var results = worldRepository.GetTripByName(tripName, User.Identity.Name);
                if (results == null)
                {
                    return Ok();
                }
                var stops = StopViewModel.MapToViewModel(results);
                return Ok(stops);
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to retrieve trip by name {tripName}";
                stopsLogger.LogError(errorMessage, e);
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
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
                    worldRepository.AddStop(tripName, User.Identity.Name, stop);

                    if (worldRepository.SaveAll())
                    {
                        viewModel = Mapper.Map<StopViewModel>(stop);
                        viewModel.Links = StopViewModel.GenerateLinks(viewModel, tripName);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StopViewModel viewModel)
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
                    stop.Id = id;
                    worldRepository.EditStop(stop);

                    if (worldRepository.SaveAll())
                    {
                        viewModel = Mapper.Map<StopViewModel>(stop);
                        return Ok(viewModel);
                    }
                    return Ok();
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

        [HttpDelete]
        public IActionResult Delete(int stopId)
        {
            try
            {
                worldRepository.DeleteStop(stopId);
                if (worldRepository.SaveAll())
                {
                    return new NoContentResult();
                }
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                const string errorMessage = "Failed to delete stop";
                stopsLogger.LogError(errorMessage, e);
                ModelState.AddModelError(errorMessage, e.Message);
                return BadRequest(ModelState);
            }
        }
    }
}