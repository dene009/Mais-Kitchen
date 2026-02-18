using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mais_Kitchen.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int? UserID { get; set; }

        [Required]
        public int RestaurantID { get; set; }

        public int? OrderID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        [ForeignKey("RestaurantID")]
        public virtual Restaurant? Restaurant { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
    }
}
