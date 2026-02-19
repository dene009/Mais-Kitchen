# Quick Reference: Making Changes to Your Site

## 🏠 Homepage Changes

### Change Hero Slider Text
**File:** `Views/Home/Index.cshtml`
**Look for:** `<section class="slider_section">`
```razor
<h1>Welcome to Mai's Kitchen</h1>  <!-- Change this -->
<p>Your description here</p>        <!-- And this -->
```

### Add/Remove Featured Items
**File:** `Controllers/HomeController.cs`
**Look for:** `Index()` method
```csharp
var featuredItems = await _context.FoodItems
    .Where(f => f.IsAvailable)
    .Take(12)  // Change number of items shown
    .ToListAsync();
```

---

## 🍕 Menu Page

### File Location
- Controller: `Controllers/HomeController.cs` → `Menu()` action
- View: `Views/Home/Menu.cshtml`

### Filter by Category
```razor
<a href="@Url.Action("Menu", "Home", new { categoryId = 5 })">Pizza</a>
```

### Filter by Restaurant
```razor
<a href="@Url.Action("Menu", "Home", new { restaurantId = 3 })">Specific Restaurant</a>
```

---

## 🎨 Styling Changes

### Colors
**File:** `wwwroot/css/site.css` or `wwwroot/css/style.css`

**Primary Color (Orange):** `#ffbe33`
**Dark Background:** `#222831`
**Text Color:** `#ffffff`

### Change Button Colors
```css
.btn1 {
    background-color: #YOUR_COLOR;
}
.order_online {
    background-color: #YOUR_COLOR;
}
```

---

## 📦 Adding New Features

### Add New Menu Item (via code)
1. **Add to Database:**
```csharp
var newItem = new FoodItem {
    FoodName = "Burger",
    Description = "Delicious beef burger",
    Price = 9.99m,
    CategoryID = 2,
    RestaurantID = 1,
    ImageUrl = "/images/f1.png",
    IsAvailable = true,
    IsVeg = false
};
_context.FoodItems.Add(newItem);
await _context.SaveChangesAsync();
```

2. **It will automatically appear** in Menu and Homepage (if featured)

### Add New Restaurant
```csharp
var restaurant = new Restaurant {
    RestaurantName = "New Restaurant",
    Description = "Great food",
    ImageUrl = "/images/o1.jpg",
    Rating = 4.5m,
    DeliveryTime = 30,
    IsActive = true
};
_context.Restaurants.Add(restaurant);
await _context.SaveChangesAsync();
```

---

## 🔧 Navigation Menu

### Add New Menu Item
**File:** `Views/Shared/_Layout.cshtml`
**Look for:** `<ul class="navbar-nav mx-auto">`

```razor
<li class="nav-item @(currentAction == "YourAction" ? "active" : "")">
    <a class="nav-link" href="@Url.Action("YourAction", "Home")">Your Page</a>
</li>
```

### Then create the controller action:
```csharp
public IActionResult YourAction() {
    return View();
}
```

### And create the view:
**File:** `Views/Home/YourAction.cshtml`

---

## 🛒 Cart Functionality

### Add to Cart Button
```razor
<button onclick="addToCart(@item.FoodItemID)">Add to Cart</button>
```

### The JavaScript (already in Index.cshtml):
```javascript
function addToCart(foodItemId) {
    fetch('@Url.Action("AddToCart", "Cart")', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ foodItemId: foodItemId, quantity: 1 })
    })
    .then(response => response.json())
    .then(data => {
        alert('Item added to cart!');
    });
}
```

---

## 📝 Common Tasks

### Change Site Name
**File:** `Views/Shared/_Layout.cshtml`
```razor
<a class="navbar-brand" href="@Url.Action("Index", "Home")">
    <span>MaisKitchen</span>  <!-- Change here -->
</a>
```

### Update Footer Contact
**File:** `Views/Shared/_Layout.cshtml`
```razor
<a href=""><i class="fa fa-phone"></i> +01 1234567890</a>     <!-- Phone -->
<a href=""><i class="fa fa-envelope"></i> info@email.com</a>  <!-- Email -->
```

