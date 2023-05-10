using System;

namespace Strongly
{
    /// <summary>
    /// Converters used to to serialize/deserialize strongly-typed ID values
    /// </summary>
    [Flags]
    public enum StronglyConverter
    {

        /// <summary>
        /// Don't create any converters for the strong type
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the default converters for the strong type.
        /// This will be the value provided in the <see cref="StronglyDefaultsAttribute"/>, which falls back to
        /// <see cref="TypeConverter"/> and <see cref="SystemTextJson"/>
        /// </summary>
        Default = 1,

        /// <summary>
        /// Creates a <see cref="TypeConverter"/> for converting from the strong type to and from a string
        /// </summary>
        TypeConverter = 2,

        /// <summary>
        /// Creates a Newtonsoft.Json.JsonConverter for serializing the strong type to its primitive value
        /// </summary>
        NewtonsoftJson = 4,

        /// <summary>
        /// Creates a System.Text.Json.Serialization.JsonConverter for serializing the strong type to its primitive value
        /// </summary>
        SystemTextJson = 8,

        /// <summary>
        /// Creates an EF Core Value Converter for extracting the primitive value
        /// </summary>
        EfValueConverter = 16,

        /// <summary>
        /// Creates a Dapper TypeHandler for converting to and from the type
        /// </summary>
        DapperTypeHandler = 32,

        /// <summary>
        /// Creates a Swagger SchemaFilter for OpenApi documentation
        /// </summary>
        SwaggerSchemaFilter = 64,
    }
}