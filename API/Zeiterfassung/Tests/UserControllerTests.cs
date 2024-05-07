using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Zeiterfassung.Controllers;
using Zeiterfassung.Data;
using Zeiterfassung.Models;

namespace Zeiterfassung.Tests;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<MyDbContext> _mockContext = new Mock<MyDbContext>();
    private readonly Mock<DbSet<Location>> _mockDbSet = new Mock<DbSet<Location>>();

    public UserControllerTests()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller = new UserController(_mockContext.Object);
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
        };
    }

    /// <summary>
    /// Tests that the method returns a list of locations associated with a user
    /// </summary>
    [Fact]
    public void GetLocationsByUserId_ReturnsLocationsForUser()
    {
        var locations = new List<Location>
        {
            new Location { Id = 1, Description = "Location 1", UserId = 1 },
            new Location { Id = 2, Description = "Location 2", UserId = 1 }
        };
        var queryableLocations = locations.AsQueryable();
        _mockDbSet.As<IQueryable<Location>>().Setup(m => m.Provider).Returns(queryableLocations.Provider);
        _mockDbSet.As<IQueryable<Location>>().Setup(m => m.Expression).Returns(queryableLocations.Expression);
        _mockDbSet.As<IQueryable<Location>>().Setup(m => m.ElementType).Returns(queryableLocations.ElementType);
        _mockDbSet.As<IQueryable<Location>>().Setup(m => m.GetEnumerator()).Returns(() => queryableLocations.GetEnumerator());
        _mockContext.Setup(c => c.Locations).Returns(_mockDbSet.Object);
        var result = _controller.GetLocationsByUserId().Result as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var returnedLocations = result.Value as List<Location>;
        Assert.Equal(2, returnedLocations.Count);
    }
    
    /// <summary>
    /// Tests that the method returns a list of projects associated with a user
    /// </summary>
    [Fact]
    public void GetProjectsByUserId_ReturnsProjectsForUser()
    {
        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Project 1", UserId = 1 },
            new Project { Id = 2, Name = "Project 2", UserId = 1 }
        };
        var queryableProjects = projects.AsQueryable();
        var mockDbSetProjects = new Mock<DbSet<Project>>();
        mockDbSetProjects.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(queryableProjects.Provider);
        mockDbSetProjects.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(queryableProjects.Expression);
        mockDbSetProjects.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(queryableProjects.ElementType);
        mockDbSetProjects.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(() => queryableProjects.GetEnumerator());
        _mockContext.Setup(c => c.Projects).Returns(mockDbSetProjects.Object);
        var result = _controller.GetProjectsByUserId().Result as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var returnedProjects = result.Value as List<Project>;
        Assert.Equal(2, returnedProjects.Count);
    }
    
    /// <summary>
    /// Verifies that an UnauthorizedResult is returned when there is no valid user claim
    /// </summary>
    [Fact]
    public void GetLocationsByUserId_Unauthorized_WhenNoUserClaim()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        var result = _controller.GetLocationsByUserId().Result as UnauthorizedResult;
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedResult>(result);
    }
    
    /// <summary>
    /// Verifies that an UnauthorizedResult is returned when there is no valid user claim
    /// </summary>
    [Fact]
    public void GetProjectsByUserId_Unauthorized_WhenNoUserClaim()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        var result = _controller.GetProjectsByUserId().Result as UnauthorizedResult;
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedResult>(result);
    }
}
