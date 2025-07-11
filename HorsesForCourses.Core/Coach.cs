public class Coach
{
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
        if (_competences.Contains(competence))
            return;

        _competences.Add(competence);
    }

    public void RemoveCompetence(string competence)
    {
        _competences.Remove(competence);
    }

    public void AddAvailability(Timeslot slot)
    {
        if (_availabilities.Contains(slot))
            return;

        _availabilities.Add(slot);
    }

    public bool CanTeach(Course course)
    {
        var missingCompetences = course.RequiredCompetences
            .Where(rc => _competences.Contains(rc) == false)
            .ToList();

        if (missingCompetences.Any())
            return false;

        foreach (var courseSlot in course.Timeslots)
        {
            bool isAvailable = false;

            foreach (var a in _availabilities)
            {
                bool sameDay = a.Day == courseSlot.Day;
                bool startsOnTime = a.Start <= courseSlot.Start;
                bool endsOnTime = a.End >= courseSlot.End;

                if (sameDay)
                {
                    if (startsOnTime)
                    {
                        if (endsOnTime)
                        {
                            isAvailable = true;
                            break;
                        }
                    }
                }
            }

            if (isAvailable == false)
                return false;
        }

        return true;
    }

    public void AssignCourse(Course course)
    {
        if (CanTeach(course))
        {
            if (_assignedCourseIds.Contains(course.Id))
                return;

            _assignedCourseIds.Add(course.Id);
            return;
        }

        throw new InvalidOperationException("Coach is not eligible for this course");
    }
}