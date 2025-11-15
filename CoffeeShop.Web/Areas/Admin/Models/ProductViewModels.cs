using Microsoft.AspNetCore.Http;
using Shared.Dtos.ProductDtos;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Web.Areas.Admin.Models
{
    public class CreateProductViewModel : CreateUpdateProductDto
    {
        [Display(Name = "Chọn ảnh sản phẩm")]
        public IFormFile ImageFile { get; set; }
    }
}