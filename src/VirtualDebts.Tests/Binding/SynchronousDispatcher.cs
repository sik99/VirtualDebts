using System;

namespace VirtualDebts.Binding
{
    public class SynchronousDispatcher : IDispatcher
    {
        public void InvokeInMainThread(Action action)
        {
            action.Invoke();
        }
    }
}