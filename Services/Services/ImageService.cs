using Microsoft.AspNetCore.Http;
using Services.Services.Contracts;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;
using System.Text;

namespace Services.Services
{
    internal class ImageService : IImageService
    {
        private readonly IBookService _bookService;

        public ImageService(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<(byte[] content, string contentType)> GetBookImage(int? bookId, CancellationToken cancellationToken)
        {
            // Get img from DB

            var book = bookId.HasValue
                ? await _bookService.GetByIdAsync(bookId.Value, cancellationToken)
                : new BookGetVM { Title = "Название книги", Author = new AuthorGetVM { FirstName = "Имя", LastName = "Автора" } };

            return (Encoding.Default.GetBytes(GetBookImageSvg(book)), "image/svg+xml");
        }

        public string GetBookImageSvg(BookGetVM bookVM, string cssClasses = "")
        {
            var textHtml = $"""
                <span style="text-align: center; color: #dee2e6;" xmlns="http://www.w3.org/1999/xhtml">
                    <p style="font-size: x-large;">{bookVM.Title}</p>
                    <p style="font-size: large; font-style: italic;">{bookVM.Author}</p>
                </span>
                """;

            return $"""
                <svg class="{cssClasses}" height="400px" width="260px" xmlns="http://www.w3.org/2000/svg" role="img">
                    <title>Book cover thumbnail</title>
                    <rect fill="#7f7f7f" height="100%" width="100%" />
                    <foreignObject x="10%" y="15%" width="80%" height="100%">
                        {textHtml}
                    </foreignObject>
                </svg>
                """;
        }
    }
}
