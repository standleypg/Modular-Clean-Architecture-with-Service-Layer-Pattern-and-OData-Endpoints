using System.Reflection;
using System.Text;

namespace RetailPortal.Data.Db.Sql;

public static class SqlHelper
{
    public static string GetSqlFromFile(string resourceName, string version)
    {
        return GetSqlFromFile(resourceName, resourceName, version);
    }
    public static string GetSqlFromFile(string folderName, string resourceName, string version)
    {
        var fileName = $"{resourceName}_{version}.sql";
        var assembly = Assembly.GetExecutingAssembly();
        using var file = assembly.GetManifestResourceStream($"RetailPortal.Data.Db.Sql.{folderName}.{resourceName}.{fileName}");
        if (file == null)
        {
            throw new FileNotFoundException($"{fileName} not found in {folderName}. Make sure the file exists and is an embedded resource.");
        }

        using var reader = new StreamReader(file, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}