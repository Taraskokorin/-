namespace HomeBudget.Storage;

/// <summary>
/// Де лежить журнал користувача. Каталог застосунку для цього не годиться:
/// самодостатню збірку часто кладуть туди, куди звичайний користувач писати
/// не може, а під час оновлення вміст цього каталогу замінюють.
/// </summary>
public static class BudgetLocation
{
    /// <summary>
    /// Windows – %APPDATA%\HomeBudget,
    /// Linux і macOS – ~/.config/HomeBudget.
    /// </summary>
    public static string Folder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "HomeBudget");

    public static string DataFile => Path.Combine(Folder, "transactions.json");
}
