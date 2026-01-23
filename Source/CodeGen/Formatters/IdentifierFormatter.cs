using TypeLitePlus.TsModels;

namespace CodeGen.Formatters;

/// <summary>
/// Handles identifier naming conventions (properties, interfaces, etc.).
/// </summary>
public class IdentifierFormatter
{
    /// <summary>
    /// Converts property names to camelCase.
    /// </summary>
    public static string FormatPropertyName(TsProperty formatter)
    {
        if (string.IsNullOrEmpty(formatter.Name))
            return formatter.Name;

        return char.ToLowerInvariant(formatter.Name[0]) + formatter.Name[1..];
    }

    /// <summary>
    /// Formats module names and adds 'I' prefix to interfaces.
    /// </summary>
    public static string FormatModuleName(TsModule formatter)
    {
        foreach (var member in formatter.Members)
        {
            // Skip if already has I prefix
            if (member.Name.StartsWith('I'))
                continue;

            // Skip generic type parameters (T, TKey, TValue, etc.)
            if (IsGenericTypeParameter(member.Name))
                continue;

            // Only add I prefix to classes (interfaces), NOT to enums
            if (member is TsClass)
            {
                member.Name = "I" + member.Name;
            }
        }

        return formatter.Name;
    }

    /// <summary>
    /// Checks if a name is likely a generic type parameter (T, TKey, TValue, etc.)
    /// </summary>
    private static bool IsGenericTypeParameter(string name)
    {
        return name.Length <= 6 &&
               name.StartsWith('T') &&
               (name.Length == 1 || char.IsUpper(name[1]));
    }
}