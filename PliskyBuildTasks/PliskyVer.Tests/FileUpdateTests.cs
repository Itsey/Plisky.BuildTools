using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plisky.Helpers;
using System.IO;
using Plisky.Build;

namespace Plisky.Build.Tests {
    [TestClass]
    public class FileUpdateTests{
        UnitTestHelper uth;
        TestSupport ts;

        public FileUpdateTests() {
            uth = new UnitTestHelper();
            ts = new TestSupport(uth);
        }
        

        
        [TestCleanup]
        public void Cleanup () {
            uth.ClearUpTestFiles();
        }

        [TestMethod]
        [DeploymentItem(".\\TestDataFiles\\")]
        [TestCategory("regression")]
        public void Update_AsmVersion_Works() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1","."), new VersionUnit("1","."), new VersionUnit("1","."));
            string srcFile = @"JustAssemblyVersion.txt";
            string fn = ts.GetFileAsTemporary(srcFile);
           
            VersionFileUpdater sut = new VersionFileUpdater(cv);
            
            sut.PerformUpdate(fn,FileUpdateType.Assembly4);

            Assert.IsFalse(ts.DoesFileContainThisText(fn, "0.0.0.0"), "No update was made to the file at all");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "1.1"), "The file does not appear to have been updated correctly.");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "AssemblyVersion(\"1.1\")"), "The file does not have the full version in it");
        }

        [TestMethod]
        public void CompleteVersionHasAllDisplayTypesCovered() {
            // Complete verison has in it hard coded array of display type to name mappings
            // This method blows when the enum changes and the display types have not been nupdated
            CompleteVersion cv = new CompleteVersion();
            foreach (int i in Enum.GetValues(typeof(FileUpdateType))) {
                var val = cv.displayTypes[i];
                Assert.IsTrue(Enum.IsDefined(typeof(DisplayType), val), "The enum mapping in the CompleteVersion constructor is out of date");
            }
        }


        [TestMethod]
        [DeploymentItem(".\\TestDataFiles\\")]
        public void Update_AssemblyTwoDigit_Works() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("9"), new VersionUnit("9", "."), new VersionUnit("1", "."), new VersionUnit("1", "."));
            string srcFile = @"JustAssemblyVersion.txt";
            string fn = ts.GetFileAsTemporary(srcFile);
            Assert.IsFalse(ts.DoesFileContainThisText(fn, "AssemblyVersion(\"9.9\")"), "Invalikd test the test cant start wtih the value written");

            VersionFileUpdater sut = new VersionFileUpdater(cv);
            sut.PerformUpdate(fn, FileUpdateType.Assembly2);

            Assert.IsTrue(ts.DoesFileContainThisText(fn, "AssemblyVersion(\"9.9\")"), "The two digit update was not written");
        }


        [TestMethod]
        [DeploymentItem(".\\TestDataFiles\\")]
        [TestCategory("regression")]
        public void Update_DoesNotAlterOtherAttributes() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1", "."), new VersionUnit("1", "."), new VersionUnit("1", "."));
            string srcFile = @"assemblyinfo.txt";
            string fn = ts.GetFileAsTemporary(srcFile);

            VersionFileUpdater sut = new VersionFileUpdater(cv);
            sut.PerformUpdate(fn, FileUpdateType.Assembly4);
            
            Assert.IsFalse(ts.DoesFileContainThisText(fn, " AssemblyVersion(\"1.0.0.0\")"), "No update was made to the file at all");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "[assembly: AssemblyFileVersion(\"1.0.0.0\")]"), "The file does not appear to have been updated correctly.");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "[assembly: AssemblyCompany(\"\")]"), "Collatoral Damage - Another element in the file was updated - Company");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "[assembly: Guid(\"557cc26f-fcb2-4d0e-a34e-447295115fc3\")]"), "Collatoral Damage - Another element in the file was updated - Guid");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "// [assembly: AssemblyVersion(\"1.0.*\")]"), "Collatoral Damage - Another element in the file was updated - Comment");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "using System.Reflection;"), "Collatoral Damage - Another element in the file was updated - Reflection First Line");
        }

        [TestMethod]
        [DeploymentItem(".\\TestDataFiles\\")]
        [TestCategory("regression")]
        public void Update_AsmInfVer_Works() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1", "."), new VersionUnit("1", "."), new VersionUnit("1", "."));
            string srcFile = @"JustInformationalVersion.txt";
            string fn = ts.GetFileAsTemporary(srcFile);

            VersionFileUpdater sut = new VersionFileUpdater(cv);

            sut.PerformUpdate(fn, FileUpdateType.AssemblyInformational);

            Assert.IsFalse(ts.DoesFileContainThisText(fn, "0.0.0.0"), "No update was made to the file at all");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "1.1"), "The file does not appear to have been updated correctly.");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "AssemblyInformationalVersion(\"1.1.1.1\")"), "The file does not have the full version in it");
        }

        [DeploymentItem(".\\TestDataFiles\\")]
        [TestMethod][TestCategory("regression")]
        public void Update_AsmFileVer_Works() {
            System.Console.WriteLine(System.Environment.CurrentDirectory‌);
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1", "."), new VersionUnit("1", "."), new VersionUnit("1", "."));
            string srcFile = @"JustFileVersion.txt";
            string fn = ts.GetFileAsTemporary(srcFile);

            VersionFileUpdater sut = new VersionFileUpdater(cv);

            sut.PerformUpdate(fn, FileUpdateType.AssemblyFile);

            Assert.IsFalse(ts.DoesFileContainThisText(fn, "0.0.0.0"), "No update was made to the file at all");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "1.1"), "The file does not appear to have been updated correctly.");
            Assert.IsTrue(ts.DoesFileContainThisText(fn, "AssemblyFileVersion(\"1.1.1.1\")"), "The file does not have the full version in it");
        }

    }
}
