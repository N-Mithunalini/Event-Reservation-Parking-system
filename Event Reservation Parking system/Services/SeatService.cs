using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventParkingReservationSystem.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _seats;
    private readonly IEventRepository _events;
    public SeatService(ISeatRepository seats, IEventRepository events) { _seats = seats; _events = events; }

    public async Task<object> GetByEventAsync(int eventId) => await _seats.GetByEventAsync(eventId);

    public async Task<object> GenerateAsync(int eventId, SeatMapCreateDto dto)
    {
        var ev = await _events.GetByIdAsync(eventId) ?? throw new KeyNotFoundException("Event not found.");
        if ((await _seats.GetByEventAsync(eventId)).Count > 0) throw new InvalidOperationException("Seat map already exists.");
        if (dto.Rows * dto.Columns != ev.Capacity) throw new InvalidOperationException("Seat count must exactly match event capacity.");

        var list = new List<Seat>();
        for (int r = 0; r < dto.Rows; r++)
        {
            var row = ((char)('A' + r)).ToString();
            for (int c = 1; c <= dto.Columns; c++)
                list.Add(new Seat { EventId = eventId, Row = row, Column = c, SeatNumber = $"{row}{c}", Status = "Available" });
        }
        await _seats.AddRangeAsync(list); await _seats.SaveAsync(); return list;
    }

    public async Task<object> UpdateAsync(int eventId, int seatId, SeatUpdateDto dto)
    {
        var seat = await _seats.GetByIdAsync(seatId) ?? throw new KeyNotFoundException("Seat not found.");
        if (seat.EventId != eventId) throw new InvalidOperationException("Seat does not belong to this event.");
        if (await _seats.IsBookedAsync(seatId)) throw new InvalidOperationException("Booked seat cannot be changed.");
        seat.SeatNumber = dto.SeatNumber; seat.SeatType = dto.SeatType; seat.Status = dto.Status;
        await _seats.SaveAsync(); return seat;
    }

    public async Task<object> DeleteAsync(int eventId, int seatId)
    {
        var seat = await _seats.GetByIdAsync(seatId) ?? throw new KeyNotFoundException("Seat not found.");
        if (seat.EventId != eventId) throw new InvalidOperationException("Seat does not belong to this event.");
        if (await _seats.IsBookedAsync(seatId)) throw new InvalidOperationException("Booked seat cannot be deleted.");
        await _seats.DeleteAsync(seat); await _seats.SaveAsync(); return new { message = "Seat deleted." };
    }
}
