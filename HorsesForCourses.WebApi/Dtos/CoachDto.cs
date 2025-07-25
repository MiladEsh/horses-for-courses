namespace HorsesForCourses.WebApi.Dtos;

public class CoachDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<string> Competences { get; set; } = new();
}