### Change Hero Background Image
**Replace file:** `wwwroot/images/hero-bg.jpg`
Or update in Index.cshtml:
```razor
<div class="bg-box">
    <img src="~/images/your-image.jpg" alt="">
</div>
```

---

## 🔍 Finding Things

### Where is the Homepage?
- Controller: `Controllers/HomeController.cs` → `Index()` method
- View: `Views/Home/Index.cshtml`

### Where are food items displayed?
- Homepage: `Views/Home/Index.cshtml` → Featured Items section
- Menu Page: `Views/Home/Menu.cshtml`

### Where is the header/navigation?
- File: `Views/Shared/_Layout.cshtml`

### Where are images stored?
- Path: `wwwroot/images/`
- Access in views: `<img src="~/images/filename.jpg">`

---

## ⚡ Quick Fixes

### Image Not Showing
1. Make sure file is in `wwwroot/images/`
2. Use `~` prefix: `~/images/your-image.jpg`
3. Check file name matches exactly (case-sensitive on Linux)

### Menu Not Showing Items
1. Check database has food items: `IsAvailable = true`
2. Check controller fetches them: `_context.FoodItems.Where(f => f.IsAvailable)`
3. Check view loops through them: `@foreach (var item in Model)`

### Cart Not Working
1. User must be logged in
2. Check JavaScript console for errors (F12 in browser)
3. Verify controller action exists: `CartController.AddToCart()`

### Layout Changes Not Showing
1. Hard refresh browser: `Ctrl + F5` (Windows) or `Cmd + Shift + R` (Mac)
2. Clear browser cache
3. Restart application

---

## 🚀 Running the Project

### Build & Run
```bash
dotnet build
dotnet run
```

### Apply Database Changes
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Access the Site
Open browser: `https://localhost:5001` (or port shown in console)

---

## 📚 File Structure Quick Map

```
Mais-Kitchen/
├── Controllers/           ← Handle requests, return views
│   ├── HomeController.cs
│   ├── CartController.cs
│   └── OrderController.cs
├── Models/               ← Database tables (entities)
│   ├── FoodItem.cs
│   ├── Restaurant.cs
│   └── Category.cs
├── Views/                ← HTML + Razor (UI)
│   ├── Shared/
│   │   └── _Layout.cshtml    ← Header + Footer
│   ├── Home/
│   │   ├── Index.cshtml      ← Homepage
│   │   └── Menu.cshtml       ← Menu page
│   └── Cart/
│       └── Cart.cshtml
├── wwwroot/              ← Static files (CSS, JS, images)
│   ├── css/
│   │   ├── style.css         ← Main theme CSS
│   │   └── site.css          ← Your custom CSS
│   ├── js/
│   └── images/
├── Data/                 ← Database context
│   └── ApplicationDbContext.cs
└── Program.cs            ← App startup
```

---

## 💡 Pro Tips

1. **Always test in browser after changes** - Don't assume it works!
2. **Use Developer Tools (F12)** - Check Console for JavaScript errors
3. **Check Build Output** - Look for compilation errors
4. **Database changes need migrations** - Run `dotnet ef migrations add YourMigrationName`
5. **Images must be in wwwroot** - Files outside won't be served
6. **Use `@Url.Action()` for links** - Don't hardcode URLs like `/home/index`

---

## 🆘 Need Help?

**Error:** "Cannot find view"
- Check view name matches controller action
- Check file is in correct folder: `Views/ControllerName/ActionName.cshtml`

**Error:** "Object reference not set"
- Add null checks: `@(item?.FoodName ?? "Unknown")`
- Check database has data

**Error:** CSS not loading
- Check path: `~/css/style.css` (tilde is important!)
- Hard refresh browser: Ctrl + F5

---

**Remember:** 
- Controllers get data → Views display data → Users see it!
- Always restart the app after code changes to Controllers/Models
- CSS/JS/Image changes don't need restart, just browser refresh
