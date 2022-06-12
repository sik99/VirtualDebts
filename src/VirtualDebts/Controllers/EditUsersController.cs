using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VirtualDebts.Binding;
using VirtualDebts.Models;
using VirtualDebts.Services;
using VirtualDebts.UseCases;
using VirtualDebts.ViewModels;

namespace VirtualDebts.Controllers
{
    public class EditUsersController : ControllerBase
    {
        public EditUsersViewModel ViewModel { get; private set; } = new EditUsersViewModel();

        public ICommand AddUserCommand { get; }
        public ICommand RemoveUserCommand { get; }
        public ICommand ViewLoadedCommand { get; }

        private readonly EditUsersInteractor interactor;
        private readonly Store<AppState> store;
        private readonly ICommandFactory commandFactory;

        public EditUsersController(EditUsersInteractor interactor, Store<AppState> store, ICommandFactory commandFactory, IDispatcher dispatcher) : base(dispatcher)
        {
            this.interactor = interactor ?? throw new ArgumentNullException(nameof(interactor));
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));

            this.AddUserCommand = this.commandFactory.CreateAsync(this.OnAddUser, this.ShouldEnableAddUserButton);
            this.RemoveUserCommand = this.commandFactory.CreateAsync(this.OnRemoveUser, this.ShouldEnableRemoveUserButton);
            this.ViewLoadedCommand = this.commandFactory.Create(this.OnViewLoaded);

            this.store.StateChanged += () => this.dispatcher?.InvokeInMainThread(this.UpdateProperties);
        }

        private string CastToString(object obj) => (obj is string objString) ? objString : null;

        public bool ShouldEnableAddUserButton(object userToAdd) => !string.IsNullOrWhiteSpace(CastToString(userToAdd));
        public bool ShouldEnableRemoveUserButton(object userToAdd) => !string.IsNullOrEmpty(CastToString(userToAdd));

        public async Task OnAddUser(object parameter)
        {
            string userToAdd = CastToString(parameter) ?? throw new ArgumentNullException(nameof(userToAdd));
            await interactor.AddUser(userToAdd);
        }

        public async Task OnRemoveUser(object parameter)
        {
            string userToRemove = CastToString(parameter) ?? throw new ArgumentNullException(nameof(userToRemove));
            if (!this.ViewModel.UserList.Contains(userToRemove))
                throw new ArgumentOutOfRangeException($"List of users does not contain user \"{userToRemove}\"");
            await this.interactor.RemoveUser(userToRemove);
        }

        public void OnViewLoaded() => this.dispatcher?.InvokeInMainThread(this.UpdateProperties);

        private void UpdateProperties()
        {
            var users = this.store.GetState().Users;
            this.ViewModel.UserList = users.Select(user => user.Name).ToList();
            this.ViewModel.UserListAsString = this.ViewModel.UserList.Count > 0
                                           ? string.Join("\n", this.ViewModel.UserList)
                                           : Properties.Resources.EditUsers_UserListEmpty;
            this.NotifyPropertyChanged(null);
        }
    }
}
