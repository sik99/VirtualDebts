using System;
using System.Linq;
using System.Threading.Tasks;
using VirtualDebts.Binding;
using VirtualDebts.Models;
using VirtualDebts.Resources.Strings;
using VirtualDebts.Services;

namespace VirtualDebts.UseCases
{
    public class EditUsersInteractor : IEditUsersInteractor
    {
        private readonly Store<AppState> store;
        private readonly INavigationService navigationService;
        private readonly Server.IUserIdGenerator userIdGenerator;

        public EditUsersInteractor(Store<AppState> store, INavigationService navigationService, Server.IUserIdGenerator userIdGenerator)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            this.userIdGenerator = userIdGenerator ?? throw new ArgumentNullException(nameof(userIdGenerator));
        }

        public async Task AddUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                string message = AppResources.EditUsers_AddWhitespaceMsg;
                await this.navigationService.ShowMessageBox(AppResources.EditUsers_AddFailedMsg, message);
                return;
            }

            userName = userName.Trim();
            bool isSuccess = this.store.Update(appState =>
            {
                bool isUserExistent = appState.Users.FindIndex(user => user.Name == userName) >= 0;
                if (isUserExistent)
                    return false;

                var userId = this.userIdGenerator.Next();
                var user = new User(userId, userName);
                appState.Users.Add(user);
                return true;
            });

            if (!isSuccess)
            {
                string message = string.Format(AppResources.EditUsers_AddExistentMsg, userName);
                await this.navigationService.ShowMessageBox(AppResources.EditUsers_AddFailedMsg, message);
            }
        }

        public async Task RemoveUser(UserIdentity userIdentity)
        {
            bool isSuccess = this.store.Update(appState =>
            {
                var userIndex = appState.Users.FindIndex(user => user.Id == userIdentity.Id);
                if (userIndex < 0)
                    return true; // user is already removed

                var user = appState.Users.ElementAt(userIndex);
                if (user.Balance != 0)
                    return false;

                bool isRemoved = appState.Users.Remove(user);
                if (!isRemoved)
                    throw new ArgumentOutOfRangeException($"User \"{userIdentity.Name}\" couldn't be removed");
                return true;
            });

            if (!isSuccess)
            {
                string message = string.Format(AppResources.EditUsers_RemoveDebtorMsg, userIdentity.Name);
                await this.navigationService.ShowMessageBox(AppResources.EditUsers_RemoveFailedMsg, message);
            }
        }
    }
}
