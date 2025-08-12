using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Dtos;
using HorsesForCourses.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly InMemoryCourseRepository _repository;
    private readonly InMemoryCoachRepository _coachRepository;

    public CourseController(InMemoryCourseRepository repository, InMemoryCoachRepository coachRepository)
    {
        _repository = repository;
        _coachRepository = coachRepository;
    }

    [HttpPost]
    public IActionResult CreateCourse([FromBody] CreateCourseDto dto)
    {
        var course = new Course(dto.Name, dto.StartDate, dto.EndDate);
        _repository.Add(course);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }

    [HttpGet("{id}")]
    public ActionResult<CourseDetailsDto> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        if (course is null) return NotFound();

        var dto = ToCourseDetailsDto(course);
        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<List<CourseListItemDto>> GetAll()
    {
        var list = _repository
            .GetAll()
            .Select(c => new CourseListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                HasSchedule = c.Timeslots.Count > 0,
                HasCoach = c.AssignedCoach != null
            })
            .ToList();

        return Ok(list);
    }

    [HttpPost("{id}/confirm")]
    public IActionResult ConfirmCourse(Guid id)
    {
        var course = _repository.GetById(id);
        if (course is null) return NotFound();

        try
        {
            course.Confirm();
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/timeslots")]
    public IActionResult AddTimeslot(Guid id, [FromBody] TimeslotDto dto)
    {
        var course = _repository.GetById(id);
        if (course is null) return NotFound();

        try
        {
            var timeslot = new Timeslot(dto.Day, dto.Start, dto.End);
            course.AddTimeslot(timeslot);
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost("{id}/skills")]
    public IActionResult UpdateSkills(Guid id, [FromBody] UpdateCourseSkillsDto dto)
    {
        var course = _repository.GetById(id);
        if (course is null) return NotFound();

        course.ReplaceRequiredCompetences(dto.Skills);
        return Ok(course);
    }


    [HttpPost("{id}/assign-coach")]
    public IActionResult AssignCoach(Guid id, [FromBody] AssignCoachDto dto)
    {
        var course = _repository.GetById(id);
        if (course is null)
            return NotFound(new { message = "Course not found" });

        var coach = _coachRepository.GetById(dto.CoachId);
        if (coach is null)
            return NotFound(new { message = "Coach not found" });

        try
        {
            course.AssignCoach(coach);
            coach.AssignCourse(course);
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static CourseDetailsDto ToCourseDetailsDto(Course c)
    {
        return new CourseDetailsDto
        {
            Id = c.Id,
            Name = c.Name,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Skills = c.RequiredCompetences.ToList(),
            Timeslots = c.Timeslots.Select(ts => new TimeslotResponseDto
            {
                Day = ts.Day,
                Start = ts.Start.Hour,
                End = ts.End.Hour
            }).ToList(),
            Coach = c.AssignedCoach is null
                ? null
                : new SimpleCoachDto { Id = c.AssignedCoach.Id, Name = c.AssignedCoach.Name }
        };
    }
}

public class CourseListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasCoach { get; set; }
}

public class CourseDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<TimeslotResponseDto> Timeslots { get; set; } = new();
    public SimpleCoachDto? Coach { get; set; }
}

public class TimeslotResponseDto
{
    public Weekday Day { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}

public class SimpleCoachDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}