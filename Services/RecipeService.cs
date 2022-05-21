using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services;

public interface IRecipeService
{
    Task<List<CategoryModel>> GetCategories();
    Task<List<RecipeModel>> GetRecipes();
    Task<RecipeModel> GetRecipe(int recipeId);
    Task<RecipeModel> Create(CreateRecipeRequest request);
    Task<RecipeModel> Update(UpdateRecipeRequest request);
}

public class RecipeService : IRecipeService
{
    private readonly IHttpService _httpService;

    public RecipeService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<List<CategoryModel>> GetCategories()
    {
        return await _httpService.Get<List<CategoryModel>>("recipes/categories");
    }
    public async Task<List<RecipeModel>> GetRecipes()
    {
        return await _httpService.Get<List<RecipeModel>>("recipes/get-all");
    }
    public async Task<RecipeModel> GetRecipe(int recipeId)
    {
        return await _httpService.Get<RecipeModel>("recipes/" + recipeId);
    }
    public async Task<RecipeModel> Create(CreateRecipeRequest request)
    {
        return await _httpService.Post<RecipeModel>("recipes/create", request);
    }
    public async Task<RecipeModel> Update(UpdateRecipeRequest request)
    {
        return await _httpService.Put<RecipeModel>("recipes/update", request);
    }
}