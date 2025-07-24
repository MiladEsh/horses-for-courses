using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Repositories;

public class InMemoryCoachRepository
{
    private readonly Dictionary<Guid, Coach> _coaches = new();

    public void Add(Coach coach)
    {
        _coaches[coach.Id] = coach;
    }

    public Coach? GetById(Guid id)
    {
        return _coaches.TryGetValue(id, out var coach) ? coach : null;
    }

    public List<Coach> GetAll()
    {
        return _coaches.Values.ToList();
    }

    public void UpdateSkills(Guid id, List<string> skills)
    {
        if (_coaches.TryGetValue(id, out var coach))
        {
            var currentSkills = coach.Competences.ToList();
            foreach (var s in currentSkills)
            {
                coach.RemoveCompetence(s);
            }

            foreach (var skill in skills.Distinct())
            {
                coach.AddCompetence(skill);
            }
        }
    }
}