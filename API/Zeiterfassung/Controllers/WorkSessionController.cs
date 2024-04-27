using Microsoft.AspNetCore.Mvc;
using Zeiterfassung.Data;
using Zeiterfassung.DTO;
using Zeiterfassung.Models;

namespace Zeiterfassung.Controllers;


[Route("api/[controller]")]
[ApiController]
public class WorkSessionController : ControllerBase
{
    private readonly MyDbContext _context;

    public WorkSessionController(MyDbContext context)
    {
        _context = context;
    }

    [HttpPost("add")]
    public async Task<ActionResult> Add(WorkSessionDto sessionDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var workSession = new WorkSession
        {
            UserId = sessionDto.UserId,
            Start = sessionDto.Start,
            End = sessionDto.End,
            LocationId = sessionDto.LocationId,
            ProjectId = sessionDto.ProjectId
        };
        _context.WorkSessions.Add(workSession);
        if (await _context.SaveChangesAsync() > 0)
        {
            return Ok("Work session added successfully");
        }
        return BadRequest("Work session could not be added");
    }
}