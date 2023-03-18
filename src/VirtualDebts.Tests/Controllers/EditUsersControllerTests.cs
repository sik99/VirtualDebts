using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VirtualDebts.Binding;
using VirtualDebts.Models;
using VirtualDebts.Services;
using VirtualDebts.UseCases;

namespace VirtualDebts.Controllers
{
    [TestClass]
    public class EditUsersControllerTests
    {
        private EditUsersController givenInstance;
        private readonly GivenFixture givenFixture = new GivenFixture();

        [TestInitialize]
        public void TestInitialize()
        {
            this.givenInstance = new EditUsersController(
                this.givenFixture.EditUsersInteractorMock.Object,
                this.givenFixture.Store,
                new CommandFactory(),
                this.givenFixture.Dispatcher);
        }

        #region View model tests
        [TestMethod]
        public void ViewLoaded_causes_view_model_to_update()
        {
            // We cannot easily set value to Store without notifying view model of the change.
            // Therefore we only test the notification that follows every view model change.

            // Given
            bool wasViewModelUpdated = false;
            this.givenInstance.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { wasViewModelUpdated = true; };

            // When
            this.givenInstance.ViewLoadedCommand.Execute(null);

            // Then
            wasViewModelUpdated.Should().BeTrue();
        }

        [TestMethod]
        public void Store_update_causes_view_model_to_update()
        {
            // Given
            bool wasViewModelUpdated = false;
            this.givenInstance.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { wasViewModelUpdated = true; };

            ImmutableList<string> userNames = new List<string> { "Alice", "Bob", "Cecilia" }.ToImmutableList();

            // When
            this.givenFixture.Store.Update(appState =>
            {
                var newState = CreateAppState(userNames);
                appState.Users = newState.Users;
                return true;
            });

            // Then
            wasViewModelUpdated.Should().BeTrue();
            this.givenInstance.ViewModel.Users.Select(user => user.Name).Should().BeEquivalentTo(userNames);
            this.givenInstance.ViewModel.UserNamesAsString.Should().Be("Alice\nBob\nCecilia");
        }
        #endregion

        #region OnAddUser tests
        [TestMethod]
        public void OnAddUser_calls_interactor()
        {
            // Given
            string userName = "User name";

            // When
            this.givenInstance.AddUserCommand.Execute(userName);

            // Then
            this.givenFixture.EditUsersInteractorMock.Verify(mock => mock.AddUser(userName), Times.Exactly(1));
            this.givenFixture.EditUsersInteractorMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void OnAddUser_is_disabled_for_null()
        {
            // Given
            object userName = null;

            // When
            bool isAddUserEnabled = this.givenInstance.AddUserCommand.CanExecute(userName);

            // Then
            isAddUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnAddUser_is_disabled_for_empty_user_name()
        {
            // Given
            object userName = "";

            // When
            bool isAddUserEnabled = this.givenInstance.AddUserCommand.CanExecute(userName);

            // Then
            isAddUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnAddUser_is_disabled_for_whitespace_user_name()
        {
            // Given
            object userName = "     ";

            // When
            bool isAddUserEnabled = this.givenInstance.AddUserCommand.CanExecute(userName);

            // Then
            isAddUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnAddUser_is_enabled_for_user_name_with_visible_characters()
        {
            // Given
            object userName = "  visible   ";

            // When
            bool isAddUserEnabled = this.givenInstance.AddUserCommand.CanExecute(userName);

            // Then
            isAddUserEnabled.Should().BeTrue();
        }
        #endregion

        #region OnRemoveUser tests
        [TestMethod]
        public void OnRemoveUser_calls_interactor_when_user_name_exists()
        {
            // Given
            UserIdentity userIdentity = new UserIdentity(Guid.NewGuid(), "Existing user name");
            this.GivenUsers(userIdentity);

            // When
            this.givenInstance.RemoveUserCommand.Execute(userIdentity);

            // Then
            this.givenFixture.EditUsersInteractorMock.Verify(mock => mock.RemoveUser(userIdentity.Name), Times.Exactly(1));
            this.givenFixture.EditUsersInteractorMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task OnRemoveUser_throws_when_user_name_does_not_exist()
        {
            // Given
            UserIdentity userIdentity = new UserIdentity(Guid.NewGuid(), "Non-existing user name");

            // When
            Func<Task> action = () => this.givenInstance.RemoveUserCommand.ExecuteAsync(userIdentity);

            // Then
            await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void OnRemoveUser_is_disabled_for_null()
        {
            // Given
            object user = null;

            // When
            bool isRemoveUserEnabled = this.givenInstance.RemoveUserCommand.CanExecute(user);

            // Then
            isRemoveUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnRemoveUser_is_disabled_for_empty_user_name()
        {
            // Given
            object user = new UserIdentity(Guid.Empty, "");

            // When
            bool isRemoveUserEnabled = this.givenInstance.RemoveUserCommand.CanExecute(user);

            // Then
            isRemoveUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnRemoveUser_is_enabled_for_nonempty_user_name()
        {
            // Given
            object user = new UserIdentity(Guid.Empty, "non-empty");

            // When
            bool isRemoveUserEnabled = this.givenInstance.RemoveUserCommand.CanExecute(user);

            // Then
            isRemoveUserEnabled.Should().BeTrue();
        }
        #endregion

        #region Given
        private void GivenUsers(params UserIdentity[] userIdentities)
        {
            var users = userIdentities
                .Select(identity => new User(identity.Id, identity.Name))
                .ToList();
            bool isSuccess = this.givenFixture.Store.Update(appState =>
            {
                appState.Users = users;
                return true;
            });

            isSuccess.Should().BeTrue();
            this.givenFixture.Store.GetState().Users.Count.Should().Be(userIdentities.Length);
            this.givenInstance.ViewModel.Users.Should().BeEquivalentTo(userIdentities);
        }

        private AppState CreateAppState(IList<string> userNames)
        {
            var users = userNames
                .Select(userName => new User(Guid.NewGuid(), userName))
                .ToList();
            return new AppState { Users = users };
        }

        internal class GivenFixture
        {
            public readonly Mock<IEditUsersInteractor> EditUsersInteractorMock = new Mock<IEditUsersInteractor>(MockBehavior.Strict);
            public readonly SynchronousDispatcher Dispatcher = new SynchronousDispatcher();
            public Store<AppState> Store = new Store<AppState>();

            public GivenFixture()
            {
                this.EditUsersInteractorMock
                    .Setup(mock => mock.AddUser(It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
                this.EditUsersInteractorMock
                    .Setup(mock => mock.RemoveUser(It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
            }
        }
        #endregion
    }
}