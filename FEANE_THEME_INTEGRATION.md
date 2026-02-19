# Feane Theme Integration Guide for Mai's Kitchen

## ✅ Theme Successfully Integrated!

Your Feane theme is now fully integrated with your ASP.NET Core MVC project. Here's how everything works together:

---

## 📁 Project Structure

### **Models** (`Models/` folder)
Your data models represent the database entities:
- `FoodItem.cs` - Food menu items
- `Restaurant.cs` - Restaurant information
- `Category.cs` - Food categories
- `Order.cs`, `OrderItem.cs` - Order management
- `User.cs` - User accounts
- `CartItem.cs` - Shopping cart items
- `Review.cs` - Restaurant/food reviews

### **Controllers** (`Controllers/` folder)
Controllers handle HTTP requests and return views with data:

#### **HomeController.cs**
- `Index()` - Homepage with categories, restaurants, and featured items
- `Menu()` - Browse all menu items with filters
- `Restaurants()` - List all restaurants
- `RestaurantDetails(id)` - View specific restaurant menu
- `Contact()` - Contact page

#### **CartController.cs**
- Manages shopping cart operations
- Add/Remove items
- Update quantities

#### **OrderController.cs**
- `Checkout()` - Process orders
- `Orders()` - View order history
- `OrderSuccess()` - Order confirmation

#### **AccountController.cs**
- `Login()`, `Register()` - Authentication
- `Profile()` - User profile management
- `Logout()` - Sign out

#### **AdminController.cs**
- Admin dashboard
- Manage restaurants, food items, categories, orders

### **Views** (`Views/` folder)
Razor views display data using Feane theme:

#### **Views/Shared/_Layout.cshtml**
The main layout file that wraps all pages with:
- **Header** with navigation (Home, Menu, Restaurants, Contact)
- **User menu** (Login/Cart/Profile based on authentication)
- **Footer** with contact information
- **Active navigation** highlighting current page

#### **Views/Home/Index.cshtml**
Homepage with Feane theme components:
1. **Hero Carousel** - 3 sliding banners
2. **Featured Restaurants** - Top 2 restaurants in offer section
3. **Menu Items** - Food cards with filters
4. **About Section** - Company information

---

## 🎨 Feane Theme Assets

### **CSS Files** (`wwwroot/css/`)
- `bootstrap.css` - Bootstrap framework
- `style.css` - Main Feane theme styles
- `responsive.css` - Mobile responsive styles
- `font-awesome.min.css` - Icon fonts
- `site.css` - **Custom additions** for restaurant/food cards

### **JavaScript Files** (`wwwroot/js/`)
- `jquery-3.4.1.min.js` - jQuery library
- `bootstrap.js` - Bootstrap components
- `custom.js` - Feane custom scripts (carousel, animations)

### **Images** (`wwwroot/images/`)
- `hero-bg.jpg` - Hero section background
- `f1.png - f9.png` - Food placeholder images
- `o1.jpg, o2.jpg` - Offer section images
- `about-img.png` - About section image
- `client1.jpg, client2.jpg` - Testimonial images

---

## 🔧 How Data Flows (MVC Pattern)

### Example: Loading Homepage

1. **User visits** `/` or `/Home/Index`

2. **Controller (`HomeController.cs`)**
```csharp
public async Task<IActionResult> Index()
{
    // Fetch data from database
    var categories = await _context.Categories.Where(c => c.IsActive).Take(6).ToListAsync();
    var restaurants = await _context.Restaurants.Where(r => r.IsActive).Take(8).ToListAsync();
    var featuredItems = await _context.FoodItems.Where(f => f.IsAvailable).Take(12).ToListAsync();
    
    // Create ViewModel
    var model = new HomeViewModel {
        Categories = categories,
        Restaurants = restaurants,
        FeaturedItems = featuredItems
    };
    
    // Pass to View
    return View(model);
}
```

3. **View (`Views/Home/Index.cshtml`)**
```razor
@model Mais_Kitchen.ViewModels.HomeViewModel

<!-- Access data -->
@foreach (var item in Model.FeaturedItems) {
    <div class="box">
        <h5>@item.FoodName</h5>
        <h6>$@item.Price</h6>
    </div>
}
```

4. **Layout (`Views/Shared/_Layout.cshtml`)**
Wraps the view with header, navigation, and footer.

---

## 🎯 Key Features Implemented

