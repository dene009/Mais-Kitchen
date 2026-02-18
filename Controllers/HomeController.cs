using Mais_Kitchen.Data;
using Mais_Kitchen.Models;
using Mais_Kitchen.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mais_Kitchen.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Take(6)
                .ToListAsync();

            var restaurants = await _context.Restaurants
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.Rating)
                .Take(8)
                .ToListAsync();

            var featuredItems = await _context.FoodItems
                .Include(f => f.Restaurant)
                .Include(f => f.Category)
                .Where(f => f.IsAvailable)
                .OrderBy(f => Guid.NewGuid())
                .Take(12)
                .ToListAsync();

            // Debug logging
            _logger.LogInformation("Home Index loaded - Categories: {CategoryCount}, Restaurants: {RestaurantCount}, FeaturedItems: {FeaturedItemCount}", 
                categories.Count, restaurants.Count, featuredItems.Count);

            var model = new
            {
                Categories = categories,
                Restaurants = restaurants,
                FeaturedItems = featuredItems
            };

            // 👇 this line is the key
            ViewBag.Model = model;

            // do NOT pass model again, or it overrides ViewBag
            return View();
        }

        public async Task<IActionResult> Restaurants()
        {
            var restaurants = await _context.Restaurants
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.Rating)
                .ToListAsync();
            return View(restaurants);
        }
        public async Task<IActionResult> RestaurantDetails(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.FoodItems.Where(f => f.IsAvailable))
                .ThenInclude(f => f.Category)
                .Include(r => r.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(r => r.RestaurantID == id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }
        public async Task<IActionResult> Menu(int? categoryId, int? restaurantId)
        {
            var query = _context.FoodItems
                .Include(f => f.Category)
                .Include(f => f.Restaurant)
                .Where(f => f.IsAvailable);
            if (categoryId.HasValue)
            {
                query = query.Where(f => f.CategoryID == categoryId.Value);
            }
            if (restaurantId.HasValue)
            {
                query = query.Where(f => f.RestaurantID == restaurantId.Value);
            }
            var foodItems = await query.ToListAsync();
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            var restaurants = await _context.Restaurants.Where(r => r.IsActive).ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.Restaurants = restaurants;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedRestaurantId = restaurantId;
            return View(foodItems);
        }
    }
}
