using Mais_Kitchen.Models;
using Microsoft.EntityFrameworkCore;

namespace Mais_Kitchen.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ===================== DB SETS =====================
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // ===================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔒 Disable ALL cascade deletes globally
            foreach (var relationship in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // ---------- DECIMAL PRECISION SETTINGS ----------
            modelBuilder.Entity<FoodItem>()
                .Property(f => f.Price)
                .HasColumnType("decimal(8,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryFee)
                .HasColumnType("decimal(8,2)");
            modelBuilder.Entity<Order>()
                .Property(o => o.TaxAmount)
                .HasColumnType("decimal(8,2)");
            modelBuilder.Entity<Order>()
                .Property(o => o.FinalAmount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(8,2)");
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.TotalPrice)
                .HasColumnType("decimal(8,2)");

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Rating)
                .HasColumnType("decimal(2,1)");
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.DeliveryFee)
                .HasColumnType("decimal(8,2)");

            // ---------- RELATIONSHIP CONFIGURATION ----------

            // ORDER → ORDERITEMS (Cascade delete)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // FOODITEM → ORDERITEMS (Restrict delete)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.FoodItem)
                .WithMany(fi => fi.OrderItems)
                .HasForeignKey(oi => oi.FoodItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // ORDER → RESTAURANT (Restrict delete)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)     // assumes Restaurant.Orders exists
                .HasForeignKey(o => o.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            // ORDER → USER (Restrict delete)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)     // assumes User.Orders exists
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- BASE CALL (ONLY ONCE) ----------
            base.OnModelCreating(modelBuilder);
        }
    }
}
