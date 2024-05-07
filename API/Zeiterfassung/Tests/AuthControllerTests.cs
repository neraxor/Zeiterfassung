using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Zeiterfassung.Controllers;
using Zeiterfassung.Data;
using Zeiterfassung.Models;
using Zeiterfassung.Data.DTO;
using Zeiterfassung.DTO;

namespace Zeiterfassung.Tests;

public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly MyDbContext _context;
    private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();

    public AuthControllerTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") 
            .Options;

        _context = new MyDbContext(options);
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.SetupGet(m => m.Value)
            .Returns("'uPjVORzudNtwQ_9lbW9d7j3q_t3sQHQbEcX50Kgh6rD1r1oMrrx_FznCxek9bac08W4nCZXUHrX3abtb6sxdiw'\\n");
        _mockConfiguration.Setup(c => c.GetSection("JWT:Token")).Returns(mockSection.Object);

        _controller = new AuthController(_mockConfiguration.Object, _context);
    }
    
    /// <summary>
    /// Verifies that registering a duplicate user results in a BadRequest response
    /// </summary>
    [Fact]
    public void Register_DuplicateUser_ReturnsBadRequest()
    {
        var controller = new AuthController(_mockConfiguration.Object, _context);
        var newUser = new UserRegisterDto
        {
            Email = "test@example.com",
            Username = "newUser",
            Password = "123456"
        };
        var firstResult = controller.Register(newUser) as OkObjectResult;
        Assert.NotNull(firstResult);
        Assert.Equal(200, firstResult.StatusCode);
        var secondResult = controller.Register(newUser) as BadRequestObjectResult;
        Assert.NotNull(secondResult);
        Assert.Equal(400, secondResult.StatusCode);  
        _context.Database.EnsureDeleted();
    }

    /// <summary>
    /// Tests that a new, valid user can be successfully registered with an Ok response
    /// </summary>
    [Fact]
    public void Register_ValidUser_ReturnsOkResult()
    { 
        var controller = new AuthController(_mockConfiguration.Object, _context);
        var newUser = new UserRegisterDto
        {
            Email = "test@example.com",
            Username = "newUser",
            Password = "123456"
        };
        var result = controller.Register(newUser) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        _context.Database.EnsureDeleted();
    }

    /// <summary>
    /// Verifies that registering a user with missing required fields results in a BadRequest response
    /// </summary>
    [Theory]
    [InlineData("", "test@example.com", "123456")]
    [InlineData("newUser", "", "123456")]
    [InlineData("newUser", "badEmail", "123456")]
    [InlineData("newUser", "test@example.com", "")]
    [InlineData("newUser", "test@example.com", "1235")]
    public void Register_RequiredFieldsMissing_ReturnsBadRequest(string username, string email, string password)
    {
        var controller = new AuthController(_mockConfiguration.Object, _context);
        controller.ModelState.Clear();  
        var newUser = new UserRegisterDto
        {
            Email = email,
            Username = username,
            Password = password
        };
        var context = new ValidationContext(newUser, null, null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(newUser, context, validationResults, true);
        if (!isValid)
        {
            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.First(),
                    validationResult.ErrorMessage??"");
            }
        }
        var result = controller.Register(newUser) as BadRequestObjectResult;
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Invalid model state", result.Value?.ToString());
    }
    
    /// <summary>
    /// Tests that a successful login returns an Ok response with a token
    /// </summary>
    [Fact]
    public void Login_Successful_ReturnsOkWithToken()
    {
        var newUser = new UserRegisterDto
        {
            Email = "test@example.com",
            Username = "validUser",
            Password = "password"
        };
        var controller = new AuthController(_mockConfiguration.Object, _context);
        controller.Register(newUser);
        var user = new UserLoginDto()
        {
            Username = "validUser",
            Password = "password"
        };
        var result = controller.Login(user) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
    
    /// <summary>
    /// Verifies that attempting to log in with a non-existent username results in a BadRequest response
    /// </summary>
    [Fact]
    public void Login_UserNotFound_ReturnsBadRequest()
    {
        var loginDto = new UserLoginDto { Username = "nonexistentUser", Password = "password" };
        var result = _controller.Login(loginDto) as BadRequestObjectResult;
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("User not found", result.Value);
    }
    
    /// <summary>
    /// Tests that attempting to log in with an incorrect password results in a BadRequest response
    /// </summary>
    [Fact]
    public void Login_InvalidPassword_ReturnsBadRequest()
    {
        var user = new User { Username = "validUser", Email = "user@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
        _context.Users.Add(user);
        _context.SaveChanges();
        var loginDto = new UserLoginDto { Username = "validUser", Password = "wrongPassword" };
        var result = _controller.Login(loginDto) as BadRequestObjectResult;
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Invalid password", result.Value);
    }
    
    /// <summary>
    /// Verifies that attempting to log in with missing username or password results in a BadRequest response
    /// </summary>
    [Theory]
    [InlineData("", "password")]
    [InlineData("validUser", "")]
    public void Login_MissingData_ReturnsBadRequest(string username, string password)
    {
        var loginDto = new UserLoginDto { Username = username, Password = password };
        var context = new ValidationContext(loginDto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(loginDto, context, results, true);
        if (!isValid)
        {
            foreach (var validationResult in results)
            {
                _controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage??"");
            }
        }
        var result = _controller.Login(loginDto) as BadRequestObjectResult;
        Assert.False(isValid);
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
}
