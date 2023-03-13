using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json;

namespace Communication.Server
{
    public class AutoConverter
    {
        public AutoConverter() { }
        public static T ConvertToType<T>(Dictionary<string, object> record)
        {
            var instance = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();

            if (record is null)
            {
                return instance;
            }

            foreach (var item in properties.Select((Property, Index) => (Property, Index)))
            {
                var attr = item.Property.GetCustomAttribute<ColumnAttribute>();

                if (attr != null)
                {
                    try
                    {
                        item.Property.SetValue(instance, ((JsonElement)record[attr.Name]).
                            Deserialize(item.Property.PropertyType), null);
                    }
                    catch { }
                }
            }

            return instance;
        }
        public static IEnumerable<T> ConvertToEnumerableOfTypes<T>(IEnumerable<Dictionary<string, object>> records)
        {
            if (records is null)
            {
                return Enumerable.Empty<T>();
            }

            var items = new List<T>(records.Count());

            foreach (var record in records)
            {
                items.Add(ConvertToType<T>(record));
            }

            return items;
        }
    }
}