using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRNAssessment.Domain.Entities;
using CRNAssessment.Application.DTOs;

namespace CRNAssessment.Application.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product,ProductDto>();
            CreateMap<Item,ItemDto>();

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<CreateItemDto, Item>();
        }

    }
}
