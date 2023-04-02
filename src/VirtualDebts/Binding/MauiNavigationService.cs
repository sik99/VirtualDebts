using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using VirtualDebts.Resources.Strings;

namespace VirtualDebts.Binding
{
    public class MauiNavigationService : INavigationService
    {
        private readonly MauiViewProvider viewProvider;

        public MauiNavigationService(MauiViewProvider viewProvider)
        {
            this.viewProvider = viewProvider ?? throw new ArgumentNullException(nameof(viewProvider));
        }

        public async Task NavigateTo(ViewId viewId)
        {
            await Application.Current.MainPage.Navigation?.PushAsync(this.viewProvider.GetView(viewId));
        }

        public async Task ShowMessageBox(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, AppResources.MessageBox_OkButton);
        }
    }
}
