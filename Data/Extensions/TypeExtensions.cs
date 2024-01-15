using System.Reflection;

namespace Data.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasCustomAttribute<TAttr>(this Type type)
            where TAttr : Attribute
        {
            return type.GetCustomAttribute<TAttr>() is not null;
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}
