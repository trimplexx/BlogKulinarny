using BlogKulinarny.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogKulinarny.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipesCategory>().HasKey(am => new {

                am.recipeId,
                am.categoryId
            });

            modelBuilder.Entity<RecipesCategory>().HasOne(m => m.recipe).WithMany(am => am.recipesCategories).HasForeignKey(m => m.recipeId);

            modelBuilder.Entity<RecipesCategory>().HasOne(m => m.category).WithMany(am => am.recipesCategories).HasForeignKey(m => m.categoryId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Category> categories { get; set; }
        public DbSet<Recipe> recipes { get; set; }
        public DbSet<RecipeElements> recipesElements { get; set; }
        public DbSet<RecipesCategory> recipesCategories { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Comment> comments { get; set; }
    }
}
