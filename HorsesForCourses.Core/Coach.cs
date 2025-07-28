namespace HorsesForCourses.Core;

public class Coach
{
    private readonly List<Timeslot> _availability = new();
    private readonly List<string> _competences = new();
    private readonly List<Guid> _assignedCourseIds = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }
    public string Email { get; }
    public IReadOnlyList<Timeslot> Availability => _availability.AsReadOnly();
    public IReadOnlyList<string> Competences => _competences.AsReadOnly();
    public IReadOnlyList<Guid> AssignedCourseIds => _assignedCourseIds.AsReadOnly();

    public Coach(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        Name = name;
        Email = email;
    }

    public void AddAvailability(Timeslot timeslot)
    {
        _availability.Add(timeslot);
    }

    public void AddCompetence(string competence)
    {
        if (!_competences.Contains(competence))
            _competences.Add(competence);
    }

    public void ReplaceCompetences(List<string> competences)
    {
        _competences.Clear();

        foreach (var comp in competences.Distinct())
        {
            _competences.Add(comp);
        }
    }

    public bool CanTeach(Course course)
    {
        var allSkillsMatch = course.RequiredCompetences.All(c => _competences.Contains(c));
        var allSlotsMatch = course.Timeslots.All(courseSlot =>
            _availability.Any(av => av.Day == courseSlot.Day &&
                                    av.Start <= courseSlot.Start &&
                                    av.End >= courseSlot.End));
        return allSkillsMatch && allSlotsMatch;
    }

    public void AssignCourse(Course course)
    {
        if (!CanTeach(course))
            throw new InvalidOperationException("Coach is not eligible to teach this course");

        if (!_assignedCourseIds.Contains(course.Id))
            _assignedCourseIds.Add(course.Id);
    }

    public bool IsAvailable(Course course)
    {
        return course.Timeslots.All(courseSlot =>
            _availability.Any(coachSlot => coachSlot.OverlapsWith(courseSlot)));
    }
}