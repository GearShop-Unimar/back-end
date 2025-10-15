using GearShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GearShop.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do relacionamento Order -> OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração do relacionamento Order -> Payments
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração do relacionamento Subscription -> Payments
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Subscription)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração de índices
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.ExternalPaymentId)
                .IsUnique()
                .HasFilter("[ExternalPaymentId] IS NOT NULL");

            modelBuilder.Entity<Payment>()
                .HasIndex(p => new { p.OrderId, p.PaymentType });

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => new { s.UserId, s.ProductId });

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.NextPaymentDate);
        }
    }
}
