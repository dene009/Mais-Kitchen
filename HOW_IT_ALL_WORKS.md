# How Your Feane Theme Works with MVC

## 🎯 The Complete Flow

```
User Browser
     ↓
  [REQUEST]  http://localhost:5001/Home/Index
     ↓
═══════════════════════════════════════════
    ASP.NET CORE ROUTING
═══════════════════════════════════════════
     ↓
┌────────────────────────────────┐
│  HomeController.cs             │
│  (Controllers folder)          │
│                                │
│  public async Task             │
│  <IActionResult> Index()       │
│  {                             │
│    // 1. Get data from DB      │
│    var categories =            │
│      await _context            │
│        .Categories             │
│        .ToListAsync();         │
│                                │
│    var restaurants = ...       │
│    var featuredItems = ...     │
│                                │
│    // 2. Create ViewModel      │
│    var model = new             │
│      HomeViewModel {           │
│        Categories = categories,│
│        Restaurants = ...,      │
│        FeaturedItems = ...     │
│      };                        │
│                                │
│    // 3. Return View           │
│    return View(model);         │
│  }                             │
└────────────────────────────────┘
     ↓
     ↓ (Passes 'model' data)
     ↓
┌────────────────────────────────┐
│  Views/Home/Index.cshtml       │
│  (View file)                   │
│                                │
│  @model HomeViewModel          │
│                                │
│  <!-- Hero Section -->         │
│  <section class="slider_      │
│           section">            │
│    <h1>Welcome</h1>            │
│  </section>                    │
│                                │
│  <!-- Loop through data -->    │
│  @foreach (var item in         │
│            Model.FeaturedItems)│
│  {                             │
│    <div class="box">           │
│      <h5>@item.FoodName</h5>   │
│      <h6>$@item.Price</h6>     │
│    </div>                      │
│  }                             │
└────────────────────────────────┘
     ↓
     ↓ (Wrapped by)
     ↓
┌────────────────────────────────┐
│  Views/Shared/_Layout.cshtml   │
│  (Master Layout)               │
│                                │
│  <!DOCTYPE html>               │
│  <html>                        │
│  <head>                        │
│    <link href="~/css/          │
│           style.css"/>         │
│  </head>                       │
│  <body>                        │
│    <!-- HEADER -->             │
│    <header>                    │
│      <nav>...</nav>            │
│    </header>                   │
│                                │
│    <!-- CONTENT -->            │
│    @RenderBody()               │
│    ↑                           │
│    └─ Index.cshtml renders here│
│                                │
│    <!-- FOOTER -->             │
│    <footer>...</footer>        │
│                                │
│    <script src="~/js/          │
│            custom.js">         │
│  </body>                       │
│  </html>                       │
└────────────────────────────────┘
     ↓
     ↓ (Loads CSS/JS from)
     ↓
┌────────────────────────────────┐
│  wwwroot/                      │
│  (Static files)                │
│                                │
│  css/                          │
│    ├─ bootstrap.css            │
│    ├─ style.css (Feane)        │
│    ├─ responsive.css           │
│    └─ site.css (Custom)        │
│                                │
│  js/                           │
│    ├─ jquery-3.4.1.min.js      │
│    ├─ bootstrap.js             │
│    └─ custom.js (Feane)        │
│                                │
│  images/                       │
│    ├─ hero-bg.jpg              │
│    ├─ f1.png - f9.png          │
│    └─ o1.jpg, o2.jpg           │
└────────────────────────────────┘
     ↓
  [RESPONSE]  Fully rendered HTML page
     ↓
  User Browser (sees beautiful Feane design!)
```

---

## 🗂️ Data Flow: Adding Item to Cart

