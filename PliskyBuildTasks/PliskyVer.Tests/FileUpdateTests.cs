using System;
using Plisky.Helpers;
using System.IO;
using Plisky.Build;
using System.Text.RegularExpressions;
using Xunit;

namespace Plisky.Build.Tests {
    
    public class FileUpdateTests : IDisposable {
        UnitTestHelper uth;
        TestSupport ts;

        public FileUpdateTests() {
            uth = new UnitTestHelper();
            ts = new TestSupport(uth);
        }
        

        [Fact][Trait("xunit","regression")]
        public void Regex_MatchesForAssembly() {

            VersionFileUpdater sut = new VersionFileUpdater();
            //var rxstr = "\\s*\\[\\s*assembly\\s*:\\s*AssemblyVersion\\s*\\(\\s*\\\"\\s*[0-9A-z\\-.*]*\\s*\\\"\\s*\\)\\s*\\]";
            //var rx = new Regex(rxstr, RegexOptions.IgnoreCase);
            var rx = sut.GetRegex("AssemblyVersion");
            Assert.True(rx.IsMatch("[assembly: AssemblyVersion(\"0.0.0.0\")]"), "1 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyVersion(\"0.0.0\")]"), "2 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyVersion(\"0.0\")]"), "3 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyVersion(\"0\")]"), "4 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyVersion(\"\")]"), "5 Invalid match for an assembly version");           
            Assert.True(rx.IsMatch("[assembly:      AssemblyVersion     (\"0.0.0.0\"   )     ]"), "7 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly     :AssemblyVersion(\"0.0.0.0\")]"), "8 Invalid match for an assembly version");

            // Assert.False(rx.IsMatch("//[assembly: AssemblyVersion(\"0.0.0.0\")]"), "6 Invalid match for an assembly version");
            // Assert.False(rx.IsMatch("   //    [assembly: AssemblyVersion(\"0.0.0.0\")]"), "9 Invalid match for an assembly version");
        }



        [Fact]
        [Trait("xunit","regression")]
        public void Regex_MatchesForInformational() {

            VersionFileUpdater sut = new VersionFileUpdater();
            var rx = sut.GetRegex("AssemblyFileVersion");
            Assert.True(rx.IsMatch("[assembly: AssemblyFileVersion(\"0.0.0.0\")]"), "1 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyFileVersion(\"0.0.0\")]"), "2 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyFileVersion(\"0.0\")]"), "3 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyFileVersion(\"0\")]"), "4 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyFileVersion(\"\")]"), "5 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly:      AssemblyFileVersion     (\"0.0.0.0\"   )     ]"), "7 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly     :AssemblyFileVersion(\"0.0.0.0\")]"), "8 Invalid match for an assembly version");
        }



        [Fact]
        [Trait("xunit","regression")]
        public void Regex_MatchesForFile() {

            VersionFileUpdater sut = new VersionFileUpdater();
            var rx = sut.GetRegex("AssemblyInformationalVersion");

            Assert.True(rx.IsMatch("[assembly: AssemblyInformationalVersion(\"0.0.0.0\")]"), "1 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyInformationalVersion(\"0.0.0\")]"), "2 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyInformationalVersion(\"0.0\")]"), "3 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyInformationalVersion(\"0\")]"), "4 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly: AssemblyInformationalVersion(\"\")]"), "5 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly:      AssemblyInformationalVersion     (\"0.0.0.0\"   )     ]"), "7 Invalid match for an assembly version");
            Assert.True(rx.IsMatch("[assembly     :AssemblyInformationalVersion(\"0.0.0.0\")]"), "8 Invalid match for an assembly version");
        }

        [Fact(Skip ="Hardcoded path")]
        [Trait("xunit","regression")]
        public void Update_AsmVersion_Works() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1","."), new VersionUnit("1","."), new VersionUnit("1","."));
            string srcFile = @"D:\Files\Code\git\Scratchpad\PliskyVersioning\_Dependencies\TestData\TestFileStructure\JustAssemblyVersion.txt";
            string fn = ts.GetFileAsTemporary(srcFile);
           
            VersionFileUpdater sut = new VersionFileUpdater(cv);
            
            sut.PerformUpdate(fn,FileUpdateType.Assembly4);

            Assert.False(ts.DoesFileContainThisText(fn, "0.0.0.0"), "No update was made to the file at all");
            Assert.True(ts.DoesFileContainThisText(fn, "1.1"), "The file does not appear to have been updated correctly.");
            Assert.True(ts.DoesFileContainThisText(fn, "AssemblyVersion(\"1.1\")"), "The file does not have the full version in it");
        }


        [Fact(Skip = "Hardcoded path")]
        [Trait("xunit","regression")]
        public void Update_DoesNotAlterOtherAttributes() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1", "."), new VersionUnit("1", "."), new VersionUnit("1", "."));
            string srcFile = @"D:\Files\Code\git\Scratchpad\PliskyVersioning\_Dependencies\TestData\TestFileStructure\DoesNotChange\assemblyinfo.txt";
            string fn = ts.GetFileAsTemporary(srcFile);

            VersionFileUpdater sut = new VersionFileUpdater(cv);

            sut.PerformUpdate(fn, FileUpdateType.Assembly4);
            
            Assert.False(ts.DoesFileContainThisText(fn, " AssemblyVersion(\"1.0.0.0\")"), "No update was made to the file at all");
            Assert.True(ts.DoesFileContainThisText(fn, "[assembly: AssemblyFileVersion(\"1.0.0.0\")]"), "The file does not appear to have been updated correctly.");
            Assert.True(ts.DoesFileContainThisText(fn, "[assembly: AssemblyCompany(\"\")]"), "Collatoral Damage - Another element in the file was updated - Company");
            Assert.True(ts.DoesFileContainThisText(fn, "[assembly: Guid(\"557cc26f-fcb2-4d0e-a34e-447295115fc3\")]"), "Collatoral Damage - Another element in the file was updated - Guid");
            Assert.True(ts.DoesFileContainThisText(fn, "// [assembly: AssemblyVersion(\"1.0.*\")]"), "Collatoral Damage - Another element in the file was updated - Comment");
            Assert.True(ts.DoesFileContainThisText(fn, "using System.Reflection;"), "Collatoral Damage - Another element in the file was updated - Reflection First Line");
        }

        [Fact(Skip = "Hardcoded path")]
        [Trait("xunit","regression")]
        public void Update_AsmInfVer_Works() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1", "."), new VersionUnit("1", "."), new VersionUnit("1", "."));
            string srcFile = @"D:\Files\Code\git\Scratchpad\PliskyVersioning\_Dependencies\TestData\TestFileStructure\JustInformationalVersion.txt";
            string fn = ts.GetFileAsTemporary(srcFile);

            VersionFileUpdater sut = new VersionFileUpdater(cv);

            sut.PerformUpdate(fn, FileUpdateType.AssemblyInformational);

            Assert.False(ts.DoesFileContainThisText(fn, "0.0.0.0"), "No update was made to the file at all");
            Assert.True(ts.DoesFileContainThisText(fn, "1.1"), "The file does not appear to have been updated correctly.");
            Assert.True(ts.DoesFileContainThisText(fn, "AssemblyInformationalVersion(\"1.1.1.1\")"), "The file does not have the full version in it");
        }


        [Fact(Skip = "Hardcoded path")]
        [Trait("xunit","regression")]
        public void Update_AsmFileVer_Works() {
            Plisky.Build.CompleteVersion cv = new Plisky.Build.CompleteVersion(new VersionUnit("1"), new VersionUnit("1", "."), new VersionUnit("1", "."), new VersionUnit("1", "."));
            string srcFile = @"D:\Files\Code\git\Scratchpad\PliskyVersioning\_Dependencies\TestData\TestFileStructure\JustFileVersion.txt";
            string fn = ts.GetFileAsTemporary(srcFile);

            VersionFileUpdater sut = new VersionFileUpdater(cv);

            sut.PerformUpdate(fn, FileUpdateType.AssemblyFile);

            Assert.False(ts.DoesFileContainThisText(fn, "0.0.0.0"), "No update was made to the file at all");
            Assert.True(ts.DoesFileContainThisText(fn, "1.1"), "The file does not appear to have been updated correctly.");
            Assert.True(ts.DoesFileContainThisText(fn, "AssemblyFileVersion(\"1.1.1.1\")"), "The file does not have the full version in it");
        }

        public void Dispose() {
            uth.ClearUpTestFiles();
        }
    }
}
