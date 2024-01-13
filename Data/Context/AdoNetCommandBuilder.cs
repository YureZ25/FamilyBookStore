using Data.Context.Contracts;
using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Data.Context
{
    internal class AdoNetCommandBuilder<TEntity> : ICommandBuilder
        where TEntity : class, IEntity
    {
        private readonly SqlCommand _command;
        private readonly TEntity _entity;

        private readonly List<AdoNetParameter> _parameters = [];
        private readonly List<AdoNetNavigation> _navigations = [];

        public AdoNetCommandBuilder(SqlCommand sqlCommand, TEntity entity)
        {
            _command = sqlCommand;
            _entity = entity;
            CollectNavigationProps();
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

            var adoNetParameter = new AdoNetParameter
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

                    if (navigation.ForeignKey.PropertyType != typeof(int))
                    {
                        throw new ApplicationException("Foreign key property must be int type");
                    }

                    _navigations.Add(navigation);
                }
            }
        }

        public void ApplyNavigationsUpdates(IEntity target)
        {
            if (target is not TEntity entity) throw new ApplicationException($"Can't apply updates of {EntityType} type for {target.GetType()} type entity");

            foreach (var nav in _navigations)
            {
                var foreignKey = (int)nav.ForeignKey.GetValue(entity);
                var navigation = (ICommonEntity)nav.Navigation.GetValue(entity);

                if (navigation?.Id == foreignKey) continue;

                if (foreignKey != default && navigation?.Id == default)
                {
                    navigation ??= (ICommonEntity)Activator.CreateInstance(nav.Navigation.PropertyType);
                    navigation.Id = foreignKey;
                }
                else if (navigation?.Id != default && foreignKey == default)
                {
                    foreignKey = navigation.Id;
                }

                nav.ForeignKey.SetValue(entity, foreignKey);
                nav.Navigation.SetValue(entity, navigation);
            }

            ApplyParametersUpdates(entity);
        }

        private class AdoNetNavigation
        {
            public PropertyInfo ForeignKey { get; set; }
            public PropertyInfo Navigation { get; set; }
        }
    }
}
