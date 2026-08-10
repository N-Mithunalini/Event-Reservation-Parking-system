using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api/events")]
public class EventController : ApiControllerBase
{
    private readonly IEventService _service;
    public EventController(IEventService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All(string? search = null, int? venueId = null, int? categoryId = null, DateTime? date = null)
    {
        try { return Ok(await _service.GetAllAsync(search, venueId, categoryId, date)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> One(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(EventDto dto)
    {
        try { return StatusCode(201, await _service.CreateAsync(dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EventDto dto)
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

