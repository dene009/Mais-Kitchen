using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mais_Kitchen.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int FoodItemID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal TotalPrice { get; set; }

        // ✅ No [ForeignKey] attributes needed
        public required virtual Order Order { get; set; }
        public required virtual FoodItem FoodItem { get; set; }
    }
}
