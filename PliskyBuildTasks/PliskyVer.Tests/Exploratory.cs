using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plisky.Plumbing;
using Plisky.Helpers;
using System.IO;
using System.Collections.Generic;
using Minimatch;

namespace Plisky.Build.Tests {
    [TestClass]
    public class Exploratory {

        UnitTestHelper uth;
        TestSupport ts;

        public Exploratory() {
            uth = new UnitTestHelper();
            ts = new TestSupport(uth);
        }


        [TestCleanup]
        public void Cleanup() {
            uth.ClearUpTestFiles();
        }
       
        [TestMethod][TestCategory("exploratory")]
        public void UseCase_Plisky_Works() {
            var sut = new CompleteVersion(new VersionUnit("2"), new VersionUnit("0", "."), new VersionUnit("Unicorn", "-"), new VersionUnit("0", ".",VersionIncrementBehaviour.ContinualIncrement));
            var verString = sut.GetVersionString();
            Assert.AreEqual("2.0-Unicorn.0",verString,"The initial string is not correct");
            sut.PerformIncrement();
            verString = sut.GetVersionString();
            Assert.AreEqual("2.0-Unicorn.1", verString, "The first increment string is not correct");
            sut.PerformIncrement();
            verString = sut.GetVersionString(DisplayType.Full);
            Assert.AreEqual("2.0-Unicorn.2", verString, "The second increment string is not correct");

        }

        [TestMethod]
        [TestCategory("exploratory")]
        public void UseCase_PliskyFileTypes_Works() {
            // MRC
            var sut = new CompleteVersion(new VersionUnit("2"), new VersionUnit("0", "."), new VersionUnit("Unicorn", "-"), new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement));
            var fut4 = sut.GetDisplayType(FileUpdateType.Assembly4);
            var verString = sut.GetVersionString(fut4);
            Assert.AreEqual("2.0-Unicorn.0", verString, "The initial string is not correct");
            sut.PerformIncrement();
            verString = sut.GetVersionString();
            Assert.AreEqual("2.0-Unicorn.1", verString, "The first increment string is not correct");
            sut.PerformIncrement();
            verString = sut.GetVersionString(DisplayType.Full);
            Assert.AreEqual("2.0-Unicorn.2", verString, "The second increment string is not correct");

        }

        [TestMethod][ExpectedException(typeof(DirectoryNotFoundException))]
        public void IncrementAndUpdateThrowsIfNoDirectory() {
            VersioningTask sut = new VersioningTask();
            sut.IncrementAndUpdateAll();
        }

        [TestMethod][TestCategory("exploratory")]
        public void UseCase_BuildVersionger_Exploratory() {
            string tfn1 = uth.NewTemporaryFileName(false);
            string tfn2 = CreateStoredVersionNumer();
            VersioningTask sut = new VersioningTask();
            string directory = Path.GetDirectoryName(tfn1);
            sut.BaseSearchDir = directory;
            sut.PersistanceValue = tfn2;
            sut.AddUpdateType(tfn1, FileUpdateType.Assembly4);
            sut.AddUpdateType(tfn1, FileUpdateType.AssemblyFile);
            sut.AddUpdateType(tfn1, FileUpdateType.AssemblyInformational);
            sut.IncrementAndUpdateAll();

            Assert.AreEqual( "1.1.1.1", sut.VersionString, "The version string should be set post update");
            var jp = new JsonVersionPersister(tfn2);
            Assert.AreEqual(sut.VersionString, jp.GetVersion().GetVersionString(), "The update should be persisted");
            Assert.IsTrue(ts.DoesFileContainThisText(tfn1, "AssemblyVersion(\"1.1"), "The target filename was not updated");
            Assert.IsTrue(ts.DoesFileContainThisText(tfn1, "AssemblyInformationalVersion(\"1.1.1.1"), "The target filename was not updated");
            Assert.IsTrue(ts.DoesFileContainThisText(tfn1, "AssemblyFileVersion(\"1.1.1.1"), "The target filename was not updated");
        }

        [TestMethod][TestCategory("exlporatory")]
        public void MiniMatchSyntax_FindAssemblyInfo() {
            var mtchs = new List<Tuple<string, bool>>();
            mtchs.Add(new Tuple<string, bool>(@"C:\temp\te st\properties\assemblyinfo.cs", true));
            mtchs.Add(new Tuple<string, bool>(@"C:\te mp\test\assemblyinfo.cs", false));
            mtchs.Add(new Tuple<string, bool>(@"C:\te mp\t e s t\properties\notassemblyinfo.cs", false));
            mtchs.Add(new Tuple<string, bool>(@"C:\temp\test\properties\assemblyinfo.cs.txt", false));

            var mm = new Minimatcher(@"**\properties\assemblyinfo.cs", new Options { AllowWindowsPaths = true });
            int i = 0;
            foreach (var v in mtchs) {
                i++;
                bool isMatch = mm.IsMatch(v.Item1);
                Assert.AreEqual(v.Item2, isMatch, "Mismatch " + i.ToString());
            }

            var mm2 = new Minimatcher(@"C:\temp\test\testfile.tst", new Options { AllowWindowsPaths = true });
            Assert.IsTrue(mm2.IsMatch(@"C:\temp\test\testfile.tst"), "Cant match on full filename");
        }

