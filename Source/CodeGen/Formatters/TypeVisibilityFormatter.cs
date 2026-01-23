namespace CodeGen.Formatters;

/// <summary>
/// Handles type visibility (which types to show/hide).
/// </summary>
public class TypeVisibilityFormatter
{
    /// <summary>
    /// Determines if a type should be visible in the output.
    /// </summary>
    public static bool IsTypeVisible(string typeName)
    {
        // Hide Guid in all its variations
        if (typeName.Contains("Guid", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }
}