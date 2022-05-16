using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services;

public interface IRecipeService
{
    Task<List<RecipeModel>> GetRecipes();
    Task<RecipeModel> Create(CreateRecipeRequest request);
}

public class RecipeService : IRecipeService
{
    private IHttpService _httpService;

    public RecipeService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<List<RecipeModel>> GetRecipes()
    {
        return await _httpService.Get<List<RecipeModel>>("recipes/get-all");
    }
    public async Task<RecipeModel> Create(CreateRecipeRequest request)
    {
        return await _httpService.Post<RecipeModel>("recipes/create", request);
    }
}