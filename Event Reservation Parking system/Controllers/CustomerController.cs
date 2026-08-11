using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventParkingReservationSystem.Controllers;

[ApiController]
[Authorize]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customers;

    public CustomerController(
        ICustomerService customers)
    {
        _customers = customers;
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> All(
        [FromQuery] string? search)
    {
        return Ok(
            await _customers
                .GetAllAsync(search));
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> One(int id)
    {
        if (!OwnerOrAdmin(id))
            return Forbid();

        try
        {
            return Ok(
                await _customers
                    .GetByIdAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        CustomerUpdateDto dto)
    {
        if (!OwnerOrAdmin(id))
            return Forbid();

        try
        {
            return Ok(
                await _customers.UpdateAsync(
                    id,
                    dto));
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(
        int id)
    {
        try
        {
            return Ok(
                await _customers
                    .DeactivateAsync(id));
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/reactivate")]
    public async Task<IActionResult> Reactivate(
        int id)
    {
        return Ok(
            await _customers
                .ReactivateAsync(id));
    }


    private bool OwnerOrAdmin(int customerId)
    {
        if (User.IsInRole("Admin"))
            return true;

        var value =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        return int.TryParse(
                   value,
                   out var currentId)
               &&
               currentId == customerId;
    }
}