using System;
using System.Linq;
using System.Threading.Tasks;
using VirtualDebts.Binding;
using VirtualDebts.Models;
using VirtualDebts.Services;

namespace VirtualDebts.UseCases
{
    public class EditUsersInteractor : IEditUsersInteractor
    {
        private readonly Store<AppState> store;
        private readonly INavigationService navigationService;

        public EditUsersInteractor(Store<AppState> store, INavigationService navigationService)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public async Task AddUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                string message = Properties.Resources.EditUsers_AddWhitespaceMsg;
                await this.navigationService.ShowMessageBox(Properties.Resources.EditUsers_AddFailedMsg, message);
                return;
            }

            userName = userName.Trim();
            bool isSuccess = this.store.Update(appState =>
            {
                bool isUserExistent = appState.Users.FindIndex(user => user.Name == userName) >= 0;
                if (isUserExistent)
                    return false;

                appState.Users.Add(new User(userName));
                return true;
            });

            if (!isSuccess)
            {
                string message = string.Format(Properties.Resources.EditUsers_AddExistentMsg, userName);
                await this.navigationService.ShowMessageBox(Properties.Resources.EditUsers_AddFailedMsg, message);
            }
        }

        // TODO RemoveUser using Guid instead of name
        public async Task RemoveUser(string userName)
        {
            bool isSuccess = this.store.Update(appState =>
            {
                var userIndex = appState.Users.FindIndex(user => user.Name == userName);
                if (userIndex < 0)
                    return true; // user is already removed

                var user = appState.Users.ElementAt(userIndex);
                if (user.Balance != 0)
                    return false;

                bool isRemoved = appState.Users.Remove(user);
                if (!isRemoved)
                    throw new ArgumentOutOfRangeException($"User \"{userName}\" couldn't be removed");
                return true;
            });

            if (!isSuccess)
            {
                string message = string.Format(Properties.Resources.EditUsers_RemoveDebtorMsg, userName);
                await this.navigationService.ShowMessageBox(Properties.Resources.EditUsers_RemoveFailedMsg, message);
            }
        }
    }
}
