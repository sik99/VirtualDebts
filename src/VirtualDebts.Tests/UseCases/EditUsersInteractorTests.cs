using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
                this.givenFixture.NavigationServiceMock.Object,
                this.givenFixture.UserIdGeneratorMock.Object);
        }

        #region AddUser function tests
        [TestMethod]
        public async Task AddUser_adds_a_first_user()
        {
            // Given
            string userName = "Test user";
            Guid userId = Guid.NewGuid();
            GivenNextAddedUserId(userId);

            // When
            await this.givenInstance.AddUser(userName);

            // Then
            this.ThenUsersShouldContain(new UserIdentity(userId, userName));
            this.ThenUsersCountShouldBe(1);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task AddUser_adds_a_further_user()
        {
            // Given
            UserIdentity user1 = new UserIdentity(Guid.NewGuid(), "User 1");
            this.GivenUsers(user1);

            string user2Name = "User 2";
            Guid user2Id = Guid.NewGuid();
            GivenNextAddedUserId(user2Id);

            // When
            await this.givenInstance.AddUser(user2Name);

            // Then
            this.ThenUsersShouldContain(user1);
            this.ThenUsersShouldContain(new UserIdentity(user2Id, user2Name));
            this.ThenUsersCountShouldBe(2);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task AddUser_trims_user_name()
        {
            // Given
            string userName = "   Test user  ";
            Guid userId = Guid.NewGuid();
            GivenNextAddedUserId(userId);

            // When
            await this.givenInstance.AddUser(userName);

            // Then
            this.ThenUsersShouldContain(new UserIdentity(userId, "Test user"));
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task AddUser_shows_message_box_for_duplicate_user_name()
        {
            // Given
            UserIdentity userIdentity = new UserIdentity(Guid.NewGuid(), "Test user");
            this.GivenUsers(userIdentity);

            // When
            await this.givenInstance.AddUser(userIdentity.Name);

            // Then
            this.ThenUsersCountShouldBe(1);
            this.ThenAddUserFailedMessagePopsUp(string.Format(Properties.Resources.EditUsers_AddExistentMsg, userIdentity.Name));
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

        #region RemoveUser function tests
        [TestMethod()]
        public async Task RemoveUser_removes_user_with_zero_balance()
        {
            // Given
            UserIdentity userIdentity = new UserIdentity(Guid.NewGuid(), "Test user");
            this.GivenUsersWithDebts((userIdentity, 0));

            // When
            await this.givenInstance.RemoveUser(userIdentity);

            // Then
            this.ThenUsersShouldBeEmpty();
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        public async Task RemoveUser_shows_message_box_when_user_balance_is_nonzero()
        {
            // Given
            UserIdentity userIdentity = new UserIdentity(Guid.NewGuid(), "Test user");
            this.GivenUsersWithDebts((userIdentity, 100));

            // When
            await this.givenInstance.RemoveUser(userIdentity);

            // Then
            this.ThenUsersCountShouldBe(1);
            this.ThenUsersShouldContain(userIdentity);
            this.ThenRemoveUserFailedMessagePopsUp(string.Format(Properties.Resources.EditUsers_RemoveDebtorMsg, userIdentity.Name));
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        public async Task RemoveUser_does_nothing_for_nonexisting_user()
        {
            // Given
            UserIdentity realUser = new UserIdentity(Guid.NewGuid(), "Real user");
            this.GivenUsers(realUser);
            UserIdentity nonexistingUser = new UserIdentity(Guid.NewGuid(), "Non-existing user");

            // When
            await this.givenInstance.RemoveUser(nonexistingUser);

            // Then
            this.ThenUsersCountShouldBe(1);
            this.ThenUsersShouldContain(realUser);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod()]
        public async Task RemoveUser_removes_user_by_id()
        {
            // Given
            string sameName = "Same name";
            UserIdentity user1Identity = new UserIdentity(Guid.NewGuid(), sameName);
            UserIdentity user2Identity = new UserIdentity(Guid.NewGuid(), sameName);
            UserIdentity user3Identity = new UserIdentity(Guid.NewGuid(), sameName);
            this.GivenUsers(user1Identity, user2Identity, user3Identity);

            // When
            await this.givenInstance.RemoveUser(user2Identity);

            // Then
            this.ThenUsersCountShouldBe(2);
            this.ThenUsersShouldContain(user1Identity);
            this.ThenUsersShouldContain(user3Identity);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }
        #endregion

        #region Then
        private void ThenUsersShouldContain(UserIdentity userIdentity)
        {
            var storeUsers = this.givenFixture.Store.GetState().Users;
            int foundUserIndex = storeUsers.FindIndex(user => user.Id == userIdentity.Id);
            foundUserIndex.Should().BeGreaterThanOrEqualTo(0);
            storeUsers[foundUserIndex].GetIdentity().Should().Be(userIdentity);
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

        private void ThenRemoveUserFailedMessagePopsUp(string message)
        {
            this.givenFixture.NavigationServiceMock.Verify(
                mock => mock.ShowMessageBox(Properties.Resources.EditUsers_RemoveFailedMsg, message),
                Times.Exactly(1));
        }
        #endregion

        #region Given
        private void GivenUsersWithDebts(params (UserIdentity, int)[] usersAndDebts)
        {
            var users = usersAndDebts
                .Select(userAndDebt => new User(userAndDebt.Item1.Id, userAndDebt.Item1.Name, userAndDebt.Item2))
                .ToList();
            bool isSuccess = this.givenFixture.Store.Update(appState =>
            {
                appState.Users = users;
                return true;
            });

            isSuccess.Should().BeTrue();
            this.givenFixture.Store.GetState().Users.Count.Should().Be(usersAndDebts.Length);
        }

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
        }

        private void GivenNextAddedUserId(Guid userId)
        {
            this.givenFixture.UserIdGeneratorMock
                .Setup(mock => mock.Next())
                .Returns(userId);
        }

        internal class GivenFixture
        {
            public readonly Mock<INavigationService> NavigationServiceMock = new Mock<INavigationService>(MockBehavior.Strict);
            public readonly Mock<Server.IUserIdGenerator> UserIdGeneratorMock = new Mock<Server.IUserIdGenerator>(MockBehavior.Strict);
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