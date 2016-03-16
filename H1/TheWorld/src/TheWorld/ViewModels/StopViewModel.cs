using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TheWorld.Models;

namespace TheWorld.ViewModels
{
    public class StopViewModel
    {
        public StopViewModel()
        {
            Links = new List<Link>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 5)]
        public string Name { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required]
        public DateTime Arrival { get; set; }

        public IList<Link> Links { get; set; }

        public static IList<StopViewModel> MapToViewModel(Trip trip)
        {
            var stops = Mapper.Map<IList<StopViewModel>>(trip.Stops.OrderBy(x => x.Order));
            GenerateLinks(trip.Name, stops);
            return stops;
        }

        private static void GenerateLinks(string tripName, IList<StopViewModel> stops)
        {
            foreach (var stop in stops)
            {
                stop.Links = GenerateLinks(stop, tripName);
            }
        }

        public static List<Link> GenerateLinks(StopViewModel stop, string tripName)
        {
            var links = new List<Link>
            {
                new Link()
                {
                    Rel = "modifyStop",
                    Uri = $"api/trips/stops/"
                },
                new Link()
                {
                    Rel = "getStops",
                    Uri = $"api/trips/{tripName}/stops"
                },
                new Link()
                {
                    Rel = "deleteStop",
                    Uri = $"api/trips/{tripName}/stops/{stop.Id}"
                },
                new Link()
                {
                    Rel = "allTrips",
                    Uri = "api/trips"
                }
            };
            return links;
        }
    }
}