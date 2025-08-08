using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Repositories
{
    public class EfCoachRepository
    {
        private readonly AppDbContext _context;

        public EfCoachRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Coach coach)
        {
            _context.Coaches.Add(coach);
            await _context.SaveChangesAsync();
        }

        public async Task<Coach?> GetByIdAsync(Guid id)
        {
            return await _context.Coaches.FindAsync(id);
        }

        public async Task<List<Coach>> GetAllAsync()
        {
            return await _context.Coaches.ToListAsync();
        }
    }
}