using System;

namespace VirtualDebts.Binding
{
    public class XamarinDispatcher : IDispatcher
    {
        public void InvokeInMainThread(Action action) => App.Current.Dispatcher.BeginInvokeOnMainThread(action);
    }
}