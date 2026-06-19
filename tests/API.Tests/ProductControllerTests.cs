using CRNAssessment.API.Controllers;
using CRNAssessment.Application.DTOs;
using CRNAssessment.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CRNAssessment.API.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var productDto = new ProductDto { Id = productId, ProductName = "Test Product" };

            _mockProductService.Setup(s => s.GetProductById(productId))
                .ReturnsAsync(productDto);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedProduct = okResult.Value.Should().BeAssignableTo<ProductDto>().Subject;
            
            returnedProduct.Id.Should().Be(productId);
            returnedProduct.ProductName.Should().Be("Test Product");
        }
    }
}
