using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using VirtualDebts.Controllers;

namespace VirtualDebts.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EditUsersView : ContentPage
{
    public EditUsersView(EditUsersController controller)
    {
        InitializeComponent();
        BindingContext = controller;
    }
}