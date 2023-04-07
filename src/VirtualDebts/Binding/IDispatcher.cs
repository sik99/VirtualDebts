using System;

namespace VirtualDebts.Binding;

public interface IDispatcher
{
    public void InvokeInMainThread(Action action);
}