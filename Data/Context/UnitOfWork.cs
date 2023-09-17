using Data.Context.Contracts;

namespace Data.Context
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AdoNetDbContext _dbContext;

        public UnitOfWork(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
