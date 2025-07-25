using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Dtos;

public class TimeslotDto
{
    public Weekday Day { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}