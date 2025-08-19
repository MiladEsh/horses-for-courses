namespace HorsesForCourses.WebApi.Dtos;

public sealed record PageRequest(int PageNumber = 1, int PageSize = 25)
{
    public int Page => PageNumber < 1 ? 1 : PageNumber;
    public int Size => PageSize is < 1 ? 1 : (PageSize > 500 ? 500 : PageSize); // guardrails
}