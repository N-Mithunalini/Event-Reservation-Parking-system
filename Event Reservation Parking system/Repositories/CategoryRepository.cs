using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;
    public CategoryRepository(ApplicationDbContext db) => _db = db;
    public Task<List<EventCategory>> GetAllAsync() => _db.EventCategories.OrderBy(x => x.Name).ToListAsync();
    public Task<EventCategory?> GetByIdAsync(int id) => _db.EventCategories.FindAsync(id).AsTask();
    public Task AddAsync(EventCategory category) { _db.EventCategories.Add(category); return Task.CompletedTask; }
    public Task UpdateAsync(EventCategory category) { _db.EventCategories.Update(category); return Task.CompletedTask; }
    public Task DeleteAsync(EventCategory category) { _db.EventCategories.Remove(category); return Task.CompletedTask; }
    public Task<bool> IsInUseAsync(int id) => _db.Events.AnyAsync(e => e.CategoryId == id);
    public Task SaveAsync() => _db.SaveChangesAsync();
}
