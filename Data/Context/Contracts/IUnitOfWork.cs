namespace Data.Context.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
