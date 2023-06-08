using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models;

public class Comment
{
    public int Id { get; set; }

    public string Text { get; set; }

    public int Rate { get; set; }

    [Display(Name = "RecipeId")] public int recipeId { get; set; }

    public Recipe recipe { get; set; }
}