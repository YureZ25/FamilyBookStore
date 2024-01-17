namespace Services.Services.Contracts
{
    public interface IImageService
    {
        Task<(byte[] content, string contentType)> GetBookImage(int? bookId, CancellationToken cancellationToken);
    }
}
