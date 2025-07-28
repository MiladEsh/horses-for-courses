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
    public ActionResult<Course> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpGet]
    public ActionResult<List<Course>> GetAll()
    {
        return Ok(_repository.GetAll());
    }

    [HttpPost("{id}/confirm")]
    public IActionResult ConfirmCourse(Guid id)
    {
        var course = _repository.GetById(id);
        if (course is null)
            return NotFound();

        try
        {
            _repository.Confirm(id);
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
        if (course is null)
            return NotFound();

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
        if (course is null)
            return NotFound();

        _repository.UpdateSkills(id, dto.Skills);
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
            _repository.AssignCoach(id, coach);
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}