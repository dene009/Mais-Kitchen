using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mais_Kitchen.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        // ==============================
        // REQUIRED CORE FIELDS
        // ==============================

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        // ==============================
        // OPTIONAL PROFILE FIELDS
        // ==============================

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? ZipCode { get; set; }

        // ==============================
        // ACCOUNT SETTINGS
        // ==============================

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Customer";

        // ==============================
        // PASSWORD RESET SUPPORT
        // ==============================

        [StringLength(200)]
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetExpiry { get; set; }

        // ==============================
        // COMPUTED PROPERTY
        // ==============================

        [NotMapped]
        public string FullName =>
            string.Join(" ",
                new[] { FirstName, LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
        public virtual List<Order> Orders { get; set; } = new();
    }
}
