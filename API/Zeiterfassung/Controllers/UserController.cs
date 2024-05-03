using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Zeiterfassung.Data;
using Zeiterfassung.Models;

namespace Zeiterfassung.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly MyDbContext _context;

    public UserController(MyDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// get all locations of a specific user
    /// </summary>
    /// <returns>list of locations with id,name</returns>
    [HttpGet("GetLocations")]
    public ActionResult<IEnumerable<Location>> GetLocationsByUserId()
    {
        int userIdClaim = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var tempUserId)
            ? tempUserId
            : 0;
        if (userIdClaim == 0)
        {
            return Unauthorized();
        }
        var locations = _context.Locations.Where(l => l.UserId == userIdClaim).ToList();
        return Ok(locations);
    }
    /// <summary>
    /// get all projects of a specific user
    /// </summary>
    /// <returns>list of projects with name,description,id</returns>
    [HttpGet("GetProjects")]
    public ActionResult<IEnumerable<Project>> GetProjectsByUserId()
    {
        int userIdClaim = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var tempUserId)
            ? tempUserId
            : 0;
        if (userIdClaim == 0)
        {
            return Unauthorized();
        }
        var projects = _context.Projects.Where(p => p.UserId == userIdClaim).ToList();
        return Ok(projects);
    }
}