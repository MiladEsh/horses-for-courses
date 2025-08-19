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
        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, ToDto(coach));
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDto> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(ToDto(coach));
    }

    [HttpGet]
    public ActionResult<PagedResult<CoachListItemDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = _repository.GetPaged(
            page, pageSize,
            c => new CoachListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                NumberOfCoursesAssignedTo = c.AssignedCourseIds.Count
            },
            orderBy: q => q.OrderBy(c => c.Name).ThenBy(c => c.Id));

        return Ok(new PagedResult<CoachListItemDto>(items, total, page, pageSize));
    }

    [HttpPost("{id}/skills")]
    public IActionResult UpdateSkills(Guid id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
            return NotFound();

        coach.ReplaceCompetences(dto.Skills);
        return Ok(ToDto(coach));
    }

    [HttpPost("{id}/availability")]
    public IActionResult AddAvailability(Guid id, [FromBody] TimeslotDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
            return NotFound();

        try
        {
            var slot = new Timeslot(dto.Day, dto.Start, dto.End);
            coach.AddAvailability(slot);
            return Ok(ToDto(coach));
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

    public class CoachListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int NumberOfCoursesAssignedTo { get; set; }
    }

    private static CoachDto ToDto(Coach coach)
    {
        return new CoachDto
        {
            Id = coach.Id,
            Name = coach.Name,
            Email = coach.Email,
            Competences = coach.Competences.ToList()
        };
    }
}