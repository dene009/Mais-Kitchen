using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mais_Kitchen.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodItemID { get; set; }

        // ================= BASIC INFO =================

        [Required(ErrorMessage = "Food name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string FoodName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // ================= PRICE =================

        [Required]
        [Range(0.01, 99999.99, ErrorMessage = "Price must be greater than 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // ================= IMAGE =================

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        // ================= RELATIONSHIPS =================

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Restaurant is required.")]
        public int RestaurantID { get; set; }

        // ================= FLAGS =================

        public bool IsVeg { get; set; } = true;

        public bool IsAvailable { get; set; } = true;

        // ================= EXTRA INFO =================

        [Range(1, 180, ErrorMessage = "Preparation time must be between 1 and 180 minutes.")]
        public int PreparationTime { get; set; } = 20;

        [Range(0, 5000, ErrorMessage = "Calories cannot be negative.")]
        public int Calories { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // ================= NAVIGATION =================

        [ForeignKey(nameof(CategoryID))]
        public virtual Category? Category { get; set; }

        [ForeignKey(nameof(RestaurantID))]
        public virtual Restaurant? Restaurant { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
