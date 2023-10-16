using Data.Context.Contracts;
using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;

namespace Data.Context
{
    internal class AdoNetCommandBuilder<TEntity> : ICommandBuilder
        where TEntity : class, IEntity
    {
        private readonly SqlCommand _command;
        private readonly TEntity _entity;

        private List<AdoNetParameter> _parameters = new();

        public AdoNetCommandBuilder(SqlCommand sqlCommand, TEntity entity)
        {
            _command = sqlCommand;
            _entity = entity;
        }

        public SqlCommand Command => _command;
        public Type EntityType => typeof(TEntity);

        public AdoNetCommandBuilder<TEntity> WithText(string text)
        {
            _command.CommandText = text;

            return this;
        }

        public AdoNetCommandBuilder<TEntity> WithParameter<TProp>(Expression<Func<TEntity, TProp>> targetProperty, ParameterDirection direction = ParameterDirection.Input)
        {
            return WithParameter(targetProperty, null, direction);
        }

        public AdoNetCommandBuilder<TEntity> WithParameter<TProp>(Expression<Func<TEntity, TProp>> targetProperty, string explicitName, ParameterDirection direction = ParameterDirection.Input)
        {
            if (targetProperty.Body is not MemberExpression memberExpression)
            {
                throw new ApplicationException("Expression must access member of entity type");
            }

            var memberName = explicitName ?? memberExpression.Member.Name;

            var sqlParameter = new SqlParameter
            {
                ParameterName = memberName,
                Direction = direction,
            };
            _command.Parameters.Add(sqlParameter);

            var entityParam = targetProperty.Parameters.First();

            var paramExpr = Expression.Parameter(typeof(SqlParameter), "p");
            var paramValueExpr = Expression.Property(paramExpr, nameof(SqlParameter.Value));

            var assignPropToParamExpr = Expression.Assign(paramValueExpr, Expression.Convert(memberExpression, typeof(object)));
            var assignPropToParam = Expression.Lambda<Action<SqlParameter, TEntity>>(assignPropToParamExpr, paramExpr, entityParam).Compile();

            assignPropToParam(sqlParameter, _entity);

            var adoNetParameter = new AdoNetParameter
            {
                Parameter = sqlParameter,
                AssignPropToParam = assignPropToParam,
            };
            _parameters.Add(adoNetParameter);

            if (direction is ParameterDirection.Input) return this;

            var assignParamToPropExpr = Expression.Assign(memberExpression, Expression.Convert(paramValueExpr, memberExpression.Type));
            var assignParamToProp = Expression.Lambda<Action<TEntity, SqlParameter>>(assignParamToPropExpr, entityParam, paramExpr).Compile();

            adoNetParameter.AssignParamToProp = assignParamToProp;

            return this;
        }

        public void ApplyEntityUpdates(IEntity target)
        {
            if (target is not TEntity entity) throw new ApplicationException($"Can't apply updates of {EntityType} type for {target.GetType()} type entity");

            foreach (var (sqlParameter, assignParamToProp) in _parameters.Select(p => (p.Parameter, p.AssignParamToProp)))
            {
                if (assignParamToProp is null) continue;

                assignParamToProp(entity, sqlParameter);
            }
        }

        public void ApplyParametersUpdates(IEntity target)
        {
            if (target is not TEntity entity) throw new ApplicationException($"Can't apply updates of {EntityType} type for {target.GetType()} type entity");

            foreach (var (sqlParameter, assignPropToParam) in _parameters.Select(p => (p.Parameter, p.AssignPropToParam)))
            {
                assignPropToParam(sqlParameter, entity);
            }
        }

        private class AdoNetParameter
        {
            public SqlParameter Parameter { get; set; }
            public Action<TEntity, SqlParameter> AssignParamToProp { get; set; }
            public Action<SqlParameter, TEntity> AssignPropToParam { get; set; }
        }
    }
}
