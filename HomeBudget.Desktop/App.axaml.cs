using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HomeBudget.Desktop.ViewModels;
using HomeBudget.Storage;

namespace HomeBudget.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var model = new MainViewModel(BudgetLocation.DataFile);
            var window = new MainWindow { DataContext = model };

            // Читання журналу повільне – починаємо його, коли вікно вже видно.
            // Обробник навмисно не async: виняток усередині async void
            // аварійно завершив би процес без жодного повідомлення.
            window.Opened += (_, _) => _ = model.LoadAsync();

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}