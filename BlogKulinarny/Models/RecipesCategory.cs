namespace BlogKulinarny.Models; 

public class RecipesCategory
{
    public int recipeId { get; set; }
    public Recipe recipe { get; set; }
    public int categoryId { get; set; }
    public Category category { get; set; }
}