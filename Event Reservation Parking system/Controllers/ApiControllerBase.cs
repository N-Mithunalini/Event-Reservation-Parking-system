// using Event_Reservation_Parking_system.Models;
// using EventParkingReservationSystem.Models;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;
// using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using EventParkingReservationSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
=======
using System.Security.Claims;
using EventParkingReservationSystem.Models;
using Microsoft.AspNetCore.Mvc;


namespace EventParkingReservationSystem.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int? CurrentCustomerId
    {
        get
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }

    protected bool IsAdmin => User.IsInRole("Admin");
    protected void EnsureOwnerOrAdmin(int customerId)
    {
//         if (IsAdmin) return;
//         if (CurrentCustomerId != customerId)
//             throw new UnauthorizedAccessException("You can only access your own data.");
//     }

//     protected IActionResult Handle(Exception ex) => ex switch
//     {
//         KeyNotFoundException => NotFound(new { message = ex.Message }),
//         ConflictException => Conflict(new { message = ex.Message }),
//         UnauthorizedAccessException => StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }),
//         InvalidOperationException => BadRequest(new { message = ex.Message }),
//         _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message })
//     };
// }
        if (IsAdmin)
            return;

        if (CurrentCustomerId != customerId)
        {
            throw new UnauthorizedAccessException(
                "You can only access your own data.");
        }
    }

    protected IActionResult Handle(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException =>
                NotFound(new { message = ex.Message }),

            ConflictException =>
                Conflict(new { message = ex.Message }),

            UnauthorizedAccessException =>
                StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message }),

            InvalidOperationException =>
                BadRequest(new { message = ex.Message }),

            _ =>
                StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message })
        };
    }
}
