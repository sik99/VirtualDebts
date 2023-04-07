using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using VirtualDebts.Resources.Strings;

namespace VirtualDebts.Binding;

public class MauiNavigationService : INavigationService
{
    public static void RegisterView(Type viewType)
    {
        Routing.RegisterRoute(viewType.Name, viewType);
    }

    public Task NavigateTo(Type viewType)
    {
        return Shell.Current.GoToAsync(viewType.Name);
    }

    public Task ShowMessageBox(string title, string message)
    {
        return Shell.Current.DisplayAlert(title, message, AppResources.MessageBox_OkButton);
    }
}
