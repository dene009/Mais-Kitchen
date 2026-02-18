using Mais_Kitchen.Data;
using Mais_Kitchen.Models;
using Mais_Kitchen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace Mais_Kitchen.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ================= DASHBOARD =================
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.Users = await _context.Users.CountAsync(u => u.Role == "Customer");
            ViewBag.Restaurants = await _context.Restaurants.CountAsync();
            ViewBag.Orders = await _context.Orders.CountAsync();

            ViewBag.Revenue = await _context.Orders
                .Select(o => (decimal?)o.FinalAmount)
                .SumAsync() ?? 0;

            return View();
        }

        // ================= VIEW ORDERS =================
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // ================= UPDATE ORDER STATUS =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            if (orderId <= 0 || string.IsNullOrWhiteSpace(newStatus))
                return Json(new { success = false, message = "Invalid request." });

            // Whitelisted statuses (Prevents tampering)
            string[] allowedStatuses =
            {
                "Pending",
                "Preparing",
                "Out for Delivery",
                "Delivered",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(newStatus))
                return Json(new { success = false, message = "Invalid status value." });

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
                return Json(new { success = false, message = "Order not found." });

            order.OrderStatus = newStatus;

            // Set delivery time only once
            if (newStatus == "Delivered" && order.DeliveryTime == null)
                order.DeliveryTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Order #{orderId} updated to {newStatus}."
            });
        }

        // ================= MANAGE RESTAURANTS =================
        public async Task<IActionResult> Restaurants()
        {
            var data = await _context.Restaurants
                .OrderBy(r => r.RestaurantName)
                .ToListAsync();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return NotFound();

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return RedirectToAction("Restaurants");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRestaurant(Restaurant model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Handle image upload safely
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.ImageUrl = "/images/" + fileName;
            }

            model.CreatedDate = DateTime.Now;

            _context.Restaurants.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Restaurants");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRestaurant(Restaurant model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var restaurant = await _context.Restaurants.FindAsync(model.RestaurantID);
            if (restaurant == null)
                return NotFound();

            restaurant.RestaurantName = model.RestaurantName;
            restaurant.Description = model.Description;
            restaurant.Email = model.Email;
            restaurant.Phone = model.Phone;
            restaurant.Address = model.Address;
            restaurant.DeliveryTime = model.DeliveryTime;
            restaurant.DeliveryFee = model.DeliveryFee;
            restaurant.IsActive = model.IsActive;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                restaurant.ImageUrl = "/images/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Restaurants");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFoodItem(int id)
        {
            var food = await _context.FoodItems.FindAsync(id);
            if (food == null)
                return NotFound();

            _context.FoodItems.Remove(food);
            await _context.SaveChangesAsync();

            return RedirectToAction("FoodItems");
        }

        // ================= CATEGORY MANAGEMENT =================

        public async Task<IActionResult> Categories()
        {
            var list = await _context.Categories
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(Category model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Invalid image format.");
                    return View(model);
                }

                string folderPath = Path.Combine(_env.WebRootPath, "images/categories");
                Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.ImageUrl = "/images/categories/" + fileName;
            }

            model.CreatedDate = DateTime.Now;

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Categories");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = await _context.Categories.FindAsync(model.CategoryID);
            if (category == null)
                return NotFound();

            category.CategoryName = model.CategoryName;
            category.Description = model.Description;
            category.IsActive = model.IsActive;

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Invalid image format.");
                    return View(model);
                }

                string folderPath = Path.Combine(_env.WebRootPath, "images/categories");
                Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                category.ImageUrl = "/images/categories/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Categories");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Categories");
        }

        [HttpGet]
        public async Task<IActionResult> AddFoodItem()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            ViewBag.Restaurants = await _context.Restaurants
                .Where(r => r.IsActive)
                .OrderBy(r => r.RestaurantName)
                .ToListAsync();

            return View(new FoodItem());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFoodItem(FoodItem model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            // Image Upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Invalid image format.");
                    await LoadDropdowns();
                    return View(model);
                }

                if (imageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Image size must be under 2MB.");
                    await LoadDropdowns();
                    return View(model);
                }

                string folderPath = Path.Combine(_env.WebRootPath, "images/food");
                Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.ImageUrl = "/images/food/" + fileName;
            }

            model.CreatedDate = DateTime.Now;

            _context.FoodItems.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("FoodItems");
        }

        [HttpGet]
        public async Task<IActionResult> EditFoodItem(int id)
        {
            var food = await _context.FoodItems.FindAsync(id);
            if (food == null)
                return NotFound();

            await LoadDropdowns();
            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFoodItem(FoodItem model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            var food = await _context.FoodItems.FindAsync(model.FoodItemID);
            if (food == null)
                return NotFound();

            food.FoodName = model.FoodName;
            food.Description = model.Description;
            food.Price = model.Price;
            food.CategoryID = model.CategoryID;
            food.RestaurantID = model.RestaurantID;
            food.IsVeg = model.IsVeg;
            food.IsAvailable = model.IsAvailable;
            food.Calories = model.Calories;
            food.PreparationTime = model.PreparationTime;

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Invalid image format.");
                    await LoadDropdowns();
                    return View(model);
                }

                if (imageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Image size must be under 2MB.");
                    await LoadDropdowns();
                    return View(model);
                }

                string folderPath = Path.Combine(_env.WebRootPath, "images/food");
                Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Optional: delete old image
                if (!string.IsNullOrEmpty(food.ImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, food.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                food.ImageUrl = "/images/food/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("FoodItems");
        }

        private async Task LoadDropdowns()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            ViewBag.Restaurants = await _context.Restaurants
                .Where(r => r.IsActive)
                .OrderBy(r => r.RestaurantName)
                .ToListAsync();
        }

    }
}
