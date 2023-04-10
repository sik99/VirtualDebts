using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using VirtualDebts.Controllers;

namespace VirtualDebts.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainMenuView : ContentPage
{
    public MainMenuView(MainMenuController controller)
    {
        InitializeComponent();
        BindingContext = controller;
    }
}
