using AsyncAwaitBestPractices.MVVM;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VirtualDebts.Binding;

public interface ICommandFactory
{
    public ICommand Create(Action action);
    public ICommand Create(Action action, Func<bool> canExecute);
    public ICommand Create(Action<object> action, Predicate<object> canExecute);
    public IAsyncCommand CreateAsync(Func<Task> action);
    public IAsyncCommand CreateAsync(Func<Task> action, Func<bool> canExecute);
    public IAsyncCommand<object> CreateAsync(Func<object, Task> action, Predicate<object> canExecute);
}