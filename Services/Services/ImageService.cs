using Data.Context.Contracts;
using Data.Repos.Contracts;
using Microsoft.AspNetCore.Http;
using Services.Extensions;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;
using System.Text;

namespace Services.Services
{
    internal class ImageService : IImageService
    {
        private readonly IBookService _bookService;
        private readonly IBookImageRepo _bookImageRepo;
        private readonly IBookRepo _bookRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ImageService(IBookService bookService, IBookImageRepo bookImageRepo, IBookRepo bookRepo, IUnitOfWork unitOfWork)
        {
            _bookService = bookService;
            _bookImageRepo = bookImageRepo;
            _bookRepo = bookRepo;
            _unitOfWork = unitOfWork;
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
                : new BookGetVM { Title = "Название книги", Author = new AuthorGetVM { FullName = "Имя Автора" } };

            return (Encoding.Default.GetBytes(GetBookImageSvg(book)), "image/svg+xml");
        }

        public async Task<ResultVM> SetImage(IFormFile image, int bookId, CancellationToken cancellationToken)
        {
            if (image == null)
            {
                return new("Image.ImagePost", "Пожалуйста, выберите обожку книги");
            }

            var book = await _bookRepo.GetById(bookId, cancellationToken);
            book.Image = book.ImageId.HasValue ? await _bookImageRepo.GetById(book.ImageId.Value, cancellationToken) : new();

            book.Image.FileName = image.FileName;
            book.Image.ContentType = image.ContentType;

            using MemoryStream ms = new();
            image.CopyTo(ms);
            book.Image.Content = ms.ToArray();

            if (!book.Image.Validate(out var errors))
            {
                return new("Image.ImagePost", errors.First().ErrorMessage);
            }

            if (book.Image.Id != default)
            {
                _bookImageRepo.Update(book.Image);
            }
            else
            {
                _bookImageRepo.Insert(book.Image);
                _bookRepo.SetImage(book);
            }

            await _unitOfWork.SaveChanges(cancellationToken);
            return new();
        }

        private static string GetBookImageSvg(BookGetVM bookVM, string cssClasses = "")
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
