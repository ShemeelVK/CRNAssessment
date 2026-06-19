using AutoMapper;
using CRNAssessment.Application.DTOs;
using CRNAssessment.Application.Interfaces;
using CRNAssessment.Application.Services;
using CRNAssessment.Domain.Entities;
using CRNAssessment.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CRNAssessment.Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _productService = new ProductService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, ProductName = "Test Product" };
            var productDto = new ProductDto { Id = productId, ProductName = "Test Product" };

            _mockRepo.Setup(repo => repo.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _mockMapper.Setup(m => m.Map<ProductDto>(product))
                .Returns(productDto);

            // Act
            var result = await _productService.GetProductById(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.ProductName.Should().Be("Test Product");
        }

        [Fact]
        public async Task GetProductById_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 99;
            _mockRepo.Setup(repo => repo.GetProductByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetProductById(productId));
        }
    }
}
