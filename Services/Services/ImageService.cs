using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;
using System.Text;

namespace Services.Services
{
    internal class ImageService : IImageService
    {
        private readonly IBookService _bookService;
        private readonly IBookImageRepo _bookImageRepo;

        public ImageService(IBookService bookService, IBookImageRepo bookImageRepo)
        {
            _bookService = bookService;
            _bookImageRepo = bookImageRepo;
        }

        public async Task<(byte[] content, string contentType)> GetBookImage(int? bookId, CancellationToken cancellationToken)
        {
            if (bookId.HasValue)
            {
                var img = await _bookImageRepo.GetByBookId(bookId.Value, cancellationToken);
                if (img != null) return (img.Content, img.ContentType);
            }

            var book = bookId.HasValue
                ? await _bookService.GetById(bookId.Value, cancellationToken)
                : new BookGetVM { Title = "Название книги", Author = new AuthorGetVM { FirstName = "Имя", LastName = "Автора" } };

            return (Encoding.Default.GetBytes(GetBookImageSvg(book)), "image/svg+xml");
        }

        public string GetBookImageSvg(BookGetVM bookVM, string cssClasses = "")
        {
            var textHtml = $"""
                <span style="text-align: center; color: #dee2e6;" xmlns="http://www.w3.org/1999/xhtml">
                    <p style="font-size: x-large;">{bookVM.Title}</p>
                    <p style="font-size: large; font-style: italic;">{bookVM.Author.FullName}</p>
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
