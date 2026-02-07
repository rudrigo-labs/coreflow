using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CoreFlow.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static List<T> ToList<T>(this T value) where T : Enum
        {
            return value.ToString().Split(',').Select(flag => (T)Enum.Parse(typeof(T), flag)).ToList();
        }

        public static string GetDisplayName<T>(this T enumValue) where T : Enum
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?
                            .GetName() ?? enumValue.ToString();
        }

        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            throw new ArgumentException("Item not found.", nameof(value));
        }

        public static int ToInt<TEnum>(this TEnum enumValue) where TEnum : Enum
        {
            return Convert.ToInt32(enumValue);
        }
    }
}

