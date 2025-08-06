using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Repositories.Ef
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
            await _context.Courses.AddAsync(course);
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task UpdateSkillsAsync(Guid id, List<string> skills)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                course.ReplaceRequiredCompetences(skills);
            }
        }

        public async Task ConfirmAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            course?.Confirm();
        }

        public async Task AssignCoachAsync(Guid courseId, Coach coach)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                course.AssignCoach(coach);
                coach.AssignCourse(course);
            }
        }

        public async Task<List<Course>> GetConfirmedAsync()
        {
            return await _context.Courses
                .Where(c => c.Status == CourseStatus.PendingForCoach)
                .ToListAsync();
        }
    }
}