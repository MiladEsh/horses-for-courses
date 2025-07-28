using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Repositories;

public class InMemorySessionRepository
{
    private readonly List<Session> _sessions = new();

    public void Add(Session session)
    {
        _sessions.Add(session);
    }

    public List<Session> GetAll()
    {
        return _sessions;
    }

    public List<Session> GetByCoach(Guid coachId)
    {
        return _sessions.Where(s => s.CoachId == coachId).ToList();
    }

    public List<Session> GetByCourse(Guid courseId)
    {
        return _sessions.Where(s => s.CourseId == courseId).ToList();
    }
}