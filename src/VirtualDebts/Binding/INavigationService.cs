using System.Threading.Tasks;

namespace VirtualDebts.Binding
{
    public interface INavigationService
    {
        public Task NavigateTo(ViewId viewId);
        public Task ShowMessageBox(string title, string message);
    }
}
