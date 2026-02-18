using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mais_Kitchen.Models;

public class Order
{
    [Key]
    public int OrderID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int RestaurantID { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal DeliveryFee { get; set; } = 0;

    [Column(TypeName = "decimal(8,2)")]
    public decimal TaxAmount { get; set; } = 0;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal FinalAmount { get; set; }

    [StringLength(50)]
    public string OrderStatus { get; set; } = "Pending";

    [StringLength(50)]
    public string PaymentStatus { get; set; } = "Pending";

    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(500)]
    public string DeliveryAddress { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? DeliveryTime { get; set; }

    [StringLength(500)]
    public string? SpecialInstructions { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual List<OrderItem> OrderItems { get; set; } = new();
    public virtual List<Review> Reviews { get; set; } = new();
}
