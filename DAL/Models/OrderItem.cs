using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [MaxLength(50)]
        public string Size { get; set; } 

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; } 

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; } 

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}