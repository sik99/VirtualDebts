using AsyncAwaitBestPractices.MVVM; // AsyncCommand
using CommunityToolkit.Mvvm.Input; // RelayCommand
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VirtualDebts.Binding;

public class CommandFactory : ICommandFactory
{
    public ICommand Create(Action action)
    {
        return new RelayCommand(
            execute: action,
            canExecute: () => true
        );
    }

    public ICommand Create(Action action, Func<bool> canExecute)
    {
        return new RelayCommand(
            execute: action,
            canExecute: () => canExecute()
        );
    }

    public ICommand Create(Action<object?> action, Predicate<object?> canExecute)
    {
        return new RelayCommand<object>(
            execute: action,
            canExecute: parameter => canExecute(parameter)
        );
    }

    public IAsyncCommand CreateAsync(Func<Task> action)
    {
        return new AsyncCommand(
            execute: action,
            canExecute: _ => true
        );
    }

    public IAsyncCommand CreateAsync(Func<Task> action, Func<bool> canExecute)
    {
        return new AsyncCommand(
            execute: action,
            canExecute: _ => canExecute()
        );
    }

    public IAsyncCommand<object?> CreateAsync(Func<object?, Task> action, Predicate<object?> canExecute)
    {
        return new AsyncCommand<object?>(
            execute: action,
            canExecute: parameter => canExecute(parameter)
        );
    }
}
