using System;

namespace VirtualDebts.Services;

public delegate void StateChangedHandler();
public delegate bool StateUpdater<TState>(TState state);

public class Store<TState> where TState : ICloneable, new()
{
    private readonly object stateLock = new();
    private readonly TState state = new();

    public TState GetState()
    {
        lock (this.stateLock)
        {
            return (TState)this.state.Clone();
        }
    }

    public bool Update(StateUpdater<TState> transform)
    {
        lock (this.stateLock)
        {
            bool isSuccess = transform(this.state);
            this.StateChanged?.Invoke();
            return isSuccess;
        }
    }

    public event StateChangedHandler StateChanged;
}
