using System.ComponentModel;
using System.Reflection;

namespace Data.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            var fieldName = Enum.GetName(value);
            if (!value.HasDescription())
            {
                return fieldName;
            }
            return typeof(TEnum).GetField(fieldName).GetCustomAttribute<DescriptionAttribute>().Description;
        }

        public static bool HasDescription<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            var fieldName = Enum.GetName(value);
            var field = typeof(TEnum).GetField(fieldName);
            return field.GetCustomAttribute<DescriptionAttribute>() is { Description: not null };
        }

        public static IEnumerable<TEnum> WithDescription<TEnum>(this IEnumerable<TEnum> enums)
            where TEnum : struct, Enum
        {
            return enums.Where(e => e.HasDescription());
        }
    }
}
