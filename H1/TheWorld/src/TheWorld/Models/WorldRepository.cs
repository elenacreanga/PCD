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

        public Trip GetTripByName(string tripName)
        {
            var trip = context.Trips.Include(x => x.Stops).FirstOrDefault(x => x.Name.Equals(tripName));
            return trip;
        }

        public void AddStop(string tripName, Stop stop)
        {
            var trip = GetTripByName(tripName);
            stop.Order = trip.Stops.Max(x => x.Order) + 1;
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
    }
}