using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Repositories;

public class InMemoryCourseRepository
{
    private readonly Dictionary<Guid, Course> _courses = new();
    private readonly Dictionary<Guid, Coach> _coaches = new();
    public void Add(Course course)
    {
        _courses[course.Id] = course;
    }

    public void AddCoach(Coach coach)
    {
        _coaches[coach.Id] = coach;
    }

    public Course? GetById(Guid id)
    {
        return _courses.TryGetValue(id, out var course) ? course : null;
    }

    public Coach? GetCoachById(Guid id)
    {
        return _coaches.TryGetValue(id, out var coach) ? coach : null;
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

    public void Confirm(Guid id)
    {
        if (_courses.TryGetValue(id, out var course))
        {
            course.Confirm();
        }
    }

    public void AssignCoach(Guid courseId, Coach coach)
    {
        if (_courses.TryGetValue(courseId, out var course))
        {
            course.AssignCoach(coach);
            coach.AssignCourse(course);
        }
    }
    public List<Course> GetConfirmed()
    {
        return _courses.Values
            .Where(c => c.Status == CourseStatus.PendingForCoach)
            .ToList();
    }
}