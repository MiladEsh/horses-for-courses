namespace HorsesForCourses.Core;

public class Session
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Timeslot Timeslot { get; private set; } = default!;
    public Guid CoachId { get; private set; }
    public Guid CourseId { get; private set; }

    private Session() { }

    public Session(Guid coachId, Guid courseId, Timeslot timeslot)
    {
        if (timeslot == null)
            throw new ArgumentNullException(nameof(timeslot));

        CoachId = coachId;
        CourseId = courseId;
        Timeslot = timeslot;
    }
}