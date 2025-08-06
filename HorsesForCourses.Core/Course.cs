namespace HorsesForCourses.Core;

public class Course
{
    private readonly List<Timeslot> _timeslots = new();
    private readonly List<string> _requiredCompetences = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public CourseStatus Status { get; private set; } = CourseStatus.PendingForTimeslots;
    public IReadOnlyList<Timeslot> Timeslots => _timeslots.AsReadOnly();
    public IReadOnlyList<string> RequiredCompetences => _requiredCompetences.AsReadOnly();
    public Coach? AssignedCoach { get; private set; }

    private Course() { }

    public Course(string name, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        if (startDate > endDate)
            throw new ArgumentException("Start date must be before or equal to end date");

        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void AddTimeslot(Timeslot timeslot)
    {
        if (Status == CourseStatus.PendingForTimeslots)
        {
            _timeslots.Add(timeslot);
            return;
        }

        throw new InvalidOperationException("Cannot add timeslots after confirmation");
    }

    public void RemoveTimeslot(Timeslot timeslot)
    {
        if (Status == CourseStatus.PendingForTimeslots)
        {
            _timeslots.Remove(timeslot);
            return;
        }

        throw new InvalidOperationException("Cannot remove timeslots after confirmation");
    }

    public void AddRequiredCompetence(string competence)
    {
        if (Status == CourseStatus.Confirmed)
            throw new InvalidOperationException("Cannot change competences after confirmation");

        if (_requiredCompetences.Contains(competence))
            return;

        _requiredCompetences.Add(competence);
    }

    public void RemoveRequiredCompetence(string competence)
    {
        if (Status == CourseStatus.Confirmed)
            throw new InvalidOperationException("Cannot change competences after confirmation");

        _requiredCompetences.Remove(competence);
    }

    public void Confirm()
    {
        if (_timeslots.Count > 0)
        {
            double totalMinutes = 0;

            foreach (var ts in _timeslots)
            {
                totalMinutes += ts.Duration.TotalMinutes;
            }

            if (totalMinutes >= 60)
            {
                Status = CourseStatus.PendingForCoach;
                return;
            }

            throw new InvalidOperationException("Total duration of timeslots must be at least 1 hour");
        }

        throw new InvalidOperationException("At least one timeslot is required");
    }

    public void AssignCoach(Coach coach)
    {
        if (Status == CourseStatus.PendingForCoach)
        {
            if (!RequiredCompetences.All(skill => coach.Competences.Contains(skill)))
                throw new InvalidOperationException("Coach does not have all required competences");

            if (!coach.IsAvailable(this))
                throw new InvalidOperationException("Coach is not available for all timeslots");

            AssignedCoach = coach;
            Status = CourseStatus.Confirmed;
            coach.AssignCourse(this);
            return;
        }

        throw new InvalidOperationException("Course must be confirmed before assigning coach");
    }

    public void ReplaceRequiredCompetences(List<string> newCompetences)
    {
        if (Status == CourseStatus.Confirmed)
            throw new InvalidOperationException("Cannot change competences after confirmation");

        _requiredCompetences.Clear();

        foreach (var competence in newCompetences.Distinct())
        {
            _requiredCompetences.Add(competence);
        }
    }
}