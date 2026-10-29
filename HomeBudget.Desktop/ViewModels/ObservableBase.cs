using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HomeBudget.Desktop.ViewModels;

public abstract class ObservableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // [CallerMemberName] підставляє ім’я властивості, з якої викликали
    // метод, – тому назв властивостей рядками в коді немає.
    protected void Raise([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool Set<T>(ref T field, T value,
                          [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        Raise(name);

        return true;
    }
}
