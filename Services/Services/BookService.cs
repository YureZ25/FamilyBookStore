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

        public async Task<BookVM> GetById(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetByIdAsync(id, cancellationToken);

            return book.Map();
        }

        public async Task<BookVM> InsertAsync(BookVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Insert(book);

            var inserted = await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }

        public async Task<BookVM> UpdateAsync(BookVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Update(book);

            var updated = await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }

        public async Task<BookVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await GetById(id, cancellationToken);

            _bookRepo.DeleteById(book.Id);

            var deleted = await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book;
        }
    }
}
