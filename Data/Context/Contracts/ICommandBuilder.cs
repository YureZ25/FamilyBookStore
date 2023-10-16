using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;

namespace Data.Context.Contracts
{
    internal interface ICommandBuilder
    {
        Type EntityType { get; }
        SqlCommand Command { get; }

        void ApplyEntityUpdates(IEntity target);
        void ApplyParametersUpdates(IEntity target);
    }
}
