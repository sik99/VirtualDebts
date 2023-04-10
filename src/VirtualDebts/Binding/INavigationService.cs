using System;
using System.Threading.Tasks;

namespace VirtualDebts.Binding;

public interface INavigationService
{
    public Task NavigateTo(Type viewType);
    public Task ShowMessageBox(string title, string message);
}
