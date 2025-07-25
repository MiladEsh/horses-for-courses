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