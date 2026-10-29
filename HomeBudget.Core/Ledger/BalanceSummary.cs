namespace HomeBudget.Ledger;

/// <summary>Підсумок: скільки операцій розглянуто, скільки надійшло й скільки витрачено.</summary>
public record BalanceSummary(int Count, int Skipped, decimal Income, decimal Expense)
{
    /// <summary>Чиста зміна балансу.</summary>
    public decimal Net => Income - Expense;

    /// <summary>Скільки операцій справді змінили баланс.</summary>
    public int Applied => Count - Skipped;

    /// <summary>Додає до підсумку вплив однієї операції.</summary>
    public static BalanceSummary operator +(BalanceSummary s, decimal effect) =>
        effect switch
        {
            > 0 => s with { Count = s.Count + 1, Income = s.Income + effect },
            < 0 => s with { Count = s.Count + 1, Expense = s.Expense - effect },
            _ => s with { Count = s.Count + 1, Skipped = s.Skipped + 1 }
        };
}
