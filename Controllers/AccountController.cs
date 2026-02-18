using Mais_Kitchen.Models;
using Mais_Kitchen.Services;
using Mais_Kitchen.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mais_Kitchen.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountService accountService,
            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        // ================= LOGIN =================

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.AuthenticateAsync(model.Email, model.Password);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for {Email}", model.Email);
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            await SignInUser(user, model.RememberMe);

            _logger.LogInformation("User {Email} logged in.", user.Email);

            return RedirectToLocal(returnUrl, user.Role);
        }

        // ================= REGISTER =================

        [AllowAnonymous]
        public IActionResult Register() => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Role = "Customer",
                IsActive = true
            };

            var result = await _accountService.RegisterAsync(user, model.Password);

            if (!result.Success)
            {
                ModelState.AddModelError("Email", result.Error!);
                return View(model);
            }

            _logger.LogInformation("New user registered: {Email}", user.Email);

            TempData["SuccessMessage"] = "Registration successful.";
            return RedirectToAction(nameof(Login));
        }

        // ================= LOGOUT =================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        // ================= HELPERS =================

        private async Task SignInUser(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe
                        ? DateTimeOffset.UtcNow.AddDays(7)
                        : DateTimeOffset.UtcNow.AddHours(2)
                });
        }

        private IActionResult RedirectToLocal(string? returnUrl, string role)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Index", "Home");
        }
    }
}
