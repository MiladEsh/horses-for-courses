namespace HorsesForCourses.WebApi.Dtos;

public class CreateCourseDto
{
    public required string Name { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
}