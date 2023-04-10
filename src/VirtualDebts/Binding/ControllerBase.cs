using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VirtualDebts.Binding;

public abstract class ControllerBase : INotifyPropertyChanged
{
    protected readonly IDispatcher dispatcher;

    protected ControllerBase(IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(this.dispatcher));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // This method is called by the Set accessor of each property.
    // The CallerMemberName attribute that is applied to the optional propertyName
    // parameter causes the property name of the caller to be substituted as an argument.
    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}