using Models.Results;

namespace BlazorApp.Services;

public interface IDocumentService
{
    Task<List<UploadResult>> Upload(MultipartFormDataContent request, int recipeId);
}

public class DocumentService : IDocumentService
{
    private readonly IHttpService _httpService;

    public DocumentService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<List<UploadResult>> Upload(MultipartFormDataContent request, int recipeId)
    {
        return await _httpService.PostForm<List<UploadResult>>("documents/upload/multiple/" + recipeId, request);
    }
}