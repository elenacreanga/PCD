using System.Collections.Generic;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();

        IEnumerable<Trip> GetAllTripsWithStops();

        bool SaveAll();

        void AddTrip(Trip newTrip);

        Trip GetTripByName(string username, string tripName);

        void AddStop(string tripName, string username, Stop stop);

        IEnumerable<Trip> GetUserTripsWithStops(string name);

        void DeleteStop(int stopId);

        void EditStop(Stop stop);
    }
}