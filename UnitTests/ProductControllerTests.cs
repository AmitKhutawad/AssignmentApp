using Moq;
using MediatR;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using AssignmentApp.Controllers;
using Application.Queries;
using Application.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Domain.Entities;

namespace UnitTests;

public class ProductControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ProductController(_mediatorMock.Object);
    }


    [Fact]
    public async Task GetAllProducts_ReturnsOk_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
    {
        new Product { Id = 1, Name = "Product 1", Price = 100 },
        new Product { Id = 2, Name = "Product 2", Price = 200 }
    };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsNotFound_WhenNoProductsExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Product>());

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedAtAction_WhenProductIsCreated()
    {
        // Arrange
        var command = new CreateProductCommand { Name = "New Product", Price = 100 };
        var createdProductId = 1;

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(createdProductId);

        // Act
        var result = await _controller.CreateProduct(command);

        // Assert
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.ActionName.Should().Be(nameof(_controller.GetProduct));
        createdAtActionResult.RouteValues["id"].Should().Be(createdProductId);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNoContent_WhenProductIsUpdated()
    {
        // Arrange
        var command = new UpdateProductCommand { Id = 1, Name = "Updated Product", Price = 200 };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateProduct(command.Id, command);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var command = new UpdateProductCommand { Id = 1, Name = "Updated Product", Price = 200 };

        // Act
        var result = await _controller.UpdateProduct(2, command); // ID mismatch

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        (result as BadRequestObjectResult)!.Value.Should().Be("Product ID mismatch");
    }
    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenProductIsDeleted()
    {
        // Arrange
        var productId = 1;

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = 1;

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false); // Product not found

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }



}

