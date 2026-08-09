using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api/dashboard")]
[Authorize]
public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service) => _service = service;

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> Admin()
    {
        try { return Ok(await _service.AdminAsync()); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> Customer(int customerId)
    {
        try
        {
            EnsureOwnerOrAdmin(customerId);
            return Ok(await _service.CustomerAsync(customerId));
        }
        catch (Exception ex) { return Handle(ex); }
    }
}
