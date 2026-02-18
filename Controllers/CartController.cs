using Mais_Kitchen.Data;
using Mais_Kitchen.Models;
using Mais_Kitchen.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mais_Kitchen.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }
            var cartItems = await _context.CartItems
                .Include(c => c.FoodItem)
                .ThenInclude(f => f.Restaurant)
                .Where(c => c.UserID == userId)
                .ToListAsync();
            return View(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int foodItemId, int quantity = 1)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserID == userId && c.FoodItemID == foodItemId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var foodItem = await _context.FoodItems
                    .Include(f => f.Restaurant)
                    .FirstOrDefaultAsync(f => f.FoodItemID == foodItemId);
                if (foodItem == null)
                {
                    return NotFound();
                }
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserID == userId);
                if (user == null)
                {
                    return Unauthorized();
                }
                var cartItem = new CartItem
                {
                    UserID = userId,
                    FoodItemID = foodItemId,
                    Quantity = quantity,
                    User = user,
                    FoodItem = foodItem
                };
                _context.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Item added to cart!" });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartID == cartId && c.UserID == userId);
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                }
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartID == cartId && c.UserID == userId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
        public async Task<IActionResult> GetCartCount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }
            var count = await _context.CartItems
                .Where(c => c.UserID == userId)
                .SumAsync(c => c.Quantity);
            return Json(new { count });
        }
    }
}
