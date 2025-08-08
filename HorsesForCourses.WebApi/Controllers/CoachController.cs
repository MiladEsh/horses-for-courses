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
    public ActionResult<List<CoachDto>> GetAll()
    {
        var coaches = _repository.GetAll().Select(ToDto).ToList();
        return Ok(coaches);
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