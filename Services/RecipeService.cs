using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace BlazorApp.Services;

public interface IRecipeService
{
    Task<List<CategoryModel>> GetCategories();

    Task<List<RecipeModel>> GetRecipes();

    Task<RecipeModel> GetRecipe(int recipeId);

    Task<List<DocumentModel>> GetRecipeDocuments(int recipeId);

    Task<RecipeModel> Create(CreateRecipeRequest request);

    Task<RecipeModel> Update(UpdateRecipeRequest request);

    Task<bool> Delete(int recipeId);
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
        return await _httpService.Get<RecipeModel>("recipes/get/" + recipeId);
    }

    public async Task<List<DocumentModel>> GetRecipeDocuments(int recipeId)
    {
        return await _httpService.Get<List<DocumentModel>>("recipes/documents/" + recipeId);
    }

    public async Task<RecipeModel> Create(CreateRecipeRequest request)
    {
        return await _httpService.Post<RecipeModel>("recipes/create", request);
    }

    public async Task<RecipeModel> Update(UpdateRecipeRequest request)
    {
        return await _httpService.Put<RecipeModel>("recipes/update", request);
    }

    public async Task<bool> Delete(int recipeId)
    {
        return await _httpService.Delete<bool>("recipes/delete/" + recipeId);
    }
}