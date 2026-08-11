using EventParkingReservationSystem.DTOs;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Models;
<<<<<<< Updated upstream
=======
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
>>>>>>> Stashed changes

namespace EventParkingReservationSystem.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    public CategoryService(ICategoryRepository repo) => _repo = repo;
    public async Task<object> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<object> CreateAsync(CategoryDto dto)
    {
        var x = new EventCategory { Name = dto.Name };
        await _repo.AddAsync(x); await _repo.SaveAsync(); return x;
    }

    public async Task<object> UpdateAsync(int id, CategoryDto dto)
    {
        var x = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Category not found.");
        x.Name = dto.Name; await _repo.UpdateAsync(x); await _repo.SaveAsync(); return x;
    }

    public async Task<object> DeleteAsync(int id)
    {
        var x = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Category not found.");
        if (await _repo.IsInUseAsync(id)) throw new InvalidOperationException("Category is assigned to existing events.");
        await _repo.DeleteAsync(x); await _repo.SaveAsync(); return new { message = "Category deleted." };
    }
}
