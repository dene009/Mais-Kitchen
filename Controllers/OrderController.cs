using Mais_Kitchen.Data;
using Mais_Kitchen.Models;
using Mais_Kitchen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mais_Kitchen.Controllers
{
    [Authorize]
    public class OrderController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // ================= CHECKOUT =================
        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Challenge();
            }

            var cartItems = await _context.CartItems
                .Include(c => c.FoodItem)
                .ThenInclude(f => f.Restaurant)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
                return RedirectToAction("Index", "Cart");

            return View(cartItems);
        }

        // ================= PLACE ORDER =================
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string paymentMethod)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Challenge();
            }

            var user = await _context.Users.FindAsync(userId);

            var cartItems = await _context.CartItems
                .Include(c => c.FoodItem)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
                return RedirectToAction("Index", "Cart");

            decimal total = cartItems.Sum(c => c.FoodItem.Price * c.Quantity);
            decimal deliveryFee = 2.99M;
            decimal tax = total * 0.08M;
            decimal final = total + deliveryFee + tax;

            var restaurantId = cartItems.First().FoodItem.RestaurantID;
            var restaurantEntity = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurantEntity == null)
            {
                // Handle the case where the restaurant is not found
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                UserID = userId,
                RestaurantID = restaurantId,
                TotalAmount = total,
                DeliveryFee = deliveryFee,
                TaxAmount = tax,
                FinalAmount = final,
                OrderStatus = "Pending",
                PaymentStatus = "Completed",
                PaymentMethod = paymentMethod,
                DeliveryAddress = user?.Address ?? string.Empty,
                OrderDate = DateTime.Now,
                User = user!,
                Restaurant = restaurantEntity
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderID = order.OrderID,
                    FoodItemID = item.FoodItemID,
                    Quantity = item.Quantity,
                    Price = item.FoodItem.Price,
                    TotalPrice = item.FoodItem.Price * item.Quantity,
                    Order = order,
                    FoodItem = item.FoodItem
                });
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess");
        }

        // ================= ORDER SUCCESS =================
        public IActionResult OrderSuccess()
        {
            return View();
        }

        // ================= USER ORDERS HISTORY =================
        public async Task<IActionResult> Orders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Challenge();
            }

            var orders = await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.FoodItem)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}
