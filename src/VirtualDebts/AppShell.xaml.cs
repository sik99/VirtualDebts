using Microsoft.Maui.Controls;
using VirtualDebts.Binding;
using VirtualDebts.Views;

namespace VirtualDebts;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        MauiNavigationService.RegisterView(typeof(EditUsersView));
        MauiNavigationService.RegisterView(typeof(EmptyView));
	}
}
