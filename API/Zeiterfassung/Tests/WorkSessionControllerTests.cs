using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Zeiterfassung.Controllers;
using Zeiterfassung.Data;
using Zeiterfassung.DTO;
using Zeiterfassung.Models;

namespace Zeiterfassung.Tests;

public class WorkSessionControllerTests
{
    private readonly WorkSessionController _controller;
    private readonly Mock<MyDbContext> _mockContext = new Mock<MyDbContext>();
    private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();
    private readonly MyDbContext _context;

    public WorkSessionControllerTests()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new MyDbContext(options);
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.SetupGet(m => m.Value)
            .Returns("'uPjVORzudNtwQ_9lbW9d7j3q_t3sQHQbEcX50Kgh6rD1r1oMrrx_FznCxek9bac08W4nCZXUHrX3abtb6sxdiw'\\n");
        _mockConfiguration.Setup(c => c.GetSection("JWT:Token")).Returns(mockSection.Object);
        _controller = new WorkSessionController(_mockContext.Object);
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
        };
    }

    /// <summary>
    /// Tests whether the method returns Unauthorized when the user ID is invalid
    /// </summary>
    [Fact]
    public void SaveWorkSession_ReturnsUnauthorized_WhenUserIdIsInvalid()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        var result = _controller.SaveWorkSession(new WorkSessionDto());
        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        Assert.NotNull(unauthorizedResult);
    }

    /// <summary>
    /// Ensures that an OkObjectResult is returned when a work session is saved successfully
    /// </summary>
    [Fact]
    public void SaveWorkSession_ReturnsOk_WhenSessionIsSavedSuccessfully()
    {
        var dto = new WorkSessionDto
        {
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddHours(1),
            LocationId = 1,
            ProjectId = 1
        };
        var data = new List<WorkSession>().AsQueryable();
        var mockSet = new Mock<DbSet<WorkSession>>();
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.GetEnumerator()).Returns(data.GetEnumerator());
        _mockContext.Setup(m => m.WorkSessions).Returns(mockSet.Object);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);
        var result = _controller.SaveWorkSession(dto) as OkObjectResult;
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Verifies that a BadRequestObjectResult is returned when the model is invalid
    /// </summary>
    [Fact]
    public void SaveWorkSession_ReturnsBadRequest_WhenModelIsInvalid()
    {
        var dto = new WorkSessionDto
        {
            Start = DateTime.UtcNow,
            End = null,
            LocationId = 0,
            ProjectId = 1
        };
        _controller.ModelState.AddModelError("Error", "Invalid model state");
        var data = new List<WorkSession>().AsQueryable();
        var mockSet = new Mock<DbSet<WorkSession>>();
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.GetEnumerator()).Returns(data.GetEnumerator());
        _mockContext.Setup(m => m.WorkSessions).Returns(mockSet.Object);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);
        var result = _controller.SaveWorkSession(dto);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult);
    }

    /// <summary>
    /// Tests whether an UnauthorizedResult is returned when user ID claims are invalid
    /// </summary>
    [Fact]
    public void GetCurrentWorkSession_ReturnsUnauthorized_WhenUserIdClaimIsInvalid()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        var result = _controller.GetCurrentWorkSession();
        Assert.IsType<UnauthorizedResult>(result);
    }

    /// <summary>
    /// Checks whether a NotFoundObjectResult is returned if there is no active work session for the current day
    /// </summary>
    [Fact]
    public void GetCurrentWorkSession_ReturnsNotFound_WhenNoActiveWorkSessionForToday()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        var data = new List<WorkSession>().AsQueryable();
        var mockSet = new Mock<DbSet<WorkSession>>();
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.GetEnumerator()).Returns(data.GetEnumerator());
        _mockContext.Setup(m => m.WorkSessions).Returns(mockSet.Object);
        var result = _controller.GetCurrentWorkSession();
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Ensures that an OkObjectResult is returned when there is an active work session for the current day
    /// </summary>
    [Fact]
    public void GetCurrentWorkSession_ReturnsOk_WhenActiveWorkSessionExistsForToday()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        var data = new List<WorkSession>
        {
            new WorkSession
            {
                UserId = 1,
                Start = DateTime.UtcNow,
                End = null
            }
        }.AsQueryable();
        var mockSet = new Mock<DbSet<WorkSession>>();
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.GetEnumerator()).Returns(data.GetEnumerator());
        _mockContext.Setup(m => m.WorkSessions).Returns(mockSet.Object);
        var result = _controller.GetCurrentWorkSession();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var workSession = Assert.IsType<WorkSession>(okResult.Value);
        Assert.Equal(1, workSession.UserId);
        Assert.Null(workSession.End);
    }

    /// <summary>
    /// Determines if the correct work hours are returned for a valid user based on weekly calculations
    /// </summary>
    [Fact]
    public void GetWeeklyWorkHours_ReturnsCorrectHoursForValidUser()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        var mockUsers = new Mock<DbSet<User>>();
        var userData = new List<User>
        {
            new User { Id = 1, WorkingHoursWeekly = 40 }
        }.AsQueryable();
        mockUsers.As<IQueryable<User>>().Setup(m =>
            m.Provider).Returns(userData.Provider);
        mockUsers.As<IQueryable<User>>().Setup(m =>
            m.Expression).Returns(userData.Expression);
        mockUsers.As<IQueryable<User>>().Setup(m =>
            m.ElementType).Returns(userData.ElementType);
        mockUsers.As<IQueryable<User>>().Setup(m =>
            m.GetEnumerator()).Returns(userData.GetEnumerator());
        _mockContext.Setup(m => m.Users).Returns(mockUsers.Object);
        var today = DateTime.UtcNow;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(7);
        var mockWorkSessions = new Mock<DbSet<WorkSession>>();
        var workSessionData = new List<WorkSession>
        {
            new WorkSession
            {
                UserId = 1,
                Start = weekStart.AddHours(9),
                End = weekStart.AddHours(17)
            }
        }.AsQueryable();
        var regulationData = new List<Regulation>
        {
            new Regulation { WorkingHours = 6, BreakTime = 30 },
            new Regulation { WorkingHours = 9, BreakTime = 45 }
        }.AsQueryable();
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(workSessionData.Provider);
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(workSessionData.Expression);
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(workSessionData.ElementType);
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
                m.GetEnumerator())
            .Returns(workSessionData.GetEnumerator());
        _mockContext.Setup(m => m.WorkSessions).Returns(mockWorkSessions.Object);
        var mockRegulations = new Mock<DbSet<Regulation>>();
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
            m.Provider).Returns(regulationData.Provider);
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
            m.Expression).Returns(regulationData.Expression);
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
            m.ElementType).Returns(regulationData.ElementType);
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
                m.GetEnumerator())
            .Returns(regulationData.GetEnumerator());
        _mockContext.Setup(m => m.Regulations).Returns(mockRegulations.Object);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);
        _mockContext.Setup(m =>
            m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var result = _controller.GetWeeklyWorkHours() as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var response = result.Value as dynamic;
        Assert.Equal(7.5, response.WorkedHours);
        Assert.Equal(40, response.WorkingHoursWeekly);
    }

    /// <summary>
    /// Tests whether an UnauthorizedResult is returned when the user ID is invalid
    /// </summary>
    [Fact]
    public void GetWeeklyWorkHours_ReturnsUnauthorized_WhenUserIdIsInvalid()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        var result = _controller.GetWeeklyWorkHours();
        Assert.IsType<UnauthorizedResult>(result);
    }
    
    /// <summary>
    /// Ensures that averages are returned when data is available for the specified month and year
    /// </summary>
    [Fact]
    public void GetMonthlyAverages_ReturnsAverages_WhenDataExists()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        var mockData = new List<WorkSession>
        {
            new WorkSession
            {
                UserId = 1,
                Start = new DateTime(2023, 5, 1, 9, 0, 0),
                End = new DateTime(2023, 5, 1, 17, 0, 0)
            }
        }.AsQueryable();
        var mockSet = new Mock<DbSet<WorkSession>>();
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(mockData.Provider);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(mockData.Expression);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(mockData.ElementType);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.GetEnumerator()).Returns(mockData.GetEnumerator());
        _mockContext.Setup(m => m.WorkSessions).Returns(mockSet.Object);
        var result = _controller.GetMonthlyAverages(2023, 5) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var averages = (result.Value as dynamic).AverageStartTime;
        Assert.Equal(9, averages);
    }

    /// <summary>
    /// Verifies that a NotFoundObjectResult is returned when no session data is found for the specified month and year
    /// </summary>
    [Fact]
    public void GetMonthlyAverages_ReturnsNotFound_WhenNoSessionsFound()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        var emptyData = new List<WorkSession>().AsQueryable();
        var mockSet = new Mock<DbSet<WorkSession>>();
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(emptyData.Provider);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(emptyData.Expression);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(emptyData.ElementType);
        mockSet.As<IQueryable<WorkSession>>().Setup(m =>
            m.GetEnumerator()).Returns(emptyData.GetEnumerator());
        _mockContext.Setup(m => m.WorkSessions).Returns(mockSet.Object);
        var result = _controller.GetMonthlyAverages(2023, 5);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void DownloadMonthlyWorkSessions_ReturnsUnauthorized_WhenUserIdIsInvalid()
    {
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        var result = _controller.DownloadMonthlyWorkSessions(2023, 5);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    /// <summary>
    /// Tests whether an UnauthorizedObjectResult is returned when the user ID is invalid
    /// </summary>
    [Fact]
    public void DownloadMonthlyWorkSessions_ReturnsFileResult_WhenDataExists()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        var mockWorkSessions = new Mock<DbSet<WorkSession>>();
        var workSessionData = new List<WorkSession>
        {
            new WorkSession
            {
                UserId = 1,
                Start = new DateTime(2023, 5, 15),
                End = new DateTime(2023, 5, 15, 8, 0, 0),
                Location = new Location { Description = "Office" },
                Project = new Project { Name = "Development" }
            }
        }.AsQueryable();
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
            m.Provider).Returns(workSessionData.Provider);
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
            m.Expression).Returns(workSessionData.Expression);
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
            m.ElementType).Returns(workSessionData.ElementType);
        mockWorkSessions.As<IQueryable<WorkSession>>().Setup(m =>
            m.GetEnumerator()).Returns(workSessionData.GetEnumerator());
        var regulationData = new List<Regulation>
        {
            new Regulation { WorkingHours = 6, BreakTime = 30 },
            new Regulation { WorkingHours = 9, BreakTime = 45 }
        }.AsQueryable();
        var mockRegulations = new Mock<DbSet<Regulation>>();
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
            m.Provider).Returns(regulationData.Provider);
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
            m.Expression).Returns(regulationData.Expression);
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
            m.ElementType).Returns(regulationData.ElementType);
        mockRegulations.As<IQueryable<Regulation>>().Setup(m =>
                m.GetEnumerator())
            .Returns(regulationData.GetEnumerator());
        _mockContext.Setup(m => m.Regulations).Returns(mockRegulations.Object);
        _mockContext.Setup(m => m.WorkSessions).Returns(mockWorkSessions.Object);
        var result = _controller.DownloadMonthlyWorkSessions(2023, 5) as FileContentResult;
        Assert.NotNull(result);
        Assert.Equal("text/csv", result.ContentType);
        Assert.Contains("WorkSessions-2023-5.csv", result.FileDownloadName);
    }
}