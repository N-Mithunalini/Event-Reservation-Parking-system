using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EventParkingReservationSystem.Controllers;

[Route("api/events/{eventId:int}/parking-slots")]
public class ParkingController : ApiControllerBase
{
    private readonly IParkingService _service;
    public ParkingController(IParkingService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All(int eventId)
    {
        try { return Ok(await _service.GetByEventAsync(eventId)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Generate(int eventId, ParkingLayoutCreateDto dto)
    {
        try { return StatusCode(201, await _service.GenerateAsync(eventId, dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{slotId:int}")]
    public async Task<IActionResult> Update(int eventId, int slotId, ParkingSlotUpdateDto dto)
    {
        try { return Ok(await _service.UpdateAsync(eventId, slotId, dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{slotId:int}")]
    public async Task<IActionResult> Delete(int eventId, int slotId)
    {
        try { return Ok(await _service.DeleteAsync(eventId, slotId)); }
        catch (Exception ex) { return Handle(ex); }
    }
}
