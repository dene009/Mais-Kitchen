using System.ComponentModel.DataAnnotations;

namespace Mais_Kitchen.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        [Display(Name = "Email Address")]
        public required string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        [Phone]
        public required string Phone { get; set; }

        [StringLength(200)]
        public required string Address { get; set; }

        [StringLength(50)]
        public required string City { get; set; }

        [StringLength(50)]
        public required string State { get; set; }

        [StringLength(10)]
        [Display(Name = "ZIP Code")]
        public string? ZipCode { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public required string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public required string Token { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Compare("Password"), DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
