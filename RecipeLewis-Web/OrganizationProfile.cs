using AutoMapper;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<CreateRecipeRequest, RecipeModel>();
        CreateMap<UpdateRecipeRequest, RecipeModel>();
        CreateMap<CreateRecipeRequest, UpdateRecipeRequest>();
        CreateMap<RecipeModel, CreateRecipeRequest>();
        CreateMap<RecipeModel, UpdateRecipeRequest>();
    }
}