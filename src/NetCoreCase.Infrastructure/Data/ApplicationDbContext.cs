using Microsoft.EntityFrameworkCore;
using NetCoreCase.Domain.Entities;

namespace NetCoreCase.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Content> Contents { get; set; }
    public DbSet<ContentVariant> ContentVariants { get; set; }
    public DbSet<UserContentVariantHistory> UserContentVariantHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Navigation
            entity.HasMany(e => e.Contents)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict); // İçerikleri olan kullanıcı silinemez
        });

        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
            
            // Navigation
            entity.HasMany(e => e.Contents)
                  .WithOne(e => e.Category)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict); // İçerikleri olan kategori silinemez
        });

        // Content Configuration
        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Language).IsRequired().HasMaxLength(5);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            
            // Indexes
            entity.HasIndex(e => e.Language);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.Title, e.Language }); // Composite index for search
            
            // Navigation
            entity.HasOne(e => e.User)
                  .WithMany(e => e.Contents)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Contents)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Variants)
                  .WithOne(e => e.Content)
                  .HasForeignKey(e => e.ContentId)
                  .OnDelete(DeleteBehavior.Cascade); // Content silindiğinde varyantlar da silinir
        });

        // ContentVariant Configuration
        modelBuilder.Entity<ContentVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VariantData).IsRequired().HasMaxLength(2000);
            
            // Indexes
            entity.HasIndex(e => e.ContentId);
            entity.HasIndex(e => new { e.ContentId, e.IsDefault }); // Default varyant bulmak için
            
            // Navigation
            entity.HasOne(e => e.Content)
                  .WithMany(e => e.Variants)
                  .HasForeignKey(e => e.ContentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserContentVariantHistory Configuration
        modelBuilder.Entity<UserContentVariantHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ViewCount).HasDefaultValue(1);
            
            // Indexes
            entity.HasIndex(e => new { e.UserId, e.ContentId }).IsUnique(); // Bir kullanıcı bir içerik için sadece bir kayıt
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ContentId);
            entity.HasIndex(e => e.VariantId);
            entity.HasIndex(e => e.LastAccessedAt);
            
            // Navigation
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde geçmişi de silinir

            entity.HasOne(e => e.Content)
                  .WithMany()
                  .HasForeignKey(e => e.ContentId)
                  .OnDelete(DeleteBehavior.Cascade); // İçerik silindiğinde geçmişi de silinir

            entity.HasOne(e => e.Variant)
                  .WithMany()
                  .HasForeignKey(e => e.VariantId)
                  .OnDelete(DeleteBehavior.Cascade); // Varyant silindiğinde geçmişi de silinir
        });

        // BaseEntity Configuration - PostgreSQL için NOW() kullan
        modelBuilder.Entity<User>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW()");
            
        modelBuilder.Entity<Category>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW()");
            
        modelBuilder.Entity<Content>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW()");
            
        modelBuilder.Entity<ContentVariant>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW()");

        modelBuilder.Entity<UserContentVariantHistory>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW()");

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories - Sabit GUID ve DateTime değerleri kullan
        var techCategoryId = new Guid("9f01a852-18a9-4b0c-84cc-ac12f82210e0");
        var healthCategoryId = new Guid("24e541c7-ee44-497e-beda-b83ab2648d39");
        var educationCategoryId = new Guid("5c7fc0e2-d376-4b50-aeef-51a948a54ad1");

        modelBuilder.Entity<Category>().HasData(
            new Category 
            { 
                Id = techCategoryId, 
                Name = "Teknoloji", 
                Description = "Teknoloji ile ilgili içerikler",
                CreatedAt = new DateTime(2025, 5, 29, 11, 10, 25, 500, DateTimeKind.Utc).AddTicks(282)
            },
            new Category 
            { 
                Id = healthCategoryId, 
                Name = "Sağlık", 
                Description = "Sağlık ile ilgili içerikler",
                CreatedAt = new DateTime(2025, 5, 29, 11, 10, 25, 500, DateTimeKind.Utc).AddTicks(435)
            },
            new Category 
            { 
                Id = educationCategoryId, 
                Name = "Eğitim", 
                Description = "Eğitim ile ilgili içerikler",
                CreatedAt = new DateTime(2025, 5, 29, 11, 10, 25, 500, DateTimeKind.Utc).AddTicks(436)
            }
        );

        // Seed Users - Sabit GUID ve DateTime değerleri kullan
        var user1Id = new Guid("b82fae75-5bef-4d73-8b15-4f255764f474");
        var user2Id = new Guid("187dfadb-2317-4ff2-9381-60b0278aa845");

        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = user1Id, 
                FullName = "Ahmet Yılmaz", 
                Email = "ahmet@example.com",
                CreatedAt = new DateTime(2025, 5, 29, 11, 10, 25, 500, DateTimeKind.Utc).AddTicks(4943)
            },
            new User 
            { 
                Id = user2Id, 
                FullName = "Ayşe Demir", 
                Email = "ayse@example.com",
                CreatedAt = new DateTime(2025, 5, 29, 11, 10, 25, 500, DateTimeKind.Utc).AddTicks(4947)
            }
        );
    }
} 