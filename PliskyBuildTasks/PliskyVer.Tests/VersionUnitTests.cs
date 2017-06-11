using System;
using Plisky.Helpers;
using Plisky.Build;
using Plisky.Plumbing;
using Xunit;

namespace PliskyVer.Tests {
    
    public class VersionUnitTests {
        UnitTestHelper uth = new UnitTestHelper();

        [Fact]
        public void DefaultBehaviour_IsFixed() {
            var vu = new VersionUnit("2", ".");
            Assert.Equal(VersionIncrementBehaviour.Fixed, vu.Behaviour);
        }

        [Fact]
        public void SetBehaviour_OnConstructor_Works() {
            var vu = new VersionUnit("2", ".", VersionIncrementBehaviour.AutoIncrementWithReset);
            Assert.Equal(VersionIncrementBehaviour.AutoIncrementWithReset, vu.Behaviour);
        }

        [Fact]
        public void VersionUnit_ChangeBehavour_ExceptionIfNotIncrementable() {
            Bilge.Log(nameof(VersionUnit_ChangeBehavour_ExceptionIfNotIncrementable) + " Entered");
            var sut = new VersionUnit("monkey");


            Assert.Throws<InvalidOperationException>(() => { sut.SetBehaviour(VersionIncrementBehaviour.AutoIncrementWithReset); });
        }

        [Fact]
        public void ChangeBehaviour_OnceSet_Exception() {
            var sut = new VersionUnit("1");
            sut.SetBehaviour(VersionIncrementBehaviour.AutoIncrementWithReset);

            Assert.Throws<InvalidOperationException>(() => { sut.Value = "Bannana"; });
        }

        [Fact]
        public void VersionUnit_FirstParamIsTheDigit() {
            var sut = new VersionUnit("1");
            Assert.Equal("1", sut.Value); //, "The value isnt set in the constructor");
        }

        [Fact]
        public void VersionUnit_DefaultPostfix_IsCorrect() {
            var sut = new VersionUnit("1");
            Assert.Equal("", sut.PreFix); //, "The default prefix is blank");
        }

        [Fact]
        public void VersionUnit_SecondParameterChangesPostfix() {
            var sut = new VersionUnit("1", "X");
            Assert.Equal("X", sut.PreFix); //, "The prefix needs to be set by the constructor");
        }

        [Fact]
        public void Prefix_IsPrefixWhenSpecified() {
            var sut = new VersionUnit("5", "Monkey");
            Assert.Equal("Monkey5", sut.ToString()); //, "The prefix was not correctly specified in the ToSTring method");
        }
        [Fact]
        public void VersionUnit_DefaultsToIncrementWithNumber() {
            var sut = new VersionUnit("1");
            Assert.Equal("1", sut.Value); //, "The value should default correctly");
            Assert.Equal(VersionIncrementBehaviour.Fixed, sut.Behaviour);
        }

        [Fact]
        public void VersionUnit_NonDigit_WorksFine() {
            var sut = new VersionUnit("monkey");
            Assert.Equal("monkey", sut.Value); //, "The value was not set correctly");
        }

    }
}
