using Microsoft.Extensions.Logging;
using TypeLitePlus.TsModels;

namespace CodeGen.Formatters;

/// <summary>
/// Handles TypeScript type formatting and conversions.
/// </summary>
public class TypeFormatter(ILogger? logger = null)
{
    /// <summary>
    /// Maps C# types to appropriate TypeScript types.
    /// </summary>
    public string FormatMemberType(TsProperty formatter, string typeName)
    {
        string baseType = GetBaseType(typeName);
        bool isNullable = this.IsNullable(formatter);

        // Add | null for nullable types
        if (isNullable)
        {
            return $"{baseType} | null";
        }

        return baseType;
    }

    /// <summary>
    /// Gets the base TypeScript type for a given C# type name.
    /// </summary>
    private static string GetBaseType(string typeName)
    {
        // Handle Guid types - map to string
        if (typeName.Contains("Guid", StringComparison.OrdinalIgnoreCase))
        {
            return "string";
        }

        // Map other types
        return typeName switch
        {
            "System.DateTime" => "Date",
            "System.DateTimeOffset" => "Date",
            _ => typeName
        };
    }

    /// <summary>
    /// Checks if a property/field is nullable.
    /// Modified to handle both value types and reference types.
    /// </summary>
    private bool IsNullable(TsProperty tsprop)
    {
        try
        {
            var clrProperty = tsprop.MemberInfo;
            if (clrProperty == null)
            {
                return false;
            }

            // Get the actual type from PropertyInfo or FieldInfo
            Type? propertyType;

            if (clrProperty is System.Reflection.PropertyInfo propertyInfo)
            {
                propertyType = propertyInfo.PropertyType;

                // Check for nullable value types (int?, Guid?, DateTime?, etc.)
                if (Nullable.GetUnderlyingType(propertyType) != null)
                {
                    logger?.LogDebug("Property {Name} is nullable value type", tsprop.Name);
                    return true;
                }

                // Check for nullable reference types (string?, MyClass?, etc.) using NullabilityInfoContext
                #if NET6_0_OR_GREATER
                try
                {
                    var context = new System.Reflection.NullabilityInfoContext();
                    var nullabilityInfo = context.Create(propertyInfo);
                    if (nullabilityInfo.ReadState == System.Reflection.NullabilityState.Nullable)
                    {
                        logger?.LogDebug("Property {Name} is nullable reference type", tsprop.Name);
                        return true;
                    }
                }
                catch { /* Ignore errors in nullability detection */ }
                #endif
            }
            else if (clrProperty is System.Reflection.FieldInfo fieldInfo)
            {
                propertyType = fieldInfo.FieldType;

                // Check for nullable value types
                if (Nullable.GetUnderlyingType(propertyType) != null)
                {
                    logger?.LogDebug("Field {Name} is nullable value type", tsprop.Name);
                    return true;
                }

                // Check for nullable reference types using NullabilityInfoContext
                try
                {
                    var context = new System.Reflection.NullabilityInfoContext();
                    var nullabilityInfo = context.Create(fieldInfo);
                    if (nullabilityInfo.ReadState == System.Reflection.NullabilityState.Nullable)
                    {
                        logger?.LogDebug("Field {Name} is nullable reference type", tsprop.Name);
                        return true;
                    }
                }
                catch { /* Ignore errors in nullability detection */ }
            }

            return false;
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Error checking nullable for property {Name}", tsprop.Name);
            return false;
        }
    }
}