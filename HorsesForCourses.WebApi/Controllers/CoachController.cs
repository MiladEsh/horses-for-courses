using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Dtos;
using HorsesForCourses.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("coaches")]
public class CoachController : ControllerBase
{
    private readonly InMemoryCoachRepository _repository;
    private readonly InMemoryCourseRepository _courseRepository;

    public CoachController(
        InMemoryCoachRepository repository,
        InMemoryCourseRepository courseRepository)
    {
        _repository = repository;
        _courseRepository = courseRepository;
    }

    [HttpPost]
    public IActionResult CreateCoach([FromBody] CreateCoachDto dto)
    {
        var coach = new Coach(dto.Name, dto.Email);
        _repository.Add(coach);
        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, ToCoachDto(coach));
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDetailsDto> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        if (coach is null) return NotFound();

        var details = ToCoachDetails(coach, _courseRepository);
        return Ok(details);
    }

    [HttpGet]
    public ActionResult<List<CoachListItemDto>> GetAll()
    {
        var list = _repository
            .GetAll()
            .Select(c => new CoachListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                NumberOfCoursesAssignedTo = c.AssignedCourseIds.Count
            })
            .ToList();

        return Ok(list);
    }

    [HttpPost("{id}/skills")]
    public IActionResult UpdateSkills(Guid id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null) return NotFound();

        coach.ReplaceCompetences(dto.Skills);
        return Ok(ToCoachDto(coach));
    }

    [HttpPost("{id}/availability")]
    public IActionResult AddAvailability(Guid id, [FromBody] TimeslotDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null) return NotFound();

        try
        {
            var slot = new Timeslot(dto.Day, dto.Start, dto.End);
            coach.AddAvailability(slot);
            return Ok(ToCoachDto(coach));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/assign")]
    public IActionResult AssignSelfToCourse(Guid id, [FromBody] AssignCoachDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
            return NotFound(new { message = "Coach not found" });

        var course = _courseRepository.GetById(dto.CourseId);
        if (course is null)
            return NotFound(new { message = "Course not found" });

        if (course.Status != CourseStatus.PendingForCoach)
            return BadRequest(new { message = "Course is not available for assignment" });

        if (!coach.CanTeach(course))
            return BadRequest(new { message = "Coach does not meet the requirements" });

        course.AssignCoach(coach);
        coach.AssignCourse(course);

        return Ok(course);
    }

    private static CoachDto ToCoachDto(Coach coach) => new CoachDto
    {
        Id = coach.Id,
        Name = coach.Name,
        Email = coach.Email,
        Competences = coach.Competences.ToList()
    };

    private static CoachDetailsDto ToCoachDetails(Coach coach, InMemoryCourseRepository courseRepo)
    {
        var courses = coach.AssignedCourseIds
            .Select(cid => courseRepo.GetById(cid))
            .Where(c => c != null)
            .Select(c => new CoachCourseRefDto { Id = c!.Id, Name = c!.Name })
            .ToList();

        return new CoachDetailsDto
        {
            Id = coach.Id,
            Name = coach.Name,
            Email = coach.Email,
            Skills = coach.Competences.ToList(),
            Courses = courses
        };
    }
}

public class CoachListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int NumberOfCoursesAssignedTo { get; set; }
}

public class CoachDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<string> Skills { get; set; } = new();
    public List<CoachCourseRefDto> Courses { get; set; } = new();
}

public class CoachCourseRefDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}