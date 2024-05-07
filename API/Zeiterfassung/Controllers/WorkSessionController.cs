using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Zeiterfassung.Data;
using Zeiterfassung.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Zeiterfassung.DTO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Zeiterfassung.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WorkSessionController : ControllerBase
{
    private readonly MyDbContext _context;

    public WorkSessionController(MyDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// saves a work session
    /// </summary>
    /// <param name="request">worksession data</param>
    /// <returns>string as feedback</returns>
    [HttpPost("SaveWorkSession")]
    public IActionResult SaveWorkSession([FromBody] WorkSessionDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state");
        }
        int userIdClaim = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var tempUserId)
            ? tempUserId
            : 0;
        if (userIdClaim == 0)
        {
            return Unauthorized();
        }
        var currentWorkSession = _context.WorkSessions
            .Where(ws => ws.UserId == userIdClaim
                         && ws.Start.Date == DateTime.UtcNow.Date
                         && ws.End == null)
            .FirstOrDefault();
         if (currentWorkSession != null)
        {
            currentWorkSession.Start = request.Start.ToUniversalTime();
            currentWorkSession.End = request.End?.ToUniversalTime();
            currentWorkSession.LocationId = request.LocationId;
            currentWorkSession.ProjectId = request.ProjectId;
        }
        else
        {
            var workSession = new WorkSession
            {
                UserId = userIdClaim,
                Start = request.Start.ToUniversalTime(),
                End = request.End?.ToUniversalTime(),
                LocationId = request.LocationId,
                ProjectId = request.ProjectId
            };
            _context.WorkSessions.Add(workSession);
        }
        _context.SaveChanges();
        return Ok("Work session saved successfully.");
    }
    /// <summary>
    /// loads the first work session of a user without enddate limited to the current day
    /// </summary>
    /// <returns>worksession</returns>
    [HttpGet("GetCurrentWorkSession")]
    public IActionResult GetCurrentWorkSession()
    {
        int userIdClaim = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var tempUserId)
            ? tempUserId
            : 0;
        if (userIdClaim == 0)
        {
            return Unauthorized();
        }
        var currentWorkSession = _context.WorkSessions
            .Where(ws => ws.UserId == userIdClaim
                         && ws.Start.Date == DateTime.UtcNow.Date
                         && ws.End == null)
            .FirstOrDefault();
        if (currentWorkSession == null)
        {
            return NotFound("No active work session found for today.");
        }
        return Ok(currentWorkSession);
    }
    /// <summary>
    /// sums up the worked hours of a user for the current week reduced by regulations / breaks
    /// </summary>
    /// <returns>workedhours,workingHoursWeekly(max)</returns>
    [HttpGet("GetWeeklyWorkHours")]
    public IActionResult GetWeeklyWorkHours()
    {
        int userIdClaim = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var tempUserId)
            ? tempUserId
            : 0;
        if (userIdClaim == 0)
        {
            return Unauthorized();
        }
        var today = DateTime.UtcNow;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(7);
        var weeklySessions = _context.WorkSessions
            .Where(ws => ws.UserId == userIdClaim
                         && ws.Start >= weekStart
                         && ws.End < weekEnd
                         && ws.End.HasValue)
            .ToList();
        double adjustedWorkHours = 0;
        foreach (var session in weeklySessions)
        {
            double sessionDuration = (session.End.Value - session.Start).TotalHours;
            var regulation = _context.Regulations
                .Where(r => r.WorkingHours <= sessionDuration)
                .OrderByDescending(r => r.WorkingHours)
                .FirstOrDefault();
            if (regulation != null)
            {
                sessionDuration -= regulation.BreakTime / 60.0;
            }
            adjustedWorkHours += sessionDuration;
        }
        return Ok(new
        {
            WorkedHours = Math.Round(adjustedWorkHours, 2),
            WorkingHoursWeekly = _context.Users
                .Where(u => u.Id == userIdClaim)
                .Select(u => u.WorkingHoursWeekly)
                .FirstOrDefault()
        });
    }
    /// <summary>
    /// Provides a Download of the work sessions of a user for a specific month
    /// </summary>
    /// <param name="year">int</param>
    /// <param name="month">int</param>
    /// <returns>csv data</returns>
    [HttpGet("DownloadMonthlyWorkSessions")]
    public IActionResult DownloadMonthlyWorkSessions(int year, int month)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return Unauthorized("Invalid user ID");
        }
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);
        var workSessions = _context.WorkSessions
            .Include(ws => ws.Location)
            .Include(ws => ws.Project)
            .Where(ws => ws.UserId == userId && ws.Start >= startDate && ws.Start < endDate)
            .ToList();
        var csvData = workSessions.Select(ws =>
        {
            double workingHours = ws.End.HasValue ? (ws.End.Value - ws.Start).TotalHours : 0;
            var regulation = _context.Regulations
                .Where(r => r.WorkingHours <= workingHours)
                .OrderByDescending(r => r.WorkingHours)
                .FirstOrDefault();
            if (regulation != null)
            {
                workingHours -= regulation.BreakTime / 60.0;
            }
            return new WorkSessionCsvDto
            {
                WorkSessionId = ws.Id,
                WorkingHours = workingHours,
                Start = ws.Start,
                End = ws.End.HasValue ? ws.End.Value : (DateTime?)null,
                LocationDescription = ws.Location.Description,
                ProjectName = ws.Project.Name
            };
        }).ToList();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
        };
        using (var memoryStream = new MemoryStream())
        using (var writer = new StreamWriter(memoryStream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(csvData);
            writer.Flush();
            return File(memoryStream.ToArray(), 
                "text/csv", $"WorkSessions-{year}-{month}.csv");
        }
    }
    /// <summary>
    /// Generates Statistics for a specific month
    /// avaerage start, end dates
    /// avaerage work hours per weekday
    /// </summary>
    /// <param name="year">int</param>
    /// <param name="month">int</param>
    /// <returns>AverageStartTime,AverageEndTime,WeekdayAverages</returns>
    [HttpGet("GetMonthlyAverages")]
    public IActionResult GetMonthlyAverages(int year, int month)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return Unauthorized("Invalid user ID");
        }
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);
        var sessions = _context.WorkSessions
            .Where(s => s.UserId == userId && s.Start >= startDate && s.Start < endDate && s.End.HasValue)
            .Select(s => new
            {
                DayOfWeek = s.Start.DayOfWeek,
                StartHour = s.Start.Hour + s.Start.Minute / 60.0,
                EndHour = s.End.Value.Hour + s.End.Value.Minute / 60.0,
                Duration = (s.End.Value - s.Start).TotalHours
            })
            .ToList();
        if (!sessions.Any())
            return NotFound("No sessions found for the specified month.");
        var averageStart = Math.Round(sessions.Average(s => s.StartHour), 2);
        var averageEnd = Math.Round(sessions.Average(s => s.EndHour), 2);
        var weekdayAverages = sessions
            .GroupBy(s => s.DayOfWeek)
            .Select(g => new { DayOfWeek = g.Key, 
                AverageHours = Math.Round(g.Average(x => x.Duration), 2) })
            .ToList();
        return Ok(new
        {
            AverageStartTime = averageStart,
            AverageEndTime = averageEnd,
            WeekdayAverages = weekdayAverages
        });
    }
}