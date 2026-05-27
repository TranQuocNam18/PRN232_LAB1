using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Models.Requests
{
    public class QueryParams
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string? Fields { get; set; }
        public string? Expand { get; set; }
    }

    /// <summary>
    /// Helper class for applying field filtering to response objects.
    /// Fields not specified in the Fields parameter will be set to null 
    /// and automatically excluded from JSON serialization.
    /// </summary>
    public static class FieldFilterHelper
    {
        /// <summary>
        /// Applies field filtering to an object based on comma-separated field names.
        /// Example: "studentId,fullName,email" will only include these fields
        /// </summary>
        public static T? ApplyFieldFilter<T>(T? obj, string? fields) where T : class
        {
            if (string.IsNullOrWhiteSpace(fields) || obj == null)
                return obj;

            var allowedFields = fields.Split(',')
                .Select(f => f.Trim().ToLower())
                .ToHashSet();

            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                // If field is not in the allowed list and property is writable, set it to null
                if (!allowedFields.Contains(prop.Name.ToLower()) && prop.CanWrite)
                {
                    // For value types, set to default; for reference types, set to null
                    if (prop.PropertyType.IsValueType && !IsNullableType(prop.PropertyType))
                        prop.SetValue(obj, Activator.CreateInstance(prop.PropertyType));
                    else
                        prop.SetValue(obj, null);
                }
            }

            return obj;
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
