using Data.Context.Contracts;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Services.Services
{
    internal class BookService : IBookService
    {
        private readonly IBookRepo _bookRepo;
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IBookRepo bookRepo, IUnitOfWork unitOfWork)
        {
            _bookRepo = bookRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookVM>> GetBooksByStoreAsync(int storeId, CancellationToken cancellationToken)
        {
            return await GetBooksAsync(cancellationToken);
        }

        public async Task<IEnumerable<BookVM>> GetBooksAsync(CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksAsync(cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<BookVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetByIdAsync(id, cancellationToken);

            return book.Map();
        }

        public async Task<BookVM> InsertAsync(BookVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Insert(book);

            _bookRepo.AttachToStore(book);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }

        public async Task<BookVM> UpdateAsync(BookVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Update(book);

            if (book.Store != null)
            {
                book = await _bookRepo.GetByIdAsync(book.Id, cancellationToken);
                if (book.Store.Id != bookVM.Store.Id)
                {
                    _bookRepo.DetachFromStore(book);
                    _bookRepo.AttachToStore(book);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }

        public async Task<BookVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetByIdAsync(id, cancellationToken);

            _bookRepo.DetachFromStore(book);

            _bookRepo.DeleteById(book.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }
    }
}
