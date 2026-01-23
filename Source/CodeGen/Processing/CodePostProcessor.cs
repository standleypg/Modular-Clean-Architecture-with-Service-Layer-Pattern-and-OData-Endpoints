using System.Text.RegularExpressions;

namespace CodeGen.Processing;

/// <summary>
/// Post-processes generated TypeScript code for cleanup.
/// </summary>
public static partial class CodePostProcessor
{
    /// <summary>
    /// Post-processes the generated TypeScript code for final cleanup.
    /// This handles edge cases that can't be addressed by the formatters.
    /// </summary>
    public static string Process(string tsCode)
    {
        // Remove any leftover empty Guid declarations that might slip through
        tsCode = RemoveGuidDeclaration().Replace(tsCode, string.Empty).Trim();

        // Also remove standalone IGuid interface declarations
        tsCode = RemoveIGuidInterface().Replace(tsCode, string.Empty).Trim();

        // Clean up multiple empty lines
        tsCode = Cleanup().Replace(tsCode, "\n\n");

        return tsCode.Trim();
    }

    [GeneratedRegex(@"declare\s+namespace\s+System\s*\{\s*(?:export\s+)?interface\s+I?Guid\s*\{[^}]*\}\s*\}", RegexOptions.Multiline)]
    private static partial Regex RemoveGuidDeclaration();

    [GeneratedRegex(@"export\s+interface\s+IGuid\s*\{[^}]*\}", RegexOptions.Multiline)]
    private static partial Regex RemoveIGuidInterface();

    [GeneratedRegex(@"\n\s*\n\s*\n", RegexOptions.Multiline)]
    private static partial Regex Cleanup();
}