using AsyncAwaitBestPractices.MVVM;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VirtualDebts.Binding;
using VirtualDebts.Models;
using VirtualDebts.Resources.Strings;
using VirtualDebts.Services;
using VirtualDebts.UseCases;
using VirtualDebts.ViewModels;

using IDispatcher = VirtualDebts.Binding.IDispatcher;

namespace VirtualDebts.Controllers;

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

        this.AddUserCommand = this.commandFactory.CreateAsync(this.OnAddUser, ShouldEnableAddUserButton);
        this.RemoveUserCommand = this.commandFactory.CreateAsync(this.OnRemoveUser, ShouldEnableRemoveUserButton);
        this.ViewLoadedCommand = this.commandFactory.Create(this.OnViewLoaded);

        this.store.StateChanged += () => this.dispatcher?.InvokeInMainThread(this.UpdateProperties);
    }

    static private string CastToString(object obj)
    {
        return obj as string ?? throw new ArgumentException($"{nameof(obj)} must be of type string");
    }

    static private UserIdentity CastToUserIdentity(object obj)
    {
        return obj as UserIdentity? ?? throw new ArgumentException($"{nameof(obj)} must be of type {nameof(UserIdentity)}");
    }

    static public bool ShouldEnableAddUserButton(object? userToAdd)
    {
        if (userToAdd is null)
            return false;

        string userName = CastToString(userToAdd);
        return !string.IsNullOrWhiteSpace(userName);
    }

    static public bool ShouldEnableRemoveUserButton(object? userToRemove)
    {
        if (userToRemove is null)
            return false;

        string userName = CastToUserIdentity(userToRemove).Name;
        return !string.IsNullOrEmpty(userName);
    }

    public async Task OnAddUser(object? parameter)
    {
        string userToAdd = CastToString(parameter!);
        await interactor.AddUser(userToAdd);
    }

    public async Task OnRemoveUser(object? parameter)
    {
        UserIdentity userToRemove = CastToUserIdentity(parameter!);
        if (!this.ViewModel.Users.Contains(userToRemove))
            throw new ArgumentOutOfRangeException($"List of users does not contain user \"{userToRemove}\"");
        await this.interactor.RemoveUser(userToRemove);
    }

    public void OnViewLoaded() => this.dispatcher?.InvokeInMainThread(this.UpdateProperties);

    private void UpdateProperties()
    {
        var users = this.store.GetState().Users;
        this.ViewModel.Users = users.Select(user => user.GetIdentity()).ToList();
        this.ViewModel.UserNamesAsString = this.ViewModel.Users.Count > 0
                                       ? string.Join("\n", this.ViewModel.Users.Select(user => user.Name))
                                       : AppResources.EditUsers_UserListEmpty;
        this.NotifyPropertyChanged(null);
    }
}
