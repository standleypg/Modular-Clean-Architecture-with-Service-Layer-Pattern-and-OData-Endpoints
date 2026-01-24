using CodeGen.Configuration;

namespace CodeGen.Utilities;

public class PathResolver(TypeScriptGenConfig config)
{
    public string ResolveOutputPath()
    {
        var outputDir = config.OutputPath;

        // If path is relative, resolve it relative to the base directory
        if (!Path.IsPathRooted(outputDir))
        {
            var baseDirectory = config.BaseDirectory ?? FindBaseDirectory();
            outputDir = Path.Combine(baseDirectory, outputDir);
        }

        return Path.GetFullPath(Path.Combine(outputDir, config.OutputFileName));
    }

    private static string FindBaseDirectory()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);

        // Navigate up to find a directory containing a .sln or .csproj file
        while (currentDir != null)
        {
            if (currentDir.GetFiles("*.sln").Length > 0 ||
                currentDir.GetFiles("*.csproj").Length > 0)
            {
                return currentDir.FullName;
            }

            currentDir = currentDir.Parent;
        }

        // Fallback to the traditional Parent.Parent.Parent navigation
        var fallback = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;

        if (fallback == null)
        {
            throw new InvalidOperationException(
                "Could not determine base directory. Please specify BaseDirectory in configuration.");
        }

        return fallback;
    }
}