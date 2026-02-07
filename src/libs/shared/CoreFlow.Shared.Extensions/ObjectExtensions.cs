using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreFlow.Shared.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Execute uma cópia profunda do objeto, usando Json como método de serialização. NOTA: membros privados não são clonados usando este método.
        /// </summary>
        /// <typeparam name="T">O tipo de objeto que está sendo copiado.</typeparam>
        /// <param name="source">A instância do objeto a ser copiada.</param>
        /// <returns>O objeto copiado.</returns>
        public static T? Clone<T>(this T source)
        {
            if (ReferenceEquals(source, null)) return default;

            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source), new JsonSerializerOptions
            {
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
            });
        }
    }
}

