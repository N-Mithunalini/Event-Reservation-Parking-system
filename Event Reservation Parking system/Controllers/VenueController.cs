using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api/venues")]
public class VenueController : ApiControllerBase
{
    private readonly IVenueService _service;
    public VenueController(IVenueService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All()
    {
        try { return Ok(await _service.GetAllAsync()); }
        catch (Exception ex) { return Handle(ex); }
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> One(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [AllowAnonymous]
    [HttpGet("available")]
    public async Task<IActionResult> Available(int venueId, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        try { return Ok(await _service.AvailableAsync(venueId, date, startTime, endTime)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(VenueDto dto)
    {
        try { return StatusCode(201, await _service.CreateAsync(dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VenueDto dto)
    {
        try { return Ok(await _service.UpdateAsync(id, dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try { return Ok(await _service.DeleteAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }
}

