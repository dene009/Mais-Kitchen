using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mais_Kitchen.Models
{
    public class Restaurant
    {
        [Key]
        public int RestaurantID { get; set; }

        [Required]
        [StringLength(100)]
        public string RestaurantName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, 5)]
        public decimal Rating { get; set; } = 0;

        [Range(0, 300)]
        public int DeliveryTime { get; set; } = 30;

        [Range(0, 9999.99)]
        [DataType(DataType.Currency)]
        public decimal DeliveryFee { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // ===================== RELATIONSHIPS =====================

        // 1️⃣  One‑to‑Many: Restaurant → FoodItems
        public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();

        // 2️⃣  One‑to‑Many: Restaurant → Orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        // 3️⃣  One‑to‑Many: Restaurant → Reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
