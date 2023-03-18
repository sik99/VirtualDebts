using AsyncAwaitBestPractices.MVVM;
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

        public IAsyncCommand<object> AddUserCommand { get; }
        public IAsyncCommand<object> RemoveUserCommand { get; }
        public ICommand ViewLoadedCommand { get; }

        private readonly IEditUsersInteractor interactor;
        private readonly Store<AppState> store;
        private readonly ICommandFactory commandFactory;

        public EditUsersController(
            IEditUsersInteractor interactor,
            Store<AppState> store,
            ICommandFactory commandFactory,
            IDispatcher dispatcher)
            : base(dispatcher)
        {
            this.interactor = interactor ?? throw new ArgumentNullException(nameof(interactor));
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));

            this.AddUserCommand = this.commandFactory.CreateAsync(this.OnAddUser, this.ShouldEnableAddUserButton);
            this.RemoveUserCommand = this.commandFactory.CreateAsync(this.OnRemoveUser, this.ShouldEnableRemoveUserButton);
            this.ViewLoadedCommand = this.commandFactory.Create(this.OnViewLoaded);

            this.store.StateChanged += () => this.dispatcher?.InvokeInMainThread(this.UpdateProperties);
        }

        private string CastToString(object obj) => obj as string;
        private UserIdentity? CastToUserIdentity(object obj) => obj as UserIdentity?;

        public bool ShouldEnableAddUserButton(object userToAdd) => !string.IsNullOrWhiteSpace(CastToString(userToAdd));
        public bool ShouldEnableRemoveUserButton(object userToRemove) => !string.IsNullOrEmpty(CastToUserIdentity(userToRemove)?.Name);

        public async Task OnAddUser(object parameter)
        {
            string userToAdd = CastToString(parameter) ?? throw new ArgumentNullException(nameof(userToAdd));
            await interactor.AddUser(userToAdd);
        }

        public async Task OnRemoveUser(object parameter)
        {
            UserIdentity userToRemove = CastToUserIdentity(parameter) ?? throw new ArgumentNullException(nameof(userToRemove));
            if (!this.ViewModel.Users.Contains(userToRemove))
                throw new ArgumentOutOfRangeException($"List of users does not contain user \"{userToRemove}\"");
            await this.interactor.RemoveUser(userToRemove.Name);
        }

        public void OnViewLoaded() => this.dispatcher?.InvokeInMainThread(this.UpdateProperties);

        private void UpdateProperties()
        {
            var users = this.store.GetState().Users;
            this.ViewModel.Users = users.Select(user => user.GetIdentity()).ToList();
            this.ViewModel.UserNamesAsString = this.ViewModel.Users.Count > 0
                                           ? string.Join("\n", this.ViewModel.Users.Select(user => user.Name))
                                           : Properties.Resources.EditUsers_UserListEmpty;
            this.NotifyPropertyChanged(null);
        }
    }
}
