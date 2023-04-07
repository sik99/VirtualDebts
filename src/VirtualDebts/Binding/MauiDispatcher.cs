using Microsoft.Maui.ApplicationModel;
using System;

namespace VirtualDebts.Binding;

public class MauiDispatcher : IDispatcher
{
    public void InvokeInMainThread(Action action) => MainThread.BeginInvokeOnMainThread(action);
}