using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plisky.Helpers;
using Plisky.Build;
using Plisky.Plumbing;

namespace PliskyVer.Tests {
    [TestClass]
    public class VersionUnitTests {
        UnitTestHelper uth = new UnitTestHelper();

        [TestMethod]
        public void DefaultBehaviour_IsFixed() {
            var vu = new VersionUnit("2", ".");            
            Assert.AreEqual(VersionIncrementBehaviour.Fixed, vu.Behaviour);
        }

        [TestMethod]
        public void SetBehaviour_OnConstructor_Works() {
            var vu = new VersionUnit("2", ".", VersionIncrementBehaviour.AutoIncrementWithReset);
            Assert.AreEqual(VersionIncrementBehaviour.AutoIncrementWithReset, vu.Behaviour);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VersionUnit_ChangeBehavour_ExceptionIfNotIncrementable() {
            Bilge.Log(nameof(VersionUnit_ChangeBehavour_ExceptionIfNotIncrementable) + " Entered");
            var sut = new VersionUnit("monkey");
            sut.SetBehaviour(VersionIncrementBehaviour.AutoIncrementWithReset);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ChangeBehaviour_OnceSet_Exception() {
            var sut = new VersionUnit("1");
            sut.SetBehaviour(VersionIncrementBehaviour.AutoIncrementWithReset);
            sut.Value = "Bannana";
        }

        [TestMethod]
        public void VersionUnit_FirstParamIsTheDigit() {
            var sut = new VersionUnit("1");
            Assert.AreEqual(sut.Value, "1", "The value isnt set in the constructor");
        }

        [TestMethod]
        public void VersionUnit_DefaultPostfix_IsCorrect() {
            var sut = new VersionUnit("1");
            Assert.AreEqual(sut.PreFix, "", "The default prefix is blank");
        }

        [TestMethod]
        public void VersionUnit_SecondParameterChangesPostfix() {
            var sut = new VersionUnit("1", "X");
            Assert.AreEqual(sut.PreFix, "X", "The prefix needs to be set by the constructor");
        }

        [TestMethod]
        public void Prefix_IsPrefixWhenSpecified() {
            var sut = new VersionUnit("5", "Monkey");
            Assert.AreEqual("Monkey5", sut.ToString(), "The prefix was not correctly specified in the ToSTring method");
        }
        [TestMethod]
        public void VersionUnit_DefaultsToIncrementWithNumber() {
            var sut = new VersionUnit("1");
            Assert.AreEqual(sut.Value, "1", "The value should default correctly");
            Assert.AreEqual(sut.Behaviour, VersionIncrementBehaviour.Fixed);
        }

        [TestMethod]
        public void VersionUnit_NonDigit_WorksFine() {
            var sut = new VersionUnit("monkey");
            Assert.AreEqual("monkey", sut.Value, "The value was not set correctly");
        }

    }
}
