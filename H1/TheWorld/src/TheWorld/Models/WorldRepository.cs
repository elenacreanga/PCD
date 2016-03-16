using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private readonly WorldContext context;
        private readonly ILogger<WorldRepository> logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            try
            {
                return context.Trips.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError("Could not get db trips", ex);
                return null;
            }
        }

        public IEnumerable<Trip> GetAllTripsWithStops()
        {
            try
            {
                return context.Trips.Include(x => x.Stops)
                    .OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError("Could not get db trips", ex);
                return null;
            }
        }

        public bool SaveAll()
        {
            return context.SaveChanges() > 0;
        }

        public void AddTrip(Trip newTrip)
        {
            context.Add(newTrip);
        }

        public Trip GetTripByName(string tripName, string username)
        {
            var trip = context.Trips
                .Include(x => x.Stops)
                .FirstOrDefault(
                    x =>
                        x.Name.Equals(tripName) &&
                        x.Username.Equals(username));
            return trip;
        }

        public void AddStop(string tripName, string username, Stop stop)
        {
            var trip = GetTripByName(tripName, username);
            stop.Order = trip.Stops.Count > 0 ? trip.Stops.Max(x => x.Order) + 1 : 1;
            trip.Stops.Add(stop);
            context.Stops.Add(stop);
        }

        public IEnumerable<Trip> GetUserTripsWithStops(string name)
        {
            try
            {
                return context.Trips.Include(x => x.Stops)
                    .OrderBy(x => x.Name)
                    .Where(x => x.Username.Equals(name))
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.LogError("Could not get db trips", ex);
                return null;
            }
        }

        public void EditStop(Stop updatedStop)
        {
            var stop = context.Stops.FirstOrDefault(x => x.Id == updatedStop.Id);
            if (stop == null) return;
            stop.Name = updatedStop.Name;
            stop.Arrival = updatedStop.Arrival;
            stop.Latitude = updatedStop.Latitude;
            stop.Longitude = updatedStop.Longitude;
        }

        public void DeleteStop(int stopId)
        {
            try
            {
                var trip = context.Trips.FirstOrDefault(x => x.Stops.First(s => s.Id == stopId) != null);
                context.Trips.Remove(trip);
                var stop = context.Stops.FirstOrDefault(x => x.Id == stopId);
                context.Stops.Remove(stop);
                var order = 1;
                foreach (var tripstop in trip.Stops.OrderBy(x => x.Order))
                {
                    tripstop.Order = order;
                    order++;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Could not get db trips", ex);
                throw;
            }
        }
    }
}