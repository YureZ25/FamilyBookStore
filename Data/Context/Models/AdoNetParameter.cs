using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;

namespace Data.Context.Models
{
    internal class AdoNetParameter<TEntity>
        where TEntity : class, IEntity
    {
        public SqlParameter Parameter { get; set; }
        public Action<TEntity, SqlParameter> AssignParamToProp { get; set; }
        public Action<SqlParameter, TEntity> AssignPropToParam { get; set; }
    }
}
