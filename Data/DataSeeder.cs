using Mais_Kitchen.Models;

namespace Mais_Kitchen.Data
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Only seed if empty
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Pizza", IsActive = true },
                    new Category { CategoryName = "Burgers", IsActive = true },
                    new Category { CategoryName = "Desserts", IsActive = true }
                );
            }

            if (!context.Restaurants.Any())
            {
                context.Restaurants.AddRange(
                    new Restaurant
                    {
                        RestaurantName = "Italiano Pizza",
                        Description = "Authentic Italian pizzas",
                        Address = "123 Main St",
                        Phone = "555‑1000",
                        Email = "pizza@example.com",
                        ImageUrl = "/images/sample‑restaurant.jpg",
                        Rating = 4.5m,
                        DeliveryTime = 30,
                        DeliveryFee = 2
                    },
                    new Restaurant
                    {
                        RestaurantName = "Burger House",
                        Description = "Juicy gourmet burgers",
                        Address = "456 Elm St",
                        Phone = "555‑2000",
                        Email = "burgers@example.com",
                        ImageUrl = "/images/sample‑restaurant2.jpg",
                        Rating = 4.2m,
                        DeliveryTime = 25,
                        DeliveryFee = 1.5m
                    }
                );
            }

            // persist them before querying
            context.SaveChanges();

            if (!context.FoodItems.Any())
            {
                var firstRestaurant = context.Restaurants.First();
                var firstCategory = context.Categories.First();

                context.FoodItems.AddRange(
                    new FoodItem
                    {
                        FoodName = "Margherita Pizza",
                        Description = "Classic tomato & mozzarella",
                        Price = 10.99m,
                        IsVeg = true,
                        CategoryID = firstCategory.CategoryID,
                        RestaurantID = firstRestaurant.RestaurantID,
                        IsAvailable = true,
                        ImageUrl = "/images/sample‑pizza.jpg"
                    },
                    new FoodItem
                    {
                        FoodName = "Cheeseburger",
                        Description = "Beef patty with cheese and fries",
                        Price = 8.99m,
                        IsVeg = false,
                        CategoryID = firstCategory.CategoryID,
                        RestaurantID = firstRestaurant.RestaurantID,
                        IsAvailable = true,
                        ImageUrl = "/images/sample‑burger.jpg"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}

