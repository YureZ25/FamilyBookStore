using Data.Entities.Contracts;

namespace Data.Extensions
{
    public static class CacheExtensions
    {
        public static string GetCacheKey(this ICommonEntity entity)
        {
            return $"{entity.GetType().Name}_{entity.Id}";
        }
    }
}
