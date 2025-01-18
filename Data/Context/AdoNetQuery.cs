using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data.Common;
using System.Reflection;

namespace Data.Context
{
    internal class AdoNetQuery<TEntity>
        where TEntity : class, IEntity
    {
        private readonly SqlCommand _command;
        private readonly Func<DbDataReader, TEntity> _mapFunc;

        public AdoNetQuery(SqlCommand command, Func<DbDataReader, TEntity> mapFunc)
        {
            _command = command;
            _mapFunc = mapFunc;
        }

        public async Task<IList<TEntity>> ToList(CancellationToken cancellationToken)
        {
            using var reader = await _command.ExecuteReaderAsync(cancellationToken);

            var entities = new List<TEntity>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var entity = _mapFunc(reader);
                if (!MapCollectionNavigations(entities, entity))
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task<TEntity> FirstOrDefault(CancellationToken cancellationToken)
        {
            var entities = await ToList(cancellationToken);

            return entities.FirstOrDefault();
        }

        private bool MapCollectionNavigations(List<TEntity> entities, TEntity entity)
        {
            if (entity is not ICommonEntity commonEntity) return false;

            var collectionNavigations = typeof(TEntity).GetProperties()
                .Where(e => 
                    e.PropertyType.IsGenericType && 
                    e.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) && 
                    typeof(ICommonEntity).IsAssignableFrom(e.PropertyType.GetGenericArguments().Single()))
                .ToArray();

            var existEntity = entities.SingleOrDefault(e => ((ICommonEntity)e).Id == commonEntity.Id);
            
            if (!collectionNavigations.Any() || existEntity == null) return false;

            foreach (var collectionNavigation in collectionNavigations)
            {
                var collection = collectionNavigation.GetValue(entity);
                var collectionType = collectionNavigation.PropertyType.GetGenericArguments().Single();

                var singleOrDefaultMethod = typeof(Enumerable).GetMethods()
                    .Single(m => 
                        m.IsGenericMethod && 
                        m.Name == nameof(Enumerable.SingleOrDefault) && 
                        m.GetParameters().Length == 1)
                    .MakeGenericMethod(collectionType);
                var collectionElement = singleOrDefaultMethod.Invoke(null, [collection]);
                if (collectionElement == null) continue;

                var existCollection = collectionNavigation.GetValue(existEntity);

                var addMethod = collectionNavigation.PropertyType.GetMethod(nameof(ICollection<ICommonEntity>.Add));
                var containsMethod = typeof(Enumerable).GetMethods()
                    .Single(m => 
                        m.IsGenericMethod && 
                        m.Name == nameof(Enumerable.Contains) && 
                        m.GetParameters().Length == 3)
                    .MakeGenericMethod(collectionType);

                if (!(bool)containsMethod.Invoke(null, [existCollection, collectionElement, new CommonEntityEqualityComparer()]))
                {
                    addMethod.Invoke(existCollection, [collectionElement]);
                }
            }

            return true;
        }
    }
}
