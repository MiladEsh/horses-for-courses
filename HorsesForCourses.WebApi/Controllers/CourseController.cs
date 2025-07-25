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

    public CourseController(InMemoryCourseRepository repository)
    {
        _repository = repository;
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
            _repository.AddTimeslot(id, timeslot);
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}