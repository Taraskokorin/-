namespace HomeBudget.Transactions;

[AttributeUsage(AttributeTargets.Class,
                AllowMultiple = false, Inherited = false)]
public sealed class TransactionKindAttribute : Attribute
{
    public TransactionKindAttribute(string kind, string title)
    {
        Kind = kind;
        Title = title;
    }

    /// <summary>Коротка позначка, яка потрапляє у файл журналу.</summary>
    public string Kind { get; }

    /// <summary>Назва різновиду для людини.</summary>
    public string Title { get; }
}
