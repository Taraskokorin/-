using System.Reflection;
using HomeBudget.Transactions;

namespace HomeBudget.Plugins;

public static class PluginLoader
{
    public static string DefaultFolder =>
        Path.Combine(AppContext.BaseDirectory, "plugins");

    public static IReadOnlyList<string> LoadInto(TransactionCatalog catalog,
                                                 string? folder = null)
    {
        folder ??= DefaultFolder;

        var report = new List<string>();

        if (!Directory.Exists(folder))
            return report;

        var files = Directory.GetFiles(folder, "*.dll").OrderBy(f => f);

        foreach (var file in files)
        {
            var name = Path.GetFileName(file);

            try
            {
                // LoadFrom вистачає для навчального завдання. У справжньому
                // застосунку беруть AssemblyLoadContext: він дає змогу
                // вивантажити складник і розділити версії бібліотек.
                var assembly = Assembly.LoadFrom(file);
                var added = catalog.Add(assembly);

                if (added > 0)
                    report.Add($"{name}: різновидів {added}");
            }
            catch (BadImageFormatException)
            {
                report.Add($"{name}: це не збірка .NET");
            }
            catch (Exception ex) when (ex is FileLoadException
                                          or FileNotFoundException
                                          or TypeLoadException
                                          or InvalidOperationException)
            {
                // Пошкоджений складник не повинен зупиняти застосунок.
                report.Add($"{name}: не завантажено ({ex.Message})");
            }
        }

        return report;
    }
}
