using System.Globalization;

namespace HomeBudget.Transactions;

/// <summary>Читання суми, яку ввела людина.</summary>
public static class Money
{
    /// <summary>
    /// Спершу за налаштуваннями системи (в українській – кома), потім
    /// за незмінними правилами (крапка). Кому на крапку наосліп не замінюємо.
    /// </summary>
    public static bool TryParse(string text, out decimal value)
    {
        var trimmed = text.Trim();

        return decimal.TryParse(trimmed, NumberStyles.Number,
                   CultureInfo.CurrentCulture, out value)
            || decimal.TryParse(trimmed, NumberStyles.Number,
                   CultureInfo.InvariantCulture, out value);
    }
}
