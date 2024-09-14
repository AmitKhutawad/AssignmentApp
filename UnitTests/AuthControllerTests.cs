using Moq;
using FluentAssertions;
using MediatR;
using AssignmentApp.Controllers;
using Application.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        // Initialize the Mediator mock and the controller
        _mediatorMock = new Mock<IMediator>();
        _authController = new AuthController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Login_Should_Return_Token_On_Success()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Username = "admin1",
            Password = "password1"
        };
        var token = "Token for admin";

        // Mock the mediator to return a valid token when the LoginCommand is sent
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act
        var result = await _authController.Login(loginCommand);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(200);
        okResult?.Value.Should().BeEquivalentTo(new { token });
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_On_Invalid_Credentials()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Username = "invalidUser",
            Password = "wrongPassword"
        };

        // Mock the mediator to return null, simulating invalid credentials
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null);

        // Act
        var result = await _authController.Login(loginCommand);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult?.StatusCode.Should().Be(401);
        unauthorizedResult?.Value.Should().Be("Invalid credentials.");
    }
}
