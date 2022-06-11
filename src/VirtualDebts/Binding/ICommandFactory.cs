using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VirtualDebts.Binding
{
    public delegate Task ActionAsync();
    public delegate Task ActionAsync<TParameter>(TParameter parameter);

    public interface ICommandFactory
    {
        public ICommand Create(Action action);
        public ICommand Create(Action action, Func<bool> canExecute);
        public ICommand Create(Action<object> action, Predicate<object> canExecute);
        public ICommand CreateAsync(ActionAsync action);
        public ICommand CreateAsync(ActionAsync action, Func<bool> canExecute);
        public ICommand CreateAsync(ActionAsync<object> action, Predicate<object> canExecute);
    }
}