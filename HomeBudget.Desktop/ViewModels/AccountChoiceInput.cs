namespace HomeBudget.Desktop.ViewModels;

public class AccountChoiceInput : AccountInput
{
    private int selectedIndex = -1;

    public AccountChoiceInput(string prompt, IReadOnlyList<string> accounts)
    {
        Prompt = prompt;
        Options = accounts;
    }

    public string Prompt { get; }

    public IReadOnlyList<string> Options { get; }

    public int SelectedIndex
    {
        get => selectedIndex;
        set { if (Set(ref selectedIndex, value)) Raise(nameof(IsReady)); }
    }

    public override bool IsReady => SelectedIndex >= 0 && SelectedIndex < Options.Count;

    public override string Given => IsReady ? Options[SelectedIndex] : "";
}
