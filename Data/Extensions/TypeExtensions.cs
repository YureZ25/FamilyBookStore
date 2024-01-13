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
    }
}
