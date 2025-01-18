using Data.Context.Contracts;
using Data.Context.Models;
using Data.Entities.Contracts;
using Data.Extensions;
using Microsoft.Data.SqlClient;

namespace Data.Context
{
    internal class AdoNetCommand<TEntity> : IAdoNetCommand
        where TEntity : class, IEntity
    {
        private readonly SqlCommand _command;
        private readonly TEntity _entity;

        private readonly List<AdoNetParameter<TEntity>> _parameters;
        private readonly List<AdoNetNavigation> _navigations;

        public AdoNetCommand(SqlCommand command, TEntity entity, List<AdoNetParameter<TEntity>> parameters, List<AdoNetNavigation> navigations)
        {
            _command = command;
            _entity = entity;
            _parameters = parameters;
            _navigations = navigations;
        }

        public Type EntityType => typeof(TEntity);

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

        public void ApplyNavigationsUpdates(IEntity target)
        {
            if (target is not TEntity entity) throw new ApplicationException($"Can't apply updates of {EntityType} type for {target.GetType()} type entity");

            foreach (var (key, nav) in _navigations.Select(x => (x.ForeignKey, x.Navigation)))
            {
                if (key.PropertyType.IsNullable())
                {
                    var foreignKey = (int?)key.GetValue(entity);
                    var navigation = (ICommonEntity)nav.GetValue(entity);

                    if (navigation?.Id == foreignKey) continue;

                    if (foreignKey != default && navigation?.Id == default)
                    {
                        navigation ??= (ICommonEntity)Activator.CreateInstance(nav.PropertyType);
                        navigation.Id = foreignKey ?? 0;
                    }
                    else if (navigation?.Id != default && foreignKey == default)
                    {
                        foreignKey = navigation.Id;
                    }

                    key.SetValue(entity, foreignKey);
                    nav.SetValue(entity, navigation);
                }
                else
                {
                    var foreignKey = (int)key.GetValue(entity);
                    var navigation = (ICommonEntity)nav.GetValue(entity);

                    if (navigation?.Id == foreignKey) continue;

                    if (foreignKey != default && navigation?.Id == default)
                    {
                        navigation ??= (ICommonEntity)Activator.CreateInstance(nav.PropertyType);
                        navigation.Id = foreignKey;
                    }
                    else if (navigation?.Id != default && foreignKey == default)
                    {
                        foreignKey = navigation.Id;
                    }

                    key.SetValue(entity, foreignKey);
                    nav.SetValue(entity, navigation);
                }
            }

            ApplyParametersUpdates(entity);
        }

        public async Task<int> ExecuteNonQuery(CancellationToken cancellationToken)
        {
            return await _command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
