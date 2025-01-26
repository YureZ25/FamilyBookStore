using Data.Context.Contracts;
using Data.Entities;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.BookQuoteVMs;

namespace Services.Services
{
    internal class BookQuoteService : IBookQuoteService
    {
        private readonly IBookQuoteRepo _bookQuoteRepo;
        private readonly IBookRepo _bookRepo;
        private readonly IUnitOfWork _unitOfWork;

        public BookQuoteService(IBookQuoteRepo bookQuoteRepo, IBookRepo bookRepo, IUnitOfWork unitOfWork)
        {
            _bookQuoteRepo = bookQuoteRepo;
            _bookRepo = bookRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookQuoteGetVM>> GetByBook(int bookId, CancellationToken cancellationToken)
        {
            var quotes = await _bookQuoteRepo.GetByBookId(bookId, cancellationToken);

            return quotes.Select(e => e.Map());
        }

        public async Task<BookQuoteGetVM> GetById(int id, CancellationToken cancellationToken)
        {
            var quote = await _bookQuoteRepo.GetById(id, cancellationToken);

            return quote.Map();
        }

        public async Task<ResultVM<BookQuoteGetVM>> Insert(BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken)
        {
            var quote = bookQuoteVM.Map();

            if (await Validate(quote, cancellationToken) is { Success: false } validateionError) return validateionError;

            _bookQuoteRepo.Insert(quote);

            await _unitOfWork.SaveChanges(cancellationToken);

            return new(quote.Map());
        }

        public async Task<ResultVM<BookQuoteGetVM>> Update(BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken)
        {
            var quote = bookQuoteVM.Map();

            if (await Validate(quote, cancellationToken) is { Success: false } validateionError) return validateionError;

            _bookQuoteRepo.Update(quote);

            await _unitOfWork.SaveChanges(cancellationToken);

            return new(quote.Map());
        }

        public async Task<ResultVM<BookQuoteGetVM>> DeleteById(int id, CancellationToken cancellationToken)
        {
            var quote = await _bookQuoteRepo.GetById(id, cancellationToken);

            _bookQuoteRepo.DeleteById(quote.Id);

            await _unitOfWork.SaveChanges(cancellationToken);

            return new(quote.Map());
        }

        private async Task<ResultVM<BookQuoteGetVM>> Validate(BookQuote quote, CancellationToken cancellationToken)
        {
            var book = await _bookRepo.GetById(quote.BookId, cancellationToken);
            if (!book.PageCount.HasValue)
            {
                return new("BookQuotePost.Page", "Сначала введите общее кол-во страниц книги");
            }
            else if (quote.Page > book.PageCount.Value)
            {
                return new("BookQuotePost.Page", "Страница цитаты не может быть больше общего их числа");
            }

            return new(quote.Map());
        }
    }
}
