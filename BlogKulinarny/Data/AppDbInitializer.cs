using BlogKulinarny.Data.Enums;
using BlogKulinarny.Models;

namespace BlogKulinarny.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder ApplicationBuilder)
        {
            using (var serviceScope = ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                context.Database.EnsureCreated();

                //User
                if (!context.users.Any())
                {
                    context.users.AddRange(new List<User>()
                    {
                        new User()
                        {
                            login = "admin",
                            password = "admin",
                            mail = "admin@admin.pl",
                            isAccepted = true,
                            rank = Ranks.admin,
                            VerificationToken = "1"
                        },
                    });
                    context.SaveChanges();
                }

                //recipes
                if (!context.recipes.Any())
                {
                    context.recipes.AddRange(new List<Recipe>()
                    {
                        new Recipe()
                        {
                           isAccepted = false,
                           title = "kebab z kapuśniakiem",
                           description = "to jest bombajski przysmak czyli kebap z kapuśniakiem",
                           imageURL = "data:image/jpeg;base64,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",
                           difficulty = 4,
                           avgTime = 5,
                           portions = 2,
                           userId = 9
                        }
                    });
                    context.SaveChanges();
                }

                // Recipe elements
                if (!context.recipesElements.Any())
                {
                    context.recipesElements.AddRange(new List<RecipeElements>()
                    {
                        new RecipeElements()
                        {
                            noOfList = 0,
                            imageURL = "data:image/jpeg;base64,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",
                            description = "to jest lista z skladnikami: kebab i kapusniak",
                            recipeId = 12,
                        },

                        new RecipeElements()
                        {
                            noOfList = 1,
                            imageURL = "https://staticsmaker.iplsc.com/smaker_production_2022_11_16/26bb30c30d60775239c5bc09c9735973-recipe_main.jpg",
                            description = "to pierwszy element z robienia przepisu",
                            recipeId = 12,
                        },

                    });
                    context.SaveChanges();
                }

                //categories
                if (!context.categories.Any())
                {
                    context.categories.AddRange(new List<Category>()
                    {
                        new Category()
                        {
                            name = "kuchnia bombajska"
                        },

                        new Category()
                        {
                            name = "kebab"
                        }
                    });
                    context.SaveChanges();
                }
                
                // Recipe @ Category relations
                if (!context.recipesCategories.Any())
                {
                    context.recipesCategories.AddRange(new List<RecipesCategory>()
                    {
                        new RecipesCategory()
                        {
                            recipeId = 12,
                            categoryId = 1
                        },

                        new RecipesCategory()
                        {
                            recipeId = 12,
                            categoryId = 2
                        },

                    });

                    context.SaveChanges();
                }
            }
        }
    }
}
