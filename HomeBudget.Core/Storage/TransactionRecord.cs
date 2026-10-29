namespace HomeBudget.Storage;

/// <summary>
/// Те, що справді лежить у файлі: спільні поля плюс словник
/// із даними, які потрібні конкретному різновиду.
/// </summary>
internal class TransactionRecord
{
    public int Id { get; set; }
    public string Kind { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "Інше";

    /// <summary>Дата у форматі ISO 8601 – однаково для будь-якої мови системи.</summary>
    public string Date { get; set; } = "";

    public decimal Amount { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
}
