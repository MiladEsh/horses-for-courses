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
    private readonly InMemorySessionRepository _sessionRepository;

    public CoachController(
        InMemoryCoachRepository repository,
        InMemoryCourseRepository courseRepository,
        InMemorySessionRepository sessionRepository)
    {
        _repository = repository;
        _courseRepository = courseRepository;
        _sessionRepository = sessionRepository;
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

        _repository.UpdateSkills(id, dto.Skills);
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

    [HttpGet("{id}/eligible-courses")]
    public ActionResult<List<Course>> GetEligibleCourses(Guid id)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
            return NotFound(new { message = "Coach not found" });

        var eligibleCourses = _courseRepository
            .GetConfirmed()
            .Where(coach.CanTeach)
            .ToList();

        return Ok(eligibleCourses);
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

        _courseRepository.AssignCoach(course.Id, coach);
        return Ok(course);
    }

    [HttpPost("{id}/sessions")]
    public IActionResult ScheduleSession(Guid id, [FromBody] ScheduleSessionDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
            return NotFound(new { message = "Coach not found" });

        var course = _courseRepository.GetById(dto.CourseId);
        if (course is null)
            return NotFound(new { message = "Course not found" });

        if (!coach.AssignedCourseIds.Contains(course.Id))
            return BadRequest(new { message = "Coach is not assigned to this course" });

        var slot = new Timeslot(dto.Day, dto.Start, dto.End);
        var session = new Session(coach.Id, course.Id, slot);
        _sessionRepository.Add(session);

        return Ok(session);
    }

    [HttpGet("{id}/sessions")]
    public IActionResult GetSessions(Guid id)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
            return NotFound(new { message = "Coach not found" });

        var sessions = _sessionRepository.GetByCoach(id);
        return Ok(sessions);
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