        private string CreateStoredVersionNumer() {
            var fn = uth.NewTemporaryFileName(true);
            var cv = new CompleteVersion(
                new VersionUnit("0", "",VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement)
                );
            JsonVersionPersister jvp = new JsonVersionPersister(fn);
            jvp.Persist(cv);
            return fn;
        }

        [TestMethod]
        public void VersionStorage_SavesCorrectly() {
            var msut = new MockVersionStorage("itsamock");
            VersionStorage sut = msut;

            var cv = new CompleteVersion(new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"));
            sut.Persist(cv);
            Assert.IsTrue(msut.PersistWasCalled, "The persist method was not called");
            Assert.AreEqual(msut.VersionStringPersisted, "1111", "The wrong version string was persisted");
        }

        [TestMethod]
        public void VersionStorage_Json_Saves() {
            string fn = uth.NewTemporaryFileName(true);
            var sut = new JsonVersionPersister(fn);
            var cv = new CompleteVersion(new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"));
            sut.Persist(cv);
            Assert.IsTrue(File.Exists(fn), "The file must be created");
        }

        [TestMethod]
        public void VersionStorage_Json_Loads() {
            string fn = uth.NewTemporaryFileName(true);
            var sut = new JsonVersionPersister(fn);
            var cv = new CompleteVersion(new VersionUnit("1",".",VersionIncrementBehaviour.ContinualIncrement), new VersionUnit("Alpha","-"), new VersionUnit("1"), new VersionUnit("1","",VersionIncrementBehaviour.ContinualIncrement));
            sut.Persist(cv);

            var cv2 = sut.GetVersion();

            Assert.AreEqual(cv.GetVersionString(), cv2.GetVersionString(), "The loaded type was not the same as the saved one, values");
            cv.PerformIncrement(); cv2.PerformIncrement();
            Assert.AreEqual(cv.GetVersionString(), cv2.GetVersionString(), "The loaded type was not the same as the saved one, behaviours");
        }


        [TestMethod]
        public void VersionStoreAndLoad_StoresUpdatedValues() {
            string fn = uth.NewTemporaryFileName(true);
            var sut = new JsonVersionPersister(fn);
            var cv = new CompleteVersion(new VersionUnit("1","", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement));

            cv.PerformIncrement();
            var beforeStore = cv.GetVersionString();
            sut.Persist(cv);
            var cv2 = sut.GetVersion();

            Assert.AreEqual(cv.GetVersionString(), cv2.GetVersionString(), "The two version strings should match");
            Assert.AreEqual("2.2.2.2", cv2.GetVersionString(), "The loaded version string should keep the increment");

            cv.PerformIncrement(); cv2.PerformIncrement();
            Assert.AreEqual(cv.GetVersionString(), cv2.GetVersionString(), "The two version strings should match");
        }


        [TestMethod]
        public void VersionStoreAndLoad_StoresDisplayTypes() {
            string fn = uth.NewTemporaryFileName(true);
            var sut = new JsonVersionPersister(fn);
            var cv = new CompleteVersion(new VersionUnit("1", "", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement));
            // None of the defaults are no display, therefore this should set all to a new value
            cv.SetDisplayTypeForVersion(FileUpdateType.Assembly4, DisplayType.NoDisplay);
            cv.SetDisplayTypeForVersion(FileUpdateType.AssemblyFile, DisplayType.NoDisplay);
            cv.SetDisplayTypeForVersion(FileUpdateType.AssemblyInformational, DisplayType.NoDisplay);
            cv.SetDisplayTypeForVersion(FileUpdateType.Wix, DisplayType.NoDisplay);

            
            sut.Persist(cv);
            var cv2 = sut.GetVersion();

            Assert.AreEqual(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.Assembly4),"The file update type assembly was not returned correctly");
            Assert.AreEqual(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.AssemblyFile), "The file update type AssemblyFile was not returned correctly");
            Assert.AreEqual(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.AssemblyInformational), "The file update type AssemblyInformational was not returned correctly");
            Assert.AreEqual(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.Wix), "The file update type assembly was not returned correctly");

        }

    }
}
