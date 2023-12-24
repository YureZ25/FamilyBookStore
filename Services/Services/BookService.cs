using Data.Context.Contracts;
using Data.Entities;
using Data.Enums;
using Data.Extensions;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.BookVMs;

namespace Services.Services
{
    internal class BookService : IBookService
    {
        private readonly IUsersBooksStatusRepo _usersBooksStatusRepo;
        private readonly IBookRepo _bookRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        public BookService(IUsersBooksStatusRepo usersBooksStatusRepo, IBookRepo bookRepo, IUnitOfWork unitOfWork, IAuthService authService)
        {
            _usersBooksStatusRepo = usersBooksStatusRepo;
            _bookRepo = bookRepo;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<IEnumerable<BookGetVM>> GetUserBooksByStatusAsync(BookStatus bookStatus, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser();

            var books = await _bookRepo.GetBooksByUserStatusAsync(user.Id, bookStatus, cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksByStoreAsync(int storeId, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksByStoreAsync(storeId, cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksAsync(CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksAsync(cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<BookGetVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetByIdAsync(id, cancellationToken);

            var user = await _authService.GetCurrentUser();
            book.Status = await _usersBooksStatusRepo.GeStatusAsync(user.Id, book.Id, cancellationToken);

            return book.Map();
        }

        public async Task<ResultVM<BookGetVM>> InsertAsync(BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Insert(book);
            _bookRepo.AttachToStore(book);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (bookVM.BookStatus is BookStatus.None)
            {
                return new(book.Map());
            }

            var user = await _authService.GetCurrentUser();
            book.Status = new UsersBooksStatus
            {
                BookId = book.Id,
                UserId = user.Id,
            };

            var result = ProcessStatus(book.Status, bookVM);
            if (!result.Success) return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new(book.Map());
        }

        public async Task<ResultVM<BookGetVM>> UpdateAsync(BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Update(book);

            if (book.Store != null && !await _bookRepo.AttachedToStore(book.Id, book.Store.Id, cancellationToken))
            {
                _bookRepo.DetachFromStore(book);
                _bookRepo.AttachToStore(book);
            }

            var user = await _authService.GetCurrentUser();
            book.Status = await _usersBooksStatusRepo.GeStatusAsync(user.Id, book.Id, cancellationToken)
                ?? new UsersBooksStatus
                {
                    BookId = book.Id,
                    UserId = user.Id,
                };

            if (bookVM.BookStatus == book.Status.BookStatus)
            {
                if (bookVM.BookStatus is BookStatus.Reading)
                {
                    book.Status.CurrentPage = bookVM.CurrentPage;
                    _usersBooksStatusRepo.Update(book.Status);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new(book.Map());
            }

            var result = ProcessStatus(book.Status, bookVM);
            if (!result.Success) return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new(book.Map());
        }

        public async Task<ResultVM<BookGetVM>> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetByIdAsync(id, cancellationToken);

            _bookRepo.DetachFromStore(book);

            _bookRepo.DeleteById(book.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new(book.Map());
        }

        private ResultVM<BookGetVM> ProcessStatus(UsersBooksStatus status, BookPostVM bookVM)
        {
            var now = DateTime.Now;

            if (bookVM.BookStatus is BookStatus.Reading or BookStatus.Read or BookStatus.Dropped && !bookVM.CurrentPage.HasValue)
            {
                return new("BookPost.BookStatus", $"При статусе '{bookVM.BookStatus.GetDescription()}' должно быть указано количество страниц книги.");
            }

            if (bookVM.CurrentPage.HasValue && bookVM.CurrentPage > bookVM.PageCount)
            {
                return new("BookPost.CurrentPage", $"Количество прочитанных страниц не может быть больше общего их числа");
            }

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

            status.BookStatus = bookVM.BookStatus;

            if (status.Id != 0)
            {
                _usersBooksStatusRepo.Update(status);
            }
            else
            {
                _usersBooksStatusRepo.Insert(status);
            }

            return new(new BookGetVM());
        }
    }
}
