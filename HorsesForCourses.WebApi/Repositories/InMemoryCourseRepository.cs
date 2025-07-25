using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Repositories;

public class InMemoryCourseRepository
{
    private readonly Dictionary<Guid, Course> _courses = new();

    public void Add(Course course)
    {
        _courses[course.Id] = course;
    }

    public Course? GetById(Guid id)
    {
        return _courses.TryGetValue(id, out var course) ? course : null;
    }

    public List<Course> GetAll()
    {
        return _courses.Values.ToList();
    }

    public void UpdateSkills(Guid id, List<string> skills)
    {
        if (_courses.TryGetValue(id, out var course))
        {
            course.ReplaceRequiredCompetences(skills);
        }
    }

    public void AddTimeslot(Guid id, Timeslot slot)
    {
        if (_courses.TryGetValue(id, out var course))
        {
            course.AddTimeslot(slot);
        }
    }

    public void Confirm(Guid id)
    {
        if (_courses.TryGetValue(id, out var course))
        {
            course.Confirm();
        }
    }
}