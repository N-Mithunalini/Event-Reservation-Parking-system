using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;

namespace EventParkingReservationSystem.Services;

public class VenueService : IVenueService
{
    private readonly IVenueRepository _repo;
    public VenueService(IVenueRepository repo) => _repo = repo;
    public async Task<object> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<object> GetByIdAsync(int id) => await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Venue not found.");

    public async Task<object> CreateAsync(VenueDto dto)
    {
        var x = new Venue { Name = dto.Name, Address = dto.Address, Capacity = dto.Capacity };
        await _repo.AddAsync(x); await _repo.SaveAsync(); return x;
    }

    public async Task<object> UpdateAsync(int id, VenueDto dto)
    {
        var x = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Venue not found.");
        x.Name = dto.Name; x.Address = dto.Address; x.Capacity = dto.Capacity;
        await _repo.UpdateAsync(x); await _repo.SaveAsync(); return x;
    }

    public async Task<object> DeleteAsync(int id)
    {
        var x = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Venue not found.");
        if (await _repo.HasUpcomingEventsAsync(id)) throw new InvalidOperationException("Venue has upcoming events.");
        await _repo.DeleteAsync(x); await _repo.SaveAsync(); return new { message = "Venue deleted." };
    }

    public async Task<object> AvailableAsync(int venueId, DateTime date, TimeSpan start, TimeSpan end) =>
        new { venueId, available = await _repo.IsAvailableAsync(venueId, date, start, end) };
}
