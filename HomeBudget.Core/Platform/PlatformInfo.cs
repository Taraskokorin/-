using System.Runtime.InteropServices;
using HomeBudget.Storage;

namespace HomeBudget.Platform;

public static class PlatformInfo
{
    public static string Describe()
    {
        var lines = new[]
        {
            $"Операційна система: {RuntimeInformation.OSDescription}",
            $"Архітектура процесора: {RuntimeInformation.ProcessArchitecture}",
            "Ідентифікатор середовища (RID): " + RuntimeInformation.RuntimeIdentifier,
            $"Версія .NET: {RuntimeInformation.FrameworkDescription}",
            $"Роздільник шляхів: {Path.DirectorySeparatorChar}",
            $"Каталог застосунку: {AppContext.BaseDirectory}",
            $"Каталог даних: {BudgetLocation.Folder}"
        };

        return string.Join(System.Environment.NewLine, lines);
    }

    public static string OsName =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows"
        : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS"
        : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux"
        : "невідома система";
}
