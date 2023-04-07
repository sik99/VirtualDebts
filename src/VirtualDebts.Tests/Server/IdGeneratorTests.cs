using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VirtualDebts.Server;

[TestClass]
public class IdGeneratorTests
{
    private readonly IdGenerator givenInstance = new IdGenerator();

    #region Next function tests
    [TestMethod]
    public void Next_generates_non_empty_id()
    {
        // When
        var id = givenInstance.Next();

        // Then
        id.Should().NotBeEmpty();
    }

    [TestMethod]
    public void Next_generates_unique_ids()
    {
        // When
        var first = givenInstance.Next();
        var second = givenInstance.Next();

        // Then
        second.Should().NotBe(first);
    }
    #endregion
}