using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        public async Task AddUser_succeeds_for_valid_user_name()
        {
            // Given
            string userName = "TestUser";

            // When
            await this.givenInstance.AddUser(userName);

            // Then
            this.ThenUsersShouldContain(userName);
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }
        #endregion

        #region Then
        private void ThenUsersShouldContain(string userName)
        {
            int foundUserIndex = this.givenFixture.Store.GetState().Users.FindIndex(user => user.Name == userName);
            foundUserIndex.Should().BeGreaterThanOrEqualTo(0);
        }
        #endregion

        #region Given
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