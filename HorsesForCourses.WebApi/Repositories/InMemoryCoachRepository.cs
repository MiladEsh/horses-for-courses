using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Repositories
{
    public class InMemoryCoachRepository
    {
        private readonly List<Coach> _coaches = new();

        public void Add(Coach coach)
        {
            _coaches.Add(coach);
        }

        public Coach? GetById(Guid id)
        {
            return _coaches.FirstOrDefault(c => c.Id == id);
        }

        public List<Coach> GetAll()
        {
            return _coaches;
        }
    }
}