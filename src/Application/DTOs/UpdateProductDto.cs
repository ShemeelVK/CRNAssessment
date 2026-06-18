using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRNAssessment.Application.DTOs
{
    public class UpdateProductDto
    {
        public int Id {  get; set; }
        public string ProductName { get; set; }
        public List<UpdateItemDto> Items { get; set; }
    }
    public class UpdateItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
