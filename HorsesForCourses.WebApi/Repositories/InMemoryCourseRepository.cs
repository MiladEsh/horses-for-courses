using HorsesForCourses.Core;


namespace HorsesForCourses.WebApi.Repositories
{
    public class InMemoryCourseRepository
    {
        private readonly List<Course> _courses = new();

        public void Add(Course course)
        {
            _courses.Add(course);
        }

        public Course? GetById(Guid id)
        {
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        public List<Course> GetAll()
        {
            return _courses;
        }
    }
}