```
User clicks "Add to Cart" button
     ↓
┌────────────────────────────────┐
│  Index.cshtml (JavaScript)     │
│                                │
│  <a onclick="                  │
│     addToCart(@item.ID)">      │
│                                │
│  function addToCart(id) {      │
│    fetch('/Cart/AddToCart',   │
│      {                         │
│        method: 'POST',         │
│        body: JSON.stringify({  │
│          foodItemId: id,       │
│          quantity: 1           │
│        })                      │
│      })                        │
│  }                             │
└────────────────────────────────┘
     ↓ (AJAX POST Request)
     ↓
┌────────────────────────────────┐
│  CartController.cs             │
│                                │
│  [HttpPost]                    │
│  public async Task             │
│  <IActionResult>               │
│  AddToCart(int foodItemId,     │
│            int quantity)       │
│  {                             │
│    // Get user ID              │
│    var userId = GetUserId();   │
│                                │
│    // Get food item            │
│    var food = await            │
│      _context.FoodItems        │
│        .FindAsync(foodItemId); │
│                                │
│    // Add to cart              │
│    var cartItem = new          │
│      CartItem {                │
│        UserID = userId,        │
│        FoodItemID = foodItemId,│
│        Quantity = quantity     │
│      };                        │
│                                │
│    _context.CartItems          │
│      .Add(cartItem);           │
│    await _context              │
│      .SaveChangesAsync();      │
│                                │
│    return Json(new {           │
│      success = true            │
│    });                         │
│  }                             │
└────────────────────────────────┘
     ↓
     ↓ (Saves to)
     ↓
┌────────────────────────────────┐
│  DATABASE                      │
│  (SQL Server / SQLite)         │
│                                │
│  CartItems Table               │
│  ┌──────┬────────┬─────────┐  │
│  │ User │ Food   │ Quantity│  │
│  │ ID   │ Item ID│         │  │
│  ├──────┼────────┼─────────┤  │
│  │  1   │   25   │    2    │  │
│  │  1   │   30   │    1    │  │
│  └──────┴────────┴─────────┘  │
└────────────────────────────────┘
     ↓ (Returns JSON)
     ↓
  JavaScript receives response
     ↓
  Shows alert: "Item added to cart!"
```

---

## 🏗️ File Relationships

### When you edit **HomeController.cs**:
```csharp
public IActionResult Menu() {
    return View();  // Looks for Views/Home/Menu.cshtml
}
```

### When you edit **Views/Home/Index.cshtml**:
```razor
@model HomeViewModel    // Uses ViewModels/HomeViewModel.cs
<img src="~/images/f1.png">  // Loads from wwwroot/images/f1.png
<a href="@Url.Action("Menu", "Home")">  // Links to HomeController.Menu()
```

### When you edit **Models/FoodItem.cs**:
```csharp
public class FoodItem {
    public int FoodItemID { get; set; }
    public string FoodName { get; set; }
    public decimal Price { get; set; }
}
```
↓
Used by **ApplicationDbContext.cs**:
```csharp
public DbSet<FoodItem> FoodItems { get; set; }
```
↓
Accessed in **Controllers**:
```csharp
var items = await _context.FoodItems.ToListAsync();
```
↓
Displayed in **Views**:
```razor
@foreach (var item in Model.FeaturedItems) {
    <h5>@item.FoodName</h5>
    <h6>$@item.Price</h6>
}
```

---

## 🎨 How Styling Works

### 1. Feane Base Styles (`wwwroot/css/style.css`)
```css
.food_section .box {
    background: #ffffff;
    padding: 25px 15px;
    border-radius: 5px;
}
```

### 2. Your Custom Overrides (`wwwroot/css/site.css`)
```css
.food_section .box:hover {
    transform: translateY(-5px);
    box-shadow: 0 5px 20px rgba(0,0,0,0.2);
}
```

### 3. Applied in Views
```razor
<div class="food_section">
    <div class="box">
        <!-- Feane + Custom styles applied here -->
    </div>
</div>
```

---

## 🔗 URL to File Mapping

