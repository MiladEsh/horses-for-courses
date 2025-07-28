namespace HorsesForCourses.WebApi.Dtos;

public class ScheduleSessionDto
{
    public Guid CourseId { get; set; }
    public Weekday Day { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}