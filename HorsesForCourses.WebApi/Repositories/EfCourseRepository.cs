using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Repositories
{
    public class EfCourseRepository
    {
        private readonly AppDbContext _context;

        public EfCourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Course course, CancellationToken ct = default)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Courses
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<List<Course>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Courses
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}