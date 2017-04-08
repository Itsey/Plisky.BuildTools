using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plisky.Build;

namespace PliskyVer.Tests {
    [TestClass]
    public class CompleteVersionTests {

        [TestMethod]
        public void FixedBehaviour_DoesNotIncrement() {
            var sut = new CompleteVersion(new VersionUnit("2"), new VersionUnit("0", "."));
            string before = sut.GetVersionString(DisplayType.Full);
            sut.PerformIncrement();
            Assert.AreEqual(before, sut.GetVersionString(DisplayType.Full), "Digits should be fixed and not change when incremented");
        }

        [TestMethod]
        public void PartiallyFixed_DoesIncrement() {
            var sut = new CompleteVersion(new VersionUnit("1", "", VersionIncrementBehaviour.ContinualIncrement), new VersionUnit("Monkey", "."));
            sut.PerformIncrement();
            Assert.AreEqual("2.Monkey", sut.GetVersionString(DisplayType.Full), "The increment for the first digit did not work in a mixed verison number");
        }

        [TestMethod]
        public void Increment_ResetAnyWorks() {
            var sut = new CompleteVersion(
                new VersionUnit("1", "", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("0", "."),
                new VersionUnit("1", "."),
                new VersionUnit("0", ".", VersionIncrementBehaviour.AutoIncrementWithResetAny));
            sut.PerformIncrement();
            Assert.AreEqual("2.0.1.0", sut.GetVersionString(DisplayType.Full), "The reset should prevent the last digit from incrementing");
        }


        [TestMethod]
        public void DisplayType_DefaultsToFull() {
            var sut = new CompleteVersion(
               new VersionUnit("1", ""),
               new VersionUnit("0", "."),
               new VersionUnit("1", "."),
               new VersionUnit("0", ".", VersionIncrementBehaviour.AutoIncrementWithResetAny));
            Assert.AreEqual(sut.GetVersionString(DisplayType.Full), sut.GetVersionString(), "The default should be to display as full");
        }


        [TestMethod]
        public void Increment_ResentAnyWorks() {
            var vu2 = new VersionUnit("0", ".");
            var sut = new CompleteVersion(
               new VersionUnit("1", ""),
               vu2,
               new VersionUnit("1", "."),
               new VersionUnit("0", ".", VersionIncrementBehaviour.AutoIncrementWithResetAny));
            Assert.AreEqual("1.0.1.0", sut.GetVersionString(), "Without an increment its wrong - invalid test");

            sut.PerformIncrement();
            Assert.AreEqual("1.0.1.1", sut.GetVersionString(), "Increment on all fixed should not change anything");
            vu2.IncrementOverride = "5";
            sut.PerformIncrement();

            Assert.AreEqual("1.5.1.0", sut.GetVersionString(), "The final digit should reset when a fixed digit changes.");
        }

        [TestMethod]
        public void Increment_OverrideReplacesIncrement() {
            var vu = new VersionUnit("1", "", VersionIncrementBehaviour.ContinualIncrement);
            vu.IncrementOverride = "9";
            var sut = new CompleteVersion(vu);
            sut.PerformIncrement();
            Assert.AreEqual("9", sut.GetVersionString(DisplayType.Full), "The overide on a word value did not work");
        }

        [TestMethod]
        public void SimpleIncrement_Fixed_DoesNothing() {
            var sut = new CompleteVersion(new VersionUnit("1"), new VersionUnit("2", "."));
            sut.PerformIncrement();
            Assert.AreEqual("1.2", sut.ToString(), "The verison increment should do nothing for fixed");
        }

        [TestMethod]
        public void SimpleIncrement_Incrment_Works() {
            var vu = new VersionUnit("2", ".");
            vu.SetBehaviour(VersionIncrementBehaviour.AutoIncrementWithReset);
            var sut = new CompleteVersion(new VersionUnit("1"), vu);
            sut.PerformIncrement();
            Assert.AreEqual("1.3", sut.ToString(), "The verison increment should do nothing for fixed");
        }

        [TestMethod]
        public void Override_NoIncrement_DoesNotChangeValue() {
            var vu = new VersionUnit("1");
            vu.Value = "Monkey";
            vu.IncrementOverride = "Fish";
            var sut = new CompleteVersion(vu);

            Assert.AreEqual("Monkey", sut.GetVersionString(DisplayType.Full), "The overide should not change when no increment has been performed.");
        }
        [TestMethod]
        public void Increment_OverrideWorksForNumbers() {
            var vu = new VersionUnit("1");
            vu.IncrementOverride = "5";

            var sut = new CompleteVersion(vu);
            sut.PerformIncrement();

            Assert.AreEqual("5", sut.GetVersionString(DisplayType.Full), "The overide on a word value did not work");
        }

        [TestMethod]
        public void Increment_OverrideWorksForNames() {
            var vu = new VersionUnit("Monkey");
            vu.IncrementOverride = "Fish";

            var sut = new CompleteVersion(vu);
            sut.PerformIncrement();

            Assert.AreEqual("Fish", sut.GetVersionString(DisplayType.Full), "The overide on a word value did not work");
        }

        [TestMethod]
        public void Increment_OverrideWorksOnFixed() {
            var vu = new VersionUnit("1", "", VersionIncrementBehaviour.Fixed);
            vu.IncrementOverride = "Fish";

            var sut = new CompleteVersion(vu);
            sut.PerformIncrement();

            Assert.AreEqual("Fish", sut.GetVersionString(DisplayType.Full), "The overide on a word value did not work");
        }


        [TestMethod]
        public void ToString_Equals_GetVersionString() {
            var sut = new CompleteVersion(new VersionUnit("1"), new VersionUnit("0", "."));
            Assert.AreEqual(sut.ToString(), sut.GetVersionString(DisplayType.Full), "GetVersionString and ToString should return the same value");
        }

        [TestMethod]
        public void GetVersionString_Short_ShowsCorrect() {
            var sut = new CompleteVersion(new VersionUnit("1"), new VersionUnit("0", "."));
            var dt = DisplayType.Short;
            Assert.AreEqual(sut.GetVersionString(dt), "1.0", "The short display type should have two digits");
        }

        [TestMethod]
        public void GetVersionString_Short_EvenWhenMoreDigits() {
            var sut = new CompleteVersion(new VersionUnit("1"), new VersionUnit("0", "."), new VersionUnit("1", "."));
            var dt = DisplayType.Short;
            Assert.AreEqual("1.0", sut.GetVersionString(dt), "The short display type should have two digits");
        }

        [TestMethod]
        public void GetString_Respects_AlternativeFormatter() {
            var sut = new CompleteVersion(new VersionUnit("1"), new VersionUnit("0", "-"), new VersionUnit("1", "-"));
            Assert.AreEqual(sut.ToString(), "1-0-1", "The alternative formatter was not repected in to string");
        }

        [TestMethod]
        public void BasicTwoDigitToString_ReturnsCorrectly() {
            var sut = new CompleteVersion(new VersionUnit("1"), new VersionUnit("0", "."));
            Assert.IsTrue(sut.ToString() == "1.0", "The simplest test did not return the correct string");
        }
    }
}
