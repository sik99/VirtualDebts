using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace VirtualDebts.Binding
{
    public class XamarinCommandFactory : ICommandFactory
    {
        public ICommand Create(Action action)
        {
            return new Command(
            execute: () =>
            {
                action();
            },
            canExecute: () =>
            {
                return true;
            });
        }

        public ICommand Create(Action action, Func<bool> canExecute)
        {
            return new Command(
            execute: () =>
            {
                action();
            },
            canExecute: () =>
            {
                return canExecute();
            });
        }

        public ICommand Create(Action<object> action, Predicate<object> canExecute)
        {
            return new Command(
            execute: parameter =>
            {
                action(parameter);
            },
            canExecute: parameter =>
            {
                return canExecute(parameter);
            });
        }

        public ICommand CreateAsync(ActionAsync action)
        {
            return new Command(
            execute: async () =>
            {
                await action();
            },
            canExecute: () =>
            {
                return true;
            });
        }

        public ICommand CreateAsync(ActionAsync action, Func<bool> canExecute)
        {
            return new Command(
            execute: async () =>
            {
                await action();
            },
            canExecute: () =>
            {
                return canExecute();

            });
        }

        public ICommand CreateAsync(ActionAsync<object> action, Predicate<object> canExecute)
        {
            return new Command(
            execute: async parameter =>
            {
                await action(parameter);
            },
            canExecute: parameter =>
            {
                return canExecute(parameter);

            });
        }
    }
}
