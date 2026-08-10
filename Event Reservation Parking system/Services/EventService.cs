using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Services;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _db;
    private readonly IEventRepository _events;
    private readonly IVenueRepository _venues;
    private readonly ICategoryRepository _categories;
    private readonly INotificationService _notifications;

    public EventService(
        ApplicationDbContext db,
        IEventRepository events,
        IVenueRepository venues,
        ICategoryRepository categories,
        INotificationService notifications)
    {
        _db = db;
        _events = events;
        _venues = venues;
        _categories = categories;
        _notifications = notifications;
    }

    private static object MapEvent(Event e) => new
    {
        e.Id,
        e.Name,
        e.VenueId,
        e.CategoryId,
        e.EventDate,
        e.StartTime,
        e.EndTime,
        e.TicketPrice,
        e.ParkingFee,
        e.Capacity,
        venue = e.Venue is null ? null : new { e.Venue.Id, e.Venue.Name, e.Venue.Address, e.Venue.Capacity },
        category = e.Category is null ? null : new { e.Category.Id, e.Category.Name }
    };

    public async Task<object> GetAllAsync(string? search, int? venueId, int? categoryId, DateTime? date)
    {
        var rows = await _events.GetAllAsync(search, venueId, categoryId, date);
        return rows.Select(MapEvent).ToList();
    }

    public async Task<object> GetByIdAsync(int id)
    {
        var e = await _events.GetByIdAsync(id) ?? throw new KeyNotFoundException("Event not found.");
        return MapEvent(e);
    }

    private async Task ValidateAsync(EventDto dto, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new InvalidOperationException("Event name is required.");
        if (dto.StartTime >= dto.EndTime)
            throw new InvalidOperationException("Event end time must be after start time.");
        if (dto.Capacity <= 0)
            throw new InvalidOperationException("Event capacity must be greater than zero.");
        if (dto.TicketPrice < 0 || dto.ParkingFee < 0)
            throw new InvalidOperationException("Prices cannot be negative.");

        var venue = await _venues.GetByIdAsync(dto.VenueId)
            ?? throw new InvalidOperationException("Venue not found.");
        if (await _categories.GetByIdAsync(dto.CategoryId) is null)
            throw new InvalidOperationException("Category not found.");
        if (dto.Capacity > venue.Capacity)
            throw new InvalidOperationException("Event capacity exceeds venue capacity.");
        if (!await _venues.IsAvailableAsync(dto.VenueId, dto.EventDate, dto.StartTime, dto.EndTime, excludeId))
            throw new ConflictException("Venue is already booked for an overlapping time period.");
    }

    public async Task<object> CreateAsync(EventDto dto)
    {
        await ValidateAsync(dto);

        var e = new Event
        {
            Name = dto.Name.Trim(),
            VenueId = dto.VenueId,
            CategoryId = dto.CategoryId,
            EventDate = dto.EventDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            TicketPrice = dto.TicketPrice,
            ParkingFee = dto.ParkingFee,
            Capacity = dto.Capacity
        };

        await _events.AddAsync(e);
        await _events.SaveAsync();
        var loaded = await _events.GetByIdAsync(e.Id) ?? e;
        return MapEvent(loaded);
    }

    public async Task<object> UpdateAsync(int id, EventDto dto)
    {
        var e = await _events.GetByIdAsync(id) ?? throw new KeyNotFoundException("Event not found.");
        await ValidateAsync(dto, id);

        var booked = await _events.GetBookedSeatCountAsync(id);
        if (dto.Capacity < booked)
            throw new InvalidOperationException("Capacity cannot be below the booked seat count.");

        var hasActiveBookings = await _events.HasActiveBookingsAsync(id);
        if (hasActiveBookings && dto.TicketPrice != e.TicketPrice)
            throw new InvalidOperationException("Ticket price cannot change after active bookings exist.");

        var seatCount = await _db.Seats.CountAsync(s => s.EventId == id);
        if (seatCount > 0 && dto.Capacity != e.Capacity)
            throw new InvalidOperationException("Capacity cannot be changed after a seat map has been generated.");

        e.Name = dto.Name.Trim();
        e.VenueId = dto.VenueId;
        e.CategoryId = dto.CategoryId;
        e.EventDate = dto.EventDate;
        e.StartTime = dto.StartTime;
        e.EndTime = dto.EndTime;
        e.TicketPrice = dto.TicketPrice;
        e.ParkingFee = dto.ParkingFee;
        e.Capacity = dto.Capacity;

        await _events.UpdateAsync(e);
        await _events.SaveAsync();

        if (hasActiveBookings)
        {
            var customerIds = await _db.Bookings
                .Where(b => b.EventId == id && (b.Status == "Pending" || b.Status == "Confirmed"))
                .Select(b => b.CustomerId)
                .Distinct()
                .ToListAsync();

            foreach (var customerId in customerIds)
                await _notifications.CreateAsync(customerId, $"Event update: {e.Name} details were changed. Please review your booking.");
        }

        var loaded = await _events.GetByIdAsync(id) ?? e;
        return MapEvent(loaded);
    }

    public async Task<object> DeleteAsync(int id)
    {
        var e = await _events.GetByIdAsync(id) ?? throw new KeyNotFoundException("Event not found.");
        if (await _events.HasActiveBookingsAsync(id))
            throw new ConflictException("Event cannot be deleted while active bookings exist.");

        await _events.DeleteAsync(e);
        await _events.SaveAsync();
        return new { message = "Event deleted." };
    }
}
