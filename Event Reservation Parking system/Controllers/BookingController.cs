using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api/bookings")]
[Authorize]
public class BookingController : ApiControllerBase
{
    private readonly IBookingService _service;
    public BookingController(IBookingService service) => _service = service;

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> Create(BookingCreateDto dto)
    {
        try
        {
            EnsureOwnerOrAdmin(dto.CustomerId);
            return StatusCode(201, await _service.CreateAsync(dto));
        }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> One(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> Customer(int customerId)
    {
        try
        {
            EnsureOwnerOrAdmin(customerId);
            return Ok(await _service.GetByCustomerAsync(customerId));
        }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Event([FromQuery] int eventId)
    {
        try { return Ok(await _service.GetByEventAsync(eventId)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        try { return Ok(await _service.CancelAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("{id:int}/hold-status")]
    public async Task<IActionResult> Hold(int id)
    {
        try { return Ok(await _service.HoldStatusAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }
}
