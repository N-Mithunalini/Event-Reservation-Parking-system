using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.Controllers;

[Route("api/categories")]
public class CategoryController : ApiControllerBase
{
    private readonly ICategoryService _service;
    public CategoryController(ICategoryService service) => _service = service;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> All()
    {
        try { return Ok(await _service.GetAllAsync()); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        try { return StatusCode(201, await _service.CreateAsync(dto)); }
        catch (Exception ex) { return Handle(ex); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryDto dto)
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
