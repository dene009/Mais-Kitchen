using System.Collections.Generic;
using Mais_Kitchen.Models;

namespace Mais_Kitchen.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
        public IEnumerable<FoodItem> FeaturedItems { get; set; } = new List<FoodItem>();
    }
}