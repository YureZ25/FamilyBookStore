using Microsoft.AspNetCore.Http;
using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IImageService
    {
        Task<(byte[] content, string contentType)> GetBookImage(int? bookId, CancellationToken cancellationToken);
        Task<ResultVM> SetImage(IFormFile image, int bookId, CancellationToken cancellationToken);
    }
}
