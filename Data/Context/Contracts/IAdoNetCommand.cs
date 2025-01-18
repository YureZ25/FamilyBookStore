using Data.Entities.Contracts;

namespace Data.Context.Contracts
{
    internal interface IAdoNetCommand
    {
        Type EntityType { get; }

        void ApplyEntityUpdates(IEntity target);
        void ApplyParametersUpdates(IEntity target);
        void ApplyNavigationsUpdates(IEntity target);
        Task<int> ExecuteNonQuery(CancellationToken cancellationToken);
    }
}
