using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Repositories
{
    public class InMemoryCourseRepository
    {
        private readonly List<Course> _courses = new();

        public void Add(Course course) => _courses.Add(course);

        public Course? GetById(Guid id) => _courses.FirstOrDefault(c => c.Id == id);

        public List<Course> GetAll() => _courses;

        public (IReadOnlyList<TResult> Items, int TotalCount) GetPaged<TResult>(
            int page, int pageSize,
            Func<Course, TResult> selector,
            Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var q = _courses.AsQueryable();
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