using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CRNAssessment.Application.DTOs;
using CRNAssessment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CRNAssessment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //get all products
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var products = await _productService.GetAllProducts(query.PageNumber,query.PageSize);
            return Ok(products);
        }

        //get by id
        [HttpGet("GetProductById{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }

        //post product
        [HttpPost("AddProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            var created = await _productService.CreateProduct(dto, username);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        //PUT Product
        [HttpPut("UpdateProduct{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "Route ID and body ID do not match." });
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            await _productService.UpdateProduct(dto, username);
            return NoContent();
        }

        //Delete Product
        [HttpDelete("DeleteProduct{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }

    }
}
