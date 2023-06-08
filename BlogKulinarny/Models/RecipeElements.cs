using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models;

public class RecipeElements
{
    [Key] public int id { get; set; }

    [Display(Name = "NoOfList")] public int noOfList { get; set; }

    [Display(Name = "ImageUrl")] public string? imageURL { get; set; }

    [Display(Name = "Description")] public string description { get; set; }

    //recipe
    [Display(Name = "RecipeId")] public int recipeId { get; set; }

    public Recipe recipe { get; set; }

    //Relationships
}