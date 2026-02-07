using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace CoreFlow.Shared.Extensions
{
    public static class GenericsExtensions
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = properties[i];
                dataTable.Columns.Add(propertyDescriptor.Name, propertyDescriptor.PropertyType);
            }

            object[] array = new object[properties.Count];
            foreach (T datum in data)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    array[j] = properties[j].GetValue(datum);
                }

                dataTable.Rows.Add(array);
            }

            return dataTable;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string Join<TSource>(this IEnumerable<TSource> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static string Description<T>(this T source)
        {
            FieldInfo field = source.GetType().GetField(source.ToString());
            DescriptionAttribute[] array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
            if (array != null && array.Length != 0)
            {
                return array[0].Description;
            }

            return source.ToString();
        }
    }
}

