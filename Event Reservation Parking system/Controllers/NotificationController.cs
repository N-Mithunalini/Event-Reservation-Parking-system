using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api/notifications")]
[Authorize]
public class NotificationController : ApiControllerBase
{
    private readonly INotificationService _service;
    public NotificationController(INotificationService service) => _service = service;

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

    [Authorize(Roles = "Customer")]
    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> Read(int id)
    {
        try { return Ok(await _service.MarkReadAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }
}