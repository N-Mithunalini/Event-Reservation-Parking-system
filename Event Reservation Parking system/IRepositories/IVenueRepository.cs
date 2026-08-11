<<<<<<< Updated upstream
using EventParkingReservationSystem.Models;
namespace EventParkingReservationSystem.IRepositories;
=======
>>>>>>> Stashed changes

using EventParkingReservationSystem.Models;
using global::EventParkingReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventParkingReservationSystem.IRepositories;
public interface IVenueRepository
{
    Task<List<Venue>> GetAllAsync();
    Task<Venue?> GetByIdAsync(int id);
    Task AddAsync(Venue venue);
    Task UpdateAsync(Venue venue);
    Task DeleteAsync(Venue venue);
    Task<bool> HasUpcomingEventsAsync(int venueId);
    Task<bool> IsAvailableAsync(int venueId, DateTime date, TimeSpan start, TimeSpan end, int? excludeEventId = null);
    Task SaveAsync();
}
