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

        public async Task AddAsync(Coach coach, CancellationToken ct = default)
        {
            _context.Coaches.Add(coach);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Coach?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Coaches
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<List<Coach>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Coaches
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}