| User Visits | Controller Action | View File |
|-------------|-------------------|-----------|
| `/` | `HomeController.Index()` | `Views/Home/Index.cshtml` |
| `/Home/Menu` | `HomeController.Menu()` | `Views/Home/Menu.cshtml` |
| `/Home/Restaurants` | `HomeController.Restaurants()` | `Views/Home/Restaurants.cshtml` |
| `/Home/RestaurantDetails/5` | `HomeController.RestaurantDetails(5)` | `Views/Home/RestaurantDetails.cshtml` |
| `/Cart` | `CartController.Index()` | `Views/Cart/Cart.cshtml` |
| `/Account/Login` | `AccountController.Login()` | `Views/Account/Login.cshtml` |
| `/Admin/Dashboard` | `AdminController.Dashboard()` | `Views/Admin/Dashboard.cshtml` |

---

## 🔄 The MVC Pattern Explained

```
┌─────────────────────────────────────────────────┐
│                   MVC PATTERN                   │
├─────────────────────────────────────────────────┤
│                                                 │
│  MODEL (Data & Business Logic)                  │
│  ├─ FoodItem.cs                                 │
│  ├─ Restaurant.cs                               │
│  └─ Category.cs                                 │
│  └─ ApplicationDbContext.cs                     │
│         ↑                                       │
│         │ (Reads/Writes)                        │
│         ↓                                       │
│     DATABASE                                    │
│                                                 │
│         ↑                                       │
│         │ (Fetches data)                        │
│         ↓                                       │
│  CONTROLLER (Logic & Routing)                   │
│  ├─ HomeController.cs                           │
│  ├─ CartController.cs                           │
│  └─ OrderController.cs                          │
│         ↑                                       │
│         │ (User requests)                       │
│         │                                       │
│         ↓ (Passes data via ViewModel)           │
│  VIEW (User Interface - Feane Theme)            │
│  ├─ _Layout.cshtml (Header/Footer)              │
│  ├─ Index.cshtml (Homepage)                     │
│  └─ Menu.cshtml (Menu page)                     │
│         ↓                                       │
│         │ (Uses static files)                   │
│         ↓                                       │
│  WWWROOT (CSS, JS, Images)                      │
│  ├─ css/style.css (Feane styles)                │
│  ├─ js/custom.js (Feane scripts)                │
│  └─ images/ (Feane images)                      │
│                                                 │
│         ↓                                       │
│    USER SEES BEAUTIFUL PAGE!                    │
└─────────────────────────────────────────────────┘
```

---

## 💡 Key Concepts

### **Razor Syntax**
```razor
@* Comment *@
@Model.PropertyName                ← Display value
@if (condition) { ... }            ← Conditional
@foreach (var item in list) { }    ← Loop
@Url.Action("Action", "Controller") ← Generate URL
<img src="~/images/pic.jpg">       ← Resolve path
```

### **Controller Actions**
```csharp
public IActionResult Index() {
    return View();                     // Returns view with no data
}

public IActionResult Menu() {
    return View(model);                // Returns view with data
}

[HttpPost]
public IActionResult Create() {
    // Handle form submission
}
```

### **Database Access**
```csharp
// Read
var items = await _context.FoodItems.ToListAsync();

// Filter
var item = await _context.FoodItems
    .Where(f => f.IsAvailable)
    .FirstOrDefaultAsync();

// Include related data
var restaurant = await _context.Restaurants
    .Include(r => r.FoodItems)
    .ThenInclude(f => f.Category)
    .FirstOrDefaultAsync();

// Create
_context.FoodItems.Add(newItem);
await _context.SaveChangesAsync();
```

---

## 🎯 Summary

**Your application uses:**

1. **MVC Pattern** - Separates concerns (data, logic, UI)
2. **Entity Framework** - Talks to database
3. **Razor Pages** - Mixes C# with HTML
4. **Feane Theme** - Provides beautiful UI
5. **Bootstrap** - Responsive layout
6. **jQuery** - JavaScript functionality

**Everything connects through:**
- **Controllers** receive requests and prepare data
- **Models** represent database structure
- **Views** display data using Feane theme
- **Static files** (CSS/JS/Images) make it look great

**The flow is always:**
```
Request → Routing → Controller → Model (DB) → ViewModel → View → Response
```

🎉 **That's how your Feane theme works with your MVC Controllers, Models, and Views!**
