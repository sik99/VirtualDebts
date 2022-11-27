using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
                new XamarinCommandFactory(),
                this.givenFixture.Dispatcher);
        }

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
        public void OnAddUser_is_disabled_for_empty_user_name()
        {
            // Given
            string userName = "";

            // When
            bool isAddUserEnabled = this.givenInstance.AddUserCommand.CanExecute(userName);

            // Then
            isAddUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnAddUser_is_disabled_for_whitespace_user_name()
        {
            // Given
            string userName = "     ";

            // When
            bool isAddUserEnabled = this.givenInstance.AddUserCommand.CanExecute(userName);

            // Then
            isAddUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnAddUser_is_enabled_for_user_name_with_visible_characters()
        {
            // Given
            string userName = "  visible   ";

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
            string userName = "Existing user name";
            this.GivenUsers(userName);

            // When
            this.givenInstance.RemoveUserCommand.Execute(userName);

            // Then
            this.givenFixture.EditUsersInteractorMock.Verify(mock => mock.RemoveUser(userName), Times.Exactly(1));
            this.givenFixture.EditUsersInteractorMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void OnRemoveUser_is_disabled_for_empty_user_name()
        {
            // Given
            string userName = "";

            // When
            bool isRemoveUserEnabled = this.givenInstance.RemoveUserCommand.CanExecute(userName);

            // Then
            isRemoveUserEnabled.Should().BeFalse();
        }

        [TestMethod]
        public void OnRemoveUser_is_enabled_for_nonempty_user_name()
        {
            // Given
            string userName = "non-empty";

            // When
            bool isRemoveUserEnabled = this.givenInstance.RemoveUserCommand.CanExecute(userName);

            // Then
            isRemoveUserEnabled.Should().BeTrue();
        }
        #endregion

        #region Given
        private void GivenUsers(params string[] userNames)
        {
            var users = userNames
                .Select(userName => new User(userName))
                .ToList();
            bool isSuccess = this.givenFixture.Store.Update((ref AppState appState) =>
            {
                appState.Users = users;
                return true;
            });

            isSuccess.Should().BeTrue();
            this.givenFixture.Store.GetState().Users.Count.Should().Be(userNames.Length);
            this.givenInstance.ViewModel.UserList.Should().BeEquivalentTo(userNames);
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