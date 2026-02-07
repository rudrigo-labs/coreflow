using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreFlow.Shared.Extensions
{
    /// <summary>
    /// Extensões para serialização e desserialização JSON.
    /// </summary>
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        /// <summary>
        /// Serializa um objeto para JSON.
        /// </summary>
        /// <typeparam name="T">O tipo do objeto a ser serializado.</typeparam>
        /// <param name="value">O objeto a ser serializado.</param>
        /// <param name="writeIndented">Indica se o JSON deve ser formatado com indentação.</param>
        /// <returns>String JSON representando o objeto.</returns>
        public static string Serialize<T>(this T value, bool writeIndented = false)
        {
            if (value is string str)
                return str;
            
            if (value is int intValue)
                return intValue.ToString();

            var options = new JsonSerializerOptions(JsonOptions);
            if (writeIndented)
                options.WriteIndented = true;

            try
            {
                return JsonSerializer.Serialize(value, options);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to serialize object of type {typeof(T).Name}.", ex);
            }
        }

        /// <summary>
        /// Desserializa uma string JSON para um objeto.
        /// </summary>
        /// <typeparam name="T">O tipo do objeto a ser desserializado.</typeparam>
        /// <param name="data">A string JSON a ser desserializada.</param>
        /// <returns>O objeto desserializado.</returns>
        /// <exception cref="JsonException">Lançada quando a string JSON é inválida.</exception>
        public static T Deserialize<T>(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                if (typeof(T).IsValueType)
                    return default(T)!;
                return Activator.CreateInstance<T>();
            }

            if (typeof(T) == typeof(string))
                return (T)(object)data;

            if (typeof(T) == typeof(int))
            {
                if (int.TryParse(data, out var intValue))
                    return (T)(object)intValue;
                throw new FormatException($"Cannot convert '{data}' to {typeof(T).Name}.");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(data, JsonOptions) ?? Activator.CreateInstance<T>();
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Failed to deserialize JSON to {typeof(T).Name}.", ex);
            }
        }

        /// <summary>
        /// Serializa uma lista para JSON.
        /// </summary>
        /// <typeparam name="T">O tipo dos itens na lista.</typeparam>
        /// <param name="list">A lista a ser serializada.</param>
        /// <returns>String JSON representando a lista.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando list é null.</exception>
        public static string SerializeList<T>(this List<T> list)
        {
            ArgumentNullException.ThrowIfNull(list);
            return JsonSerializer.Serialize(list, new JsonSerializerOptions());
        }
    }
}

