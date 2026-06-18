using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRNAssessment.Application.DTOs;

namespace CRNAssessment.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllProducts(int pageNumber, int pageSize);
        Task<ProductDto> GetProductById(int id);
        Task<ProductDto> CreateProduct(CreateProductDto productDto, string createdBy);
        Task UpdateProduct(UpdateProductDto productDto, string modifiedBy);
        Task DeleteProduct(int id);
    }
}
