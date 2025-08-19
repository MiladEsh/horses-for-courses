using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Repositories
{
    public class InMemoryCoachRepository
    {
        private readonly List<Coach> _coaches = new();

        public void Add(Coach coach) => _coaches.Add(coach);

        public Coach? GetById(Guid id) => _coaches.FirstOrDefault(c => c.Id == id);

        public List<Coach> GetAll() => _coaches;

        public (IReadOnlyList<TResult> Items, int TotalCount) GetPaged<TResult>(
            int page, int pageSize,
            Func<Coach, TResult> selector,
            Func<IQueryable<Coach>, IOrderedQueryable<Coach>>? orderBy = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var q = _coaches.AsQueryable();
            var total = q.Count();

            var sorted = (orderBy is null)
                ? q.OrderBy(c => c.Name).ThenBy(c => c.Id)
                : orderBy(q);

            var items = sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(selector)
                .ToList();

            return (items, total);
        }
    }
}