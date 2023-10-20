﻿using Data.Context.Contracts;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels.BookVMs;

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

        public async Task<IEnumerable<BookGetVM>> GetBooksByStoreAsync(int storeId, CancellationToken cancellationToken)
        {
            return await GetBooksAsync(cancellationToken);
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksAsync(CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksAsync(cancellationToken);

            return books.Select(e => e.Map());
        }

        public async Task<BookGetVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetByIdAsync(id, cancellationToken);

            return book.Map();
        }

        public async Task<BookGetVM> InsertAsync(BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Insert(book);

            _bookRepo.AttachToStore(book);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }

        public async Task<BookGetVM> UpdateAsync(BookPostVM bookVM, CancellationToken cancellationToken)
        {
            var book = bookVM.Map();

            _bookRepo.Update(book);

            if (book.Store != null && !await _bookRepo.AttachedToStore(book.Id, book.Store.Id, cancellationToken))
            {
                _bookRepo.DetachFromStore(book);
                _bookRepo.AttachToStore(book);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }

        public async Task<BookGetVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetByIdAsync(id, cancellationToken);

            _bookRepo.DetachFromStore(book);

            _bookRepo.DeleteById(book.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return book.Map();
        }
    }
}
