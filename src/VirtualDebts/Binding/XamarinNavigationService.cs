using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VirtualDebts.Binding
{
    public class XamarinNavigationService : INavigationService
    {
        private readonly XamarinViewProvider viewProvider;

        public XamarinNavigationService(XamarinViewProvider viewProvider)
        {
            this.viewProvider = viewProvider ?? throw new ArgumentNullException(nameof(viewProvider));
        }

        public async Task NavigateTo(ViewId viewId)
        {
            await Application.Current.MainPage.Navigation?.PushAsync(this.viewProvider.GetView(viewId));
        }

        public async Task ShowMessageBox(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, Properties.Resources.MessageBox_OkButton);
        }
    }
}
