using AutoMapper;
using CRNAssessment.Application.DTOs;
using CRNAssessment.Application.Interfaces;
using CRNAssessment.Domain.Entities;
using CRNAssessment.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNAssessment.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetAllProducts(int pageNumber, int pageSize)
        {
            var (Items, totalCount) =await _repository.GetAllProducts(pageNumber, pageSize);

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(Items);

            return new PagedResult<ProductDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} was not found.");
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProduct(CreateProductDto productDto,string createdBy)
        {
            var product=_mapper.Map<Product>(productDto);
            product.CreatedBy = createdBy;

            var created = await _repository.AddProduct(product);

            return _mapper.Map<ProductDto>(created);
        }

        public async Task UpdateProduct(UpdateProductDto productDto,string modifiedBy)
        {
            var product = await _repository.GetProductByIdAsync(productDto.Id);
            if (product == null)
                throw new NotFoundException($"Product with ID {productDto.Id} was not found.");

            product.ProductName = productDto.ProductName;
            product.ModifiedBy = modifiedBy;
            product.ModifiedOn = DateTime.Now;

            foreach (var item in productDto.Items)
            {
                var existingItem = product.Items.FirstOrDefault(i => i.Id == item.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity = item.Quantity;
                }
            }
            await _repository.UpdateProduct(product);
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} was not found.");
            await _repository.DeleteProduct(product);
        }

    }
}
