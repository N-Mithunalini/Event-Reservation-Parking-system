using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api")]
[Authorize]
public class PaymentController : ApiControllerBase
{
    private readonly IPaymentService _service;
    public PaymentController(IPaymentService service) => _service = service;

    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("bookings/{id:int}/payment")]
    public async Task<IActionResult> Get(int id)
    {
        try { return Ok(await _service.GetForBookingAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("bookings/{id:int}/payment")]
    public async Task<IActionResult> Pay(int id)
    {
        try { return Ok(await _service.PayAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("payments/customer/{customerId:int}")]
    public async Task<IActionResult> Customer(int customerId)
    {
        try
        {
            EnsureOwnerOrAdmin(customerId);
            return Ok(await _service.GetByCustomerAsync(customerId));
        }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("payments/{id:int}/receipt")]
    public async Task<IActionResult> Receipt(int id)
    {
        try { return Ok(await _service.GetReceiptAsync(id)); }
        catch (Exception ex) { return Handle(ex); }
    }
}
