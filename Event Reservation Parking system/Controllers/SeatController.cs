using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api/events/{eventId:int}/seats")]
public class SeatController : ApiControllerBase
{
    private readonly ISeatService _service;
    public SeatController(ISeatService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All(int eventId)
    {
        try { return Ok(await _service.GetByEventAsync(eventId)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Generate(int eventId, SeatMapCreateDto dto)
    {
        try { return StatusCode(201, await _service.GenerateAsync(eventId, dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{seatId:int}")]
    public async Task<IActionResult> Update(int eventId, int seatId, SeatUpdateDto dto)
    {
        try { return Ok(await _service.UpdateAsync(eventId, seatId, dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{seatId:int}")]
    public async Task<IActionResult> Delete(int eventId, int seatId)
    {
        try { return Ok(await _service.DeleteAsync(eventId, seatId)); }
        catch (Exception ex) { return Handle(ex); }
    }
}
