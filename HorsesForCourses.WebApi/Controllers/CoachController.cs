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

    public CoachController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public IActionResult CreateCoach([FromBody] CreateCoachDto dto)
    {
        var coach = new Coach(dto.Name, dto.Email);
        _repository.Add(coach);

        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, coach);
    }

    [HttpGet("{id}")]
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpGet]
    public ActionResult<List<Coach>> GetAll()
    {
        return Ok(_repository.GetAll());
    }
    
    [HttpPost("{id}/skills")]
    public IActionResult UpdateSkills(Guid id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
            return NotFound();

        _repository.UpdateSkills(id, dto.Skills);
        return Ok(coach);
    }
}