using Data.Context.Models;
using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace Data.Context
{
    internal class AdoNetCommandBuilder<TEntity>
        where TEntity : class, IEntity
    {
        private readonly AdoNetDbContext _context;
        private readonly SqlCommand _command;
        private readonly TEntity _entity;

        private readonly List<AdoNetParameter<TEntity>> _parameters = [];
        private readonly List<AdoNetNavigation> _navigations = [];

        public AdoNetCommandBuilder(AdoNetDbContext context, SqlCommand sqlCommand, TEntity entity)
        {
            _context = context;
            _command = sqlCommand;
            _entity = entity;
            CollectNavigationProps();
        }

        private static Type EntityType => typeof(TEntity);

        public AdoNetCommand<TEntity> Build()
        {
            var cmd = new AdoNetCommand<TEntity>(_command, _entity, _parameters, _navigations);
            _context.Commands.Add(cmd, _entity);
            return cmd;
        }

        public AdoNetCommandBuilder<TEntity> WithText(string text)
        {
            _command.CommandText = text;

            return this;
        }

        public AdoNetCommandBuilder<TEntity> WithParameter<TProp>(Expression<Func<TEntity, TProp>> targetProperty, TProp value, ParameterDirection direction = ParameterDirection.Input)
        {
            if (targetProperty.Body is not MemberExpression propExpression)
            {
                throw new ApplicationException("Expression must access member of entity type");
            }

            _command.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                ParameterName = propExpression.Member.Name,
                Value = value,
            });
            return this;
        }

        public AdoNetCommandBuilder<TEntity> WithParameter<TProp>(Expression<Func<TEntity, TProp>> targetProperty, ParameterDirection direction = ParameterDirection.Input)
        {
            return WithParameter(targetProperty, null, direction);
        }

        public AdoNetCommandBuilder<TEntity> WithParameter<TProp>(Expression<Func<TEntity, TProp>> targetProperty, string explicitName, ParameterDirection direction = ParameterDirection.Input)
        {
            if (targetProperty.Body is not MemberExpression propExpression)
            {
                throw new ApplicationException("Expression must access member of entity type");
            }

            var memberName = explicitName ?? propExpression.Member.Name;

            var sqlParameter = new SqlParameter
            {
                ParameterName = memberName,
                Direction = direction,
            };
            _command.Parameters.Add(sqlParameter);

            var entityParam = targetProperty.Parameters.First();

            var paramExpr = Expression.Parameter(typeof(SqlParameter), "p");
            var paramValueExpr = Expression.Property(paramExpr, nameof(SqlParameter.Value));

            var propObjectExp = Expression.Convert(propExpression, typeof(object));
            var checkedPropExpr = Expression.Condition(Expression.NotEqual(propObjectExp, Expression.Constant(null)), propObjectExp, Expression.Constant(DBNull.Value, typeof(object)));

            var assignPropToParamExpr = Expression.Assign(paramValueExpr, checkedPropExpr);
            var assignPropToParam = Expression.Lambda<Action<SqlParameter, TEntity>>(assignPropToParamExpr, paramExpr, entityParam).Compile();

            assignPropToParam(sqlParameter, _entity);

            var adoNetParameter = new AdoNetParameter<TEntity>
            {
                Parameter = sqlParameter,
                AssignPropToParam = assignPropToParam,
            };
            _parameters.Add(adoNetParameter);

            if (direction is ParameterDirection.Input) return this;

            var assignParamToPropExpr = Expression.Assign(propExpression, Expression.Convert(paramValueExpr, propExpression.Type));
            var assignParamToProp = Expression.Lambda<Action<TEntity, SqlParameter>>(assignParamToPropExpr, entityParam, paramExpr).Compile();

            adoNetParameter.AssignParamToProp = assignParamToProp;

            return this;
        }

        private void CollectNavigationProps()
        {
            foreach (var prop in EntityType.GetProperties())
            {
                if (prop.PropertyType.IsAssignableTo(typeof(ICommonEntity)))
                {
                    var navigation = new AdoNetNavigation
                    {
                        Navigation = prop,
                    };

                    if (prop.GetCustomAttribute<ForeignKeyAttribute>() is { } attr
                        && EntityType.GetProperty(attr.Name) is { } foreignKey)
                    {
                        navigation.ForeignKey = foreignKey;
                    }
                    else if (EntityType.GetProperties().FirstOrDefault(p => 
                        p.GetCustomAttribute<ForeignKeyAttribute>() is { } attr && attr.Name == prop.Name) is { } foreignKeyProp)
                    {
                        navigation.ForeignKey = foreignKeyProp;
                    }
                    else
                    {
                        continue;
                    }

                    if (navigation.ForeignKey.PropertyType != typeof(int) && navigation.ForeignKey.PropertyType != typeof(int?))
                    {
                        throw new ApplicationException("Foreign key property must be int type");
                    }

                    _navigations.Add(navigation);
                }
            }
        }
    }
}
