using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Repositories.Ef
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
            await _context.Coaches.AddAsync(coach);
        }

        public async Task<Coach?> GetByIdAsync(Guid id)
        {
            return await _context.Coaches.FindAsync(id);
        }

        public async Task<List<Coach>> GetAllAsync()
        {
            return await _context.Coaches.ToListAsync();
        }

        public async Task UpdateSkillsAsync(Guid id, List<string> skills)
        {
            var coach = await _context.Coaches.FindAsync(id);
            if (coach != null)
            {
                coach.ReplaceCompetences(skills);
            }
        }
    }
}