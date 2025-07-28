namespace HorsesForCourses.Core;

public class Session
{
    public Timeslot Timeslot { get; }
    public Guid CoachId { get; }
    public Guid CourseId { get; }

    public Session(Guid coachId, Guid courseId, Timeslot timeslot)
    {
        CoachId = coachId;
        CourseId = courseId;
        Timeslot = timeslot;
    }
}