### 1. **Dynamic Navigation**
- Active page highlighting
- Authentication-aware menu (shows Login/Cart/Profile based on login status)

### 2. **Hero Carousel**
- 3 rotating slides with call-to-action buttons
- Background image overlay
- Feane's signature styling

### 3. **Restaurant/Food Cards**
- Feane's box design
- Hover effects
- Dynamic data from database
- SVG cart icons

### 4. **Add to Cart Functionality**
- JavaScript function `addToCart(foodItemId)`
- AJAX POST to `/Cart/AddToCart`
- Success/error notifications

### 5. **Responsive Design**
- Mobile-friendly navigation
- Bootstrap grid system
- Custom breakpoints in `site.css`

---

## 🚀 How to Customize

### **Change Brand Name**
In `_Layout.cshtml`:
```razor
<a class="navbar-brand" href="@Url.Action("Index", "Home")">
    <span>YOUR_NAME_HERE</span>
</a>
```

### **Update Footer Contact**
In `_Layout.cshtml` footer section:
```razor
<a href=""><i class="fa fa-phone"></i> YOUR_PHONE</a>
<a href=""><i class="fa fa-envelope"></i> YOUR_EMAIL</a>
```

### **Add More Menu Items**
1. Add data to database via Admin panel
2. Controller automatically fetches from `_context.FoodItems`
3. View displays them in Feane cards

### **Change Hero Slides**
In `Views/Home/Index.cshtml`, edit carousel items:
```razor
<div class="carousel-item active">
    <div class="detail-box">
        <h1>YOUR TITLE</h1>
        <p>YOUR DESCRIPTION</p>
    </div>
</div>
```

### **Custom Styles**
Add to `wwwroot/css/site.css`:
```css
/* Your custom styles */
.my-custom-class {
    color: red;
}
```

---

## 📱 Pages Available

| URL | Controller Action | Purpose |
|-----|------------------|---------|
| `/` | `Home/Index` | Homepage |
| `/Home/Menu` | `Home/Menu` | Browse all food items |
| `/Home/Restaurants` | `Home/Restaurants` | List restaurants |
| `/Home/RestaurantDetails/5` | `Home/RestaurantDetails` | View restaurant menu |
| `/Cart` | `Cart/Index` | View shopping cart |
| `/Order/Checkout` | `Order/Checkout` | Place order |
| `/Account/Login` | `Account/Login` | Sign in |
| `/Account/Register` | `Account/Register` | Create account |
| `/Admin` | `Admin/Dashboard` | Admin panel |

---

## 🔐 Authentication Flow

### **For Guests (Not Logged In)**
- Can browse restaurants and menu
- "Order Online" button → redirects to login
- No cart access

### **For Logged In Users**
- Cart icon visible in header
- Can add items to cart
- Can place orders
- Profile menu access

---

## 🛠️ Next Steps to Enhance

1. **Add Owl Carousel** (currently using Bootstrap carousel)
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/OwlCarousel2/2.3.4/assets/owl.carousel.min.css" />
```

2. **Add Nice Select** for dropdowns
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/jquery-nice-select/1.1.0/css/nice-select.min.css" />
```

3. **Implement Food Filters** (currently shows all items)
4. **Add Search Functionality** in header
5. **Client Testimonials Section** on homepage
6. **Book Table Feature** from Feane theme

---

## 📝 Database Seeding

To populate with sample data:
1. Check `Data/DataSeeder.cs`
2. Add sample restaurants, categories, food items
3. Run migrations: `dotnet ef database update`

---

## ✨ Summary

**Your project successfully combines:**
- ✅ **ASP.NET Core MVC** architecture (Models, Views, Controllers)
- ✅ **Feane HTML Template** (modern, responsive design)
- ✅ **Entity Framework Core** (database operations)
- ✅ **Dynamic Data Binding** (Razor syntax)
- ✅ **Authentication & Authorization**
- ✅ **Shopping Cart & Orders**
- ✅ **Admin Dashboard**

Everything is working together! Your controllers fetch data from the database, pass it to views, and the Feane theme displays it beautifully. 🎉

---

**Need Help?**
- Controllers define actions (what happens when you visit a URL)
- Views display the UI using Razor syntax and Feane HTML/CSS
- Models represent your database tables
- The flow is always: **Request → Controller → Model (Database) → ViewModel → View → Response**
