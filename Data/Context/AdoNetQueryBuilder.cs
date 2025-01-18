using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace Data.Context
{
    internal class AdoNetQueryBuilder<TEntity>
        where TEntity : class, IEntity
    {
        private readonly SqlCommand _command;
        private readonly Func<DbDataReader, TEntity> _mapFunc;

        public AdoNetQueryBuilder(SqlCommand command, Func<DbDataReader, TEntity> mapFunc)
        {
            _command = command;
            _mapFunc = mapFunc;
        }

        public AdoNetQuery<TEntity> Build()
        {
            return new(_command, _mapFunc);
        }

        public AdoNetQueryBuilder<TEntity> WithText(string text)
        {
            _command.CommandText = text;
            return this;
        }

        public AdoNetQueryBuilder<TEntity> WithParameter<TProp>(Expression<Func<TEntity, TProp>> targetProperty, TProp value)
        {
            if (targetProperty.Body is not MemberExpression propExpression)
            {
                throw new ApplicationException("Expression must access member of entity type");
            }

            return WithParameter(propExpression.Member.Name, value);
        }

        public AdoNetQueryBuilder<TEntity> WithParameter(string parameter, object value)
        {   
            _command.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                ParameterName = parameter,
                Value = value,
            });
            return this;
        }
    }
}
