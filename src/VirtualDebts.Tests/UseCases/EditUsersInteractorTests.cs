using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using VirtualDebts.Binding;
using VirtualDebts.Models;
using VirtualDebts.Services;

namespace VirtualDebts.UseCases
{
    [TestClass]
    public class EditUsersInteractorTests
    {
        private EditUsersInteractor givenInstance;
        private readonly GivenFixture givenFixture = new GivenFixture();

        [TestInitialize]
        public void TestInitialize()
        {
            this.givenInstance = new EditUsersInteractor(
                this.givenFixture.Store,
                this.givenFixture.NavigationServiceMock.Object);
        }

        #region AddUser function tests
        [TestMethod]
        public async Task AddUser_adds_a_first_user()
        {
            // Given
            string userName = "Test user";

            // When
            await this.givenInstance.AddUser(userName);

            // Then
            this.ThenUsersShouldContain(userName);
            this.ThenUsersCountShouldBe(1);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task AddUser_adds_a_further_user()
        {
            // Given
            string user1 = "User 1";
            this.GivenUsers(user1);
            string user2 = "User 2";

            // When
            await this.givenInstance.AddUser(user2);

            // Then
            this.ThenUsersShouldContain(user1);
            this.ThenUsersShouldContain(user2);
            this.ThenUsersCountShouldBe(2);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task AddUser_trims_user_name()
        {
            // Given
            string userName = " Test user ";

            // When
            await this.givenInstance.AddUser(userName);

            // Then
            this.ThenUsersShouldContain("Test user");
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task AddUser_shows_message_box_for_duplicate_user_name()
        {
            // Given
            string userName = "Test user";
            this.GivenUsers(userName);

            // When
            await this.givenInstance.AddUser(userName);

            // Then
            this.ThenUsersCountShouldBe(1);
            this.ThenAddUserFailedMessagePopsUp(string.Format(Properties.Resources.EditUsers_AddExistentMsg, userName));
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task AddUser_shows_message_box_for_empty_user_name()
        {
            // Given
            string userName = "";

            // When
            await this.givenInstance.AddUser(userName);

            // Then
            this.ThenUsersShouldBeEmpty();
            this.ThenAddUserFailedMessagePopsUp(Properties.Resources.EditUsers_AddWhitespaceMsg);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }
        #endregion

        #region Then
        private void ThenUsersShouldContain(string userName)
        {
            int foundUserIndex = this.givenFixture.Store.GetState().Users.FindIndex(user => user.Name == userName);
            foundUserIndex.Should().BeGreaterThanOrEqualTo(0);
        }

        private void ThenUsersShouldBeEmpty()
        {
            this.givenFixture.Store.GetState().Users.Should().BeEmpty();
        }

        private void ThenUsersCountShouldBe(int usersCount)
        {
            this.givenFixture.Store.GetState().Users.Should().HaveCount(usersCount);
        }

        private void ThenAddUserFailedMessagePopsUp(string message)
        {
            this.givenFixture.NavigationServiceMock.Verify(
                mock => mock.ShowMessageBox(Properties.Resources.EditUsers_AddFailedMsg, message),
                Times.Exactly(1));
        }
        #endregion

        #region Given
        private void GivenUsers(params string[] userNames)
        {
            var users = userNames
                .Select(userName => new User(userName))
                .ToList();
            bool isSuccess = this.givenFixture.Store.Update(appState =>
            {
                appState.Users = users;
                return true;
            });

            isSuccess.Should().BeTrue();
            this.givenFixture.Store.GetState().Users.Count.Should().Be(userNames.Length);
        }

        internal class GivenFixture
        {
            public readonly Mock<INavigationService> NavigationServiceMock = new Mock<INavigationService>(MockBehavior.Strict);
            public Store<AppState> Store = new Store<AppState>();

            public GivenFixture()
            {
                this.NavigationServiceMock
                    .Setup(mock => mock.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
            }
        }
        #endregion
    }
}