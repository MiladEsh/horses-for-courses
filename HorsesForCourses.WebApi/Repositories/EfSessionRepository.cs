using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Repositories.Ef
{
    public class EfSessionRepository
    {
        private readonly AppDbContext _context;

        public EfSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
        }

        public async Task<List<Session>> GetAllAsync()
        {
            return await _context.Sessions.ToListAsync();
        }
    }
}