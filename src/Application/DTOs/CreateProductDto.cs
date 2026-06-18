using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNAssessment.Application.DTOs
{
    public class CreateProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public List<CreateItemDto> Items { get; set; } = new List<CreateItemDto>();
    }

    public class CreateItemDto
    {
        public int Quantity { get; set; }
    }
}
