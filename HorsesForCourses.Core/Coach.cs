namespace HorsesForCourses.Core;

public class Coach
{
    public Guid Id { get; } = Guid.NewGuid();

    private readonly List<string> _competences = new();
    private readonly List<Timeslot> _availabilities = new();
    private readonly List<Guid> _assignedCourseIds = new();

    public string Name { get; }
    public string Email { get; }

    public IReadOnlyList<string> Competences => _competences.AsReadOnly();
    public IReadOnlyList<Timeslot> Availabilities => _availabilities.AsReadOnly();
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

    public void AddCompetence(string competence)
    {
        if (!_competences.Contains(competence))
            _competences.Add(competence);
    }

    public void RemoveCompetence(string competence)
    {
        _competences.Remove(competence);
    }

    public void AddAvailability(Timeslot slot)
    {
        if (!_availabilities.Contains(slot))
            _availabilities.Add(slot);
    }

    public bool CanTeach(Course course)
    {
        var missingCompetences = course.RequiredCompetences
            .Where(rc => !_competences.Contains(rc))
            .ToList();

        if (missingCompetences.Any())
            return false;

        foreach (var courseSlot in course.Timeslots)
        {
            bool isAvailable = _availabilities.Any(a =>
                a.Day == courseSlot.Day &&
                a.Start <= courseSlot.Start &&
                a.End >= courseSlot.End
            );

            if (!isAvailable)
                return false;
        }

        return true;
    }

    public void AssignCourse(Course course)
    {
        if (!CanTeach(course))
            throw new InvalidOperationException("Coach is not eligible for this course");

        if (_assignedCourseIds.Contains(course.Id))
            return;

        _assignedCourseIds.Add(course.Id);
    }
}