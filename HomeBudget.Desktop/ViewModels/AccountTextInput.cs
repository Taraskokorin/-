namespace HomeBudget.Desktop.ViewModels;

public class AccountTextInput : AccountInput
{
    private string text = "";

    public AccountTextInput(string prompt) => Prompt = prompt;

    public string Prompt { get; }

    public string Text
    {
        get => text;
        // Очищене поле може передати null – BalanceEffect його не чекає.
        set { if (Set(ref text, value ?? "")) Raise(nameof(IsReady)); }
    }

    public override bool IsReady => !string.IsNullOrWhiteSpace(Text);

    public override string Given => Text;
}
