using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mais_Kitchen.Models
{
    public class CartItem
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int FoodItemID { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public required virtual User User { get; set; }

        [ForeignKey("FoodItemID")]
        public required virtual FoodItem FoodItem { get; set; }
    }
}
