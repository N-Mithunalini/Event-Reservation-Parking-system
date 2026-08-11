using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;

namespace EventParkingReservationSystem.Services;

public class ParkingService : IParkingService
{
    private readonly IParkingRepository _parking;
    private readonly IEventRepository _events;

    public ParkingService(
        IParkingRepository parking,
        IEventRepository events)
    {
        _parking = parking;
        _events = events;
    }

    public async Task<object> GetByEventAsync(int eventId)
    {
        return await _parking.GetByEventAsync(eventId);
    }

    public async Task<object> GenerateAsync(
        int eventId,
        ParkingLayoutCreateDto dto)
    {
        _ = await _events.GetByIdAsync(eventId)
            ?? throw new KeyNotFoundException("Event not found.");

        if (dto.SlotCount <= 0)
        {
            throw new InvalidOperationException(
                "Slot count must be greater than zero.");
        }

        if (dto.Fee < 0)
        {
            throw new InvalidOperationException(
                "Parking fee cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(dto.Zone))
        {
            throw new InvalidOperationException(
                "Parking zone is required.");
        }

        var existing =
            await _parking.GetByEventAsync(eventId);

        if (existing.Count > 0)
        {
            throw new InvalidOperationException(
                "Parking layout already exists.");
        }

        var slots =
            Enumerable.Range(1, dto.SlotCount)
                .Select(i => new ParkingSlot
                {
                    EventId = eventId,
                    SlotNumber = $"P{i:00}",
                    Zone = dto.Zone.Trim(),
                    Fee = dto.Fee,
                    Status = "Available"
                })
                .ToList();

        await _parking.AddRangeAsync(slots);
        await _parking.SaveAsync();

        return slots;
    }

    public async Task<object> UpdateAsync(
        int eventId,
        int slotId,
        ParkingSlotUpdateDto dto)
    {
        var slot =
            await _parking.GetByIdAsync(slotId)
            ?? throw new KeyNotFoundException(
                "Parking slot not found.");

        if (slot.EventId != eventId)
        {
            throw new InvalidOperationException(
                "Parking slot does not belong to this event.");
        }

        if (await _parking.IsReservedAsync(slotId))
        {
            throw new InvalidOperationException(
                "Reserved parking slot cannot be changed.");
        }

        if (string.IsNullOrWhiteSpace(dto.SlotNumber))
        {
            throw new InvalidOperationException(
                "Slot number is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Zone))
        {
            throw new InvalidOperationException(
                "Parking zone is required.");
        }

        if (dto.Fee < 0)
        {
            throw new InvalidOperationException(
                "Parking fee cannot be negative.");
        }

        slot.SlotNumber = dto.SlotNumber.Trim();
        slot.Zone = dto.Zone.Trim();
        slot.Fee = dto.Fee;
        slot.Status = dto.Status;

        await _parking.SaveAsync();

        return slot;
    }

    public async Task<object> DeleteAsync(
        int eventId,
        int slotId)
    {
        var slot =
            await _parking.GetByIdAsync(slotId)
            ?? throw new KeyNotFoundException(
                "Parking slot not found.");

        if (slot.EventId != eventId)
        {
            throw new InvalidOperationException(
                "Parking slot does not belong to this event.");
        }

        if (await _parking.IsReservedAsync(slotId))
        {
            throw new InvalidOperationException(
                "Reserved parking slot cannot be deleted.");
        }

        await _parking.DeleteAsync(slot);
        await _parking.SaveAsync();

        return new
        {
            message = "Parking slot deleted."
        };
    }
}