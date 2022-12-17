using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualDebts.Services
{
    [TestClass]
    public class StoreTests
    {
        private readonly Store<TestState> givenInstance = new Store<TestState>();

        #region Update function tests
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Update_return_value_reflects_if_transformation_succeeded(bool isTransformSuccessful)
        {
            // When
            bool result = this.givenInstance.Update(state => isTransformSuccessful);

            // Then
            result.Should().Be(isTransformSuccessful);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Update_triggers_StateChanged_event(bool isTransformSuccessful)
        {
            // Given
            bool isStateChangedTriggered = false;
            this.givenInstance.StateChanged += () => { isStateChangedTriggered = true; };

            // When
            this.givenInstance.Update(_ => isTransformSuccessful);

            // Then
            isStateChangedTriggered.Should().BeTrue();
        }

        [TestMethod]
        public void Update_applies_transformation_to_state_members()
        {
            // When
            this.givenInstance.Update(state =>
            {
                state.ValueObject = 5;
                state.ReferenceObject = new DataClass("Monday");
                return true;
            });

            // Then
            this.givenInstance.GetState().ShouldBe(5, "Monday");
        }

        [TestMethod]
        public void Update_applies_transformation_to_single_state_member()
        {
            // Given
            this.GivenState(5, "Monday");

            // When
            this.givenInstance.Update(state =>
            {
                state.ValueObject = 12;
                return true;
            });

            // Then
            this.givenInstance.GetState().ShouldBe(12, "Monday");

        }
        #endregion

        #region GetState function tests
        [TestMethod]
        public void GetState_returns_deep_copy_of_state()
        {
            // Given
            this.GivenState(5, "Monday");
            var actualState = this.givenInstance.GetState();

            // When
            this.givenInstance.Update(state =>
            {
                state.ValueObject = 12;
                state.ReferenceObject = new DataClass("Wednesday");
                return true;
            });

            // Then
            actualState.ShouldBe(5, "Monday");
        }
        #endregion

        #region Given
        void GivenState(int valueObject, string referencedContent)
        {
            this.givenInstance.Update(state =>
            {
                state.ValueObject = valueObject;
                state.ReferenceObject = new DataClass(referencedContent);
                return true;
            });
        }
        #endregion
    }

    #region TestState
    internal class TestState : ICloneable
    {
        public int ValueObject { get; set; }
        public DataClass ReferenceObject { get; set; }

        public object Clone()
        {
            return new TestState
            {
                ValueObject = this.ValueObject,
                ReferenceObject = new DataClass(this.ReferenceObject.DataString)
            };
        }
    }

    internal static class TestStateExtensions
    {
        internal static void ShouldBe(this TestState actual, int expectedValueObject, string expectedReferencedContent)
        {
            using (new AssertionScope())
            {
                actual.ValueObject.Should().Be(expectedValueObject);
                actual.ReferenceObject.Should().Be(new DataClass(expectedReferencedContent));
            }
        }
    }

    internal class DataClass
    {
        public string DataString { get; set; }
        public List<int> AsciiCodes { get; set; }

        public DataClass(string data)
        {
            this.DataString = data;
            this.AsciiCodes = data.ToCharArray().Select(c => (int)c).ToList();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DataClass other))
                return false;

            if (!this.AsciiCodes.SequenceEqual(other.AsciiCodes))
                return false;

            if (!this.DataString.Equals(other.DataString))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.DataString, this.AsciiCodes).GetHashCode();
        }
    }
    #endregion
}