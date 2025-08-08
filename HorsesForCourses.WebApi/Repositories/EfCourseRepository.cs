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

        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }
    }
}