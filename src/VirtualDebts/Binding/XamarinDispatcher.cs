using Microsoft.Maui.ApplicationModel;
using System;

namespace VirtualDebts.Binding
{
    public class XamarinDispatcher : IDispatcher
    {
        public void InvokeInMainThread(Action action) => MainThread.BeginInvokeOnMainThread(action);
    }
}