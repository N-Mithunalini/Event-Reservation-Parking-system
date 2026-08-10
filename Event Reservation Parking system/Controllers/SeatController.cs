using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EventParkingReservationSystem.Controllers;

[ApiController]
[Route("api/events/{eventId:int}/seats")]
public class SeatController : ApiControllerBase
{
    private readonly ISeatService _service;

    public SeatController(ISeatService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All(int eventId)
    {
        try
        {
            var result = await _service.GetByEventAsync(eventId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Handle(ex);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Generate(
        int eventId,
        [FromBody] SeatMapCreateDto dto)
    {
        try
        {
            var result = await _service.GenerateAsync(eventId, dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (Exception ex)
        {
            return Handle(ex);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{seatId:int}")]
    public async Task<IActionResult> Update(
        int eventId,
        int seatId,
        [FromBody] SeatUpdateDto dto)
    {
        try
        {
            var result = await _service.UpdateAsync(eventId, seatId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Handle(ex);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{seatId:int}")]
    public async Task<IActionResult> Delete(
        int eventId,
        int seatId)
    {
        try
        {
            var result = await _service.DeleteAsync(eventId, seatId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Handle(ex);
        }
    }
}
