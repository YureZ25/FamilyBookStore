using Data.Context.Contracts;
using Data.Entities;
using Data.Enums;
using Data.Extensions;
using Data.Repos.Contracts;
using Microsoft.Identity.Client;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.BookVMs;

namespace Services.Services
{
    internal class BookService : IBookService
    {
        private readonly IUsersBooksStatusRepo _usersBooksStatusRepo;
        private readonly IBookImageRepo _bookImageRepo;
        private readonly IBookRepo _bookRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public BookService(
            IUsersBooksStatusRepo usersBooksStatusRepo,
            IBookImageRepo bookImageRepo,
            IBookRepo bookRepo,
            IUnitOfWork unitOfWork,
            IAuthService authService)
        {
            _usersBooksStatusRepo = usersBooksStatusRepo;
            _bookImageRepo = bookImageRepo;
            _bookRepo = bookRepo;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<IEnumerable<BookGetVM>> GetUserBooksByStatus(BookStatus bookStatus, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser();

            var books = await _bookRepo.GetBooksByUserStatus(user.Id, bookStatus, cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksByStore(int storeId, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksByStore(storeId, cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksByAuthor(int authorId, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksByAuthor(authorId, cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksByGenre(int genreId, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksByGenre(genreId, cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<IEnumerable<BookGetVM>> GetBooks(CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetAll(cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<BookGetVM> GetById(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetById(id, cancellationToken);

            var user = await _authService.GetCurrentUser();
            book.Status = await _usersBooksStatusRepo.GetStatus(user.Id, book.Id, cancellationToken);

            return book.Map();
        }

        public async Task<ResultVM<BookGetVM>> Insert(BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            if (bookVM.Image != null)
            {
                book.Image = new BookImage();
                var imageResult = ProcessImage(book.Image, bookVM);
                if (!imageResult.Success) return imageResult;
            }

            _bookRepo.Insert(book);
            _bookRepo.AttachToStore(book);

            if (bookVM.BookStatus is not BookStatus.None)
            {
                var user = await _authService.GetCurrentUser();
                book.Status = new UsersBooksStatus
                {
                    Book = book,
                    UserId = user.Id,
                };

                var statusResult = ProcessStatus(book.Status, bookVM);
                if (!statusResult.Success) return statusResult;
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            return new(book.Map());
        }

        public async Task<ResultVM<BookGetVM>> Update(BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            if (bookVM.Image != null)
            {
                book.Image = await _bookImageRepo.GetByBookId(book.Id, cancellationToken) ?? new BookImage();
                var imageResult = ProcessImage(book.Image, bookVM);
                if (!imageResult.Success) return imageResult;
            }

            _bookRepo.Update(book);

            if (book.Store != null && !await _bookRepo.AttachedToStore(book.Id, book.Store.Id, cancellationToken))
            {
                _bookRepo.DetachFromStore(book);
                _bookRepo.AttachToStore(book);
            }

            var user = await _authService.GetCurrentUser();
            book.Status = await _usersBooksStatusRepo.GetStatus(user.Id, book.Id, cancellationToken)
                ?? new UsersBooksStatus
                {
                    BookId = book.Id,
                    UserId = user.Id,
                };

            var statusResult = ProcessStatus(book.Status, bookVM);
            if (!statusResult.Success) return statusResult;

            await _unitOfWork.SaveChanges(cancellationToken);

            return new(book.Map());
        }

        public async Task<ResultVM<BookGetVM>> DeleteById(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetById(id, cancellationToken);

            var user = await _authService.GetCurrentUser();
            var status = await _usersBooksStatusRepo.GetStatus(user.Id, book.Id, cancellationToken);
            if (status is not null)
            {
                _usersBooksStatusRepo.DeleteById(status.Id);
            }

            _bookRepo.DetachFromStore(book);

            _bookRepo.DeleteById(book.Id);

            await _unitOfWork.SaveChanges(cancellationToken);

            return new(book.Map());
        }

        private ResultVM<BookGetVM> ProcessStatus(UsersBooksStatus status, BookPostVM bookVM)
        {
            if (bookVM.BookStatus is BookStatus.Read && !bookVM.PageCount.HasValue)
            {
                return new("BookPost.BookStatus", $"При статусе '{bookVM.BookStatus.GetDescription()}' должно быть указано количество страниц книги.");
            }

            if (bookVM.BookStatus is BookStatus.Reading or BookStatus.Dropped && !bookVM.CurrentPage.HasValue)
            {
                return new("BookPost.BookStatus", $"При статусе '{bookVM.BookStatus.GetDescription()}' должно быть указано количество прочитанных страниц книги.");
            }

            if (bookVM.CurrentPage.HasValue && bookVM.CurrentPage > bookVM.PageCount)
            {
                return new("BookPost.CurrentPage", $"Количество прочитанных страниц не может быть больше общего их числа");
            }

            status.CurrentPage = bookVM.CurrentPage;

            if (status.BookStatus != bookVM.BookStatus)
            {
                var now = DateTime.Now;
                switch (bookVM.BookStatus)
                {
                    case BookStatus.None: break;
                    case BookStatus.WillRead:
                        status.WishRead = now;
                        break;
                    case BookStatus.Reading:
                        status.StartRead = now;
                        status.CurrentPage = bookVM.CurrentPage ?? 0;
                        break;
                    case BookStatus.Read:
                        status.CurrentPage = bookVM.PageCount;
                        status.EndRead = now;
                        break;
                    case BookStatus.Dropped:
                        status.CurrentPage = bookVM.CurrentPage;
                        status.EndRead = now;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            status.BookStatus = bookVM.BookStatus;

            if (status.Id != default)
            {
                _usersBooksStatusRepo.Update(status);
            }
            else
            {
                _usersBooksStatusRepo.Insert(status);
            }

            return new(new BookGetVM());
        }

        private ResultVM<BookGetVM> ProcessImage(BookImage bookImage, BookPostVM bookVM)
        {
            const int maxSize = 5 * 1024 * 1024;

            if (bookVM.Image.Length > maxSize)
            {
                return new("BookPost.Image", "Обложка книги не может быть больше 5 Мб");
            }

            bookImage.FileName = bookVM.Image.FileName;
            bookImage.ContentType = bookVM.Image.ContentType;

            using MemoryStream ms = new();
            bookVM.Image.CopyTo(ms);
            bookImage.Content = ms.ToArray();

            if (bookImage.Id != default)
            {
                _bookImageRepo.Update(bookImage);
            }
            else
            {
                _bookImageRepo.Insert(bookImage);
            }

            return new(new BookGetVM());
        }
    }
}
