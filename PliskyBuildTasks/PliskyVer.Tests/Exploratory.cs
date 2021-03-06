﻿using Minimatch;
using Plisky.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Plisky.Build.Tests {

    public class Exploratory : IDisposable {
        private UnitTestHelper uth;
        private TestSupport ts;

        public Exploratory() {
            uth = new UnitTestHelper();
            ts = new TestSupport(uth);
        }

        [Fact]
        [Trait("xunit", "UserBug")]
        public void Bug464_BuildVersionsNotUpdatedDuringBuild() {
            string srcFile = @"D:\Files\Code\git\PliskyBuild\PliskyBuildTasks\_Dependencies\TestData\BugData\B464_AsmInfo_Source.txt";
            string fn = ts.GetFileAsTemporary(srcFile);
            var cv = new CompleteVersion(new VersionUnit("2"), new VersionUnit("0", "."), new VersionUnit("Unicorn", "-"), new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement));
            VersionFileUpdater sut = new VersionFileUpdater(cv);

            sut.PerformUpdate(fn, FileUpdateType.Assembly2);
            sut.PerformUpdate(fn, FileUpdateType.AssemblyInformational);
            sut.PerformUpdate(fn, FileUpdateType.AssemblyFile);

            Assert.False(ts.DoesFileContainThisText(fn, "AssemblyFileVersion(\"2.0.0\""), "The file version should be three digits and present");
            Assert.True(ts.DoesFileContainThisText(fn, "AssemblyInformationalVersion(\"2.0-Unicorn.0\""), "The informational version should be present");
            Assert.True(ts.DoesFileContainThisText(fn, "AssemblyVersion(\"2.0\")"), "the assembly version should be two digits and present.");
        }

        [Fact]
        [Trait("xunit", "Manual")]
        public void ManualExportVersionFile() {
            CompleteVersion cv = new CompleteVersion(new VersionUnit("1"), new VersionUnit("0", "."), new VersionUnit("Unicorn", "-"), new VersionUnit("0", ".", VersionIncrementBehaviour.AutoIncrementWithResetAny));
            VersionStorage vs = new JsonVersionPersister(@"c:\temp\output.json");
            vs.Persist(cv);
        }

        [Fact]
        public void BuildTask_InvalidRuleType_Throws() {
            TestableVersioningTask sut = new TestableVersioningTask();
            string verItemsSimple = "**/assemblyinfo.cs;ASSXXEMBLY";

            Assert.Throws<InvalidOperationException>(() => { sut.SetAllVersioningItems(verItemsSimple); });
        }

        [Fact]
        public void BuildTask_PassInRules_Works() {
            var v = GetDefaultVersion();
            TestableVersioningTask sut = new TestableVersioningTask();
            sut.SetVersionNumber(v);
            string verItemsSimple = "**/assemblyinfo.cs!ASSEMBLY";
            sut.SetAllVersioningItems(verItemsSimple);

            Assert.True(sut.IsThisMinimatchIncluded("**/assemblyinfo.cs"), "The minimatch was not included");
        }

        [Fact]
        public void BuildTask_PassInMultipleRules_Works() {
            var v = GetDefaultVersion();
            TestableVersioningTask sut = new TestableVersioningTask();
            sut.SetVersionNumber(v);
            string verItemsSimple = $"**/assemblyinfo.cs!ASSEMBLY{Environment.NewLine}xxMonkey!FILE{Environment.NewLine}yyzzxxbannana!WIX{Environment.NewLine}";
            sut.SetAllVersioningItems(verItemsSimple);

            Assert.True(sut.IsThisMinimatchIncluded("**/assemblyinfo.cs"), "The minimatch was not included");
            Assert.True(sut.IsThisMinimatchIncluded("xxMonkey"), "The second minimatch was not included");
            Assert.True(sut.IsThisMinimatchIncluded("yyzzxxbannana"), "The third minimatch was not included");
        }

        [Fact]
        [Trait("xunit", "usecase")]
        public void UseCase_Plisky_Works() {
            var sut = new CompleteVersion(
                new VersionUnit("2"),
                new VersionUnit("0", "."),
                new VersionUnit("Unicorn", "-"),
                new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement));

            var verString = sut.GetVersionString(DisplayType.FullIncludeText);
            Assert.Equal("2.0-Unicorn.0", verString);

            sut.PerformIncrement();
            verString = sut.GetVersionString(DisplayType.FullIncludeText);
            Assert.Equal("2.0-Unicorn.1", verString);

            sut.PerformIncrement();
            verString = sut.GetVersionString(DisplayType.FullIncludeText);
            Assert.Equal("2.0-Unicorn.2", verString);
        }


        [Fact]
        [Trait("xunit", "regression")]
        public void Display_Assem4_AllDigits_Correct() {
            var sut = new CompleteVersion(new VersionUnit("2"), new VersionUnit("0", "."), new VersionUnit("0", "."), new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement));
            var fut4 = sut.GetDisplayType(FileUpdateType.Assembly4);
            var verStringFor4 = sut.GetVersionString(fut4);
            Assert.Equal("2.0.0.0", verStringFor4);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);

            Assert.Equal("2.0.0.1", verStringFor4);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);

            Assert.Equal("2.0.0.2", verStringFor4);
        }

        [Fact]
        [Trait("xunit", "regression")]
        public void Display_Assem4_AllDigitsMultiIncrement_Correct() {
            var sut = new CompleteVersion(new VersionUnit("2", "", VersionIncrementBehaviour.ContinualIncrement),
                                          new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                                          new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                                          new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement)
                                          );

            var fut4 = sut.GetDisplayType(FileUpdateType.Assembly4);
            var verStringFor4 = sut.GetVersionString(fut4);
            Assert.Equal("2.0.0.0", verStringFor4);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);

            Assert.Equal("3.1.1.1", verStringFor4);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);

            Assert.Equal("4.2.2.2", verStringFor4);
        }


        [Fact]
        [Trait("xunit", "regression")]
        public void Display_Assem4_TextComponent_Correct() {
            var sut = new CompleteVersion(new VersionUnit("2"),
                                          new VersionUnit("0", "."),
                                          new VersionUnit("Unicorn", "-"),
                                          new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement)
                                          );

            var fut4 = sut.GetDisplayType(FileUpdateType.Assembly4);
            var verStringFor4 = sut.GetVersionString(fut4);
            Assert.Equal("2.0.0.0", verStringFor4);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);

            Assert.Equal("2.0.0.1", verStringFor4);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);

            Assert.Equal("2.0.0.2", verStringFor4);
        }


        [Fact]        
        [Trait("xunit", "regression")]
        public void Display_Assem2_AllDigits_Correct() {
            var sut = new CompleteVersion(new VersionUnit("2"), new VersionUnit("0", "."), new VersionUnit("0", "."), new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement));
            var fut2 = sut.GetDisplayType(FileUpdateType.Assembly2);
            var verStringFor2 = sut.GetVersionString(fut2);
            Assert.Equal("2.0", verStringFor2);

            sut.PerformIncrement();
            verStringFor2 = sut.GetVersionString(fut2);

            Assert.Equal("2.0", verStringFor2);

            sut.PerformIncrement();
            verStringFor2 = sut.GetVersionString(fut2);

            Assert.Equal("2.0", verStringFor2);
        }

        [Fact]        
        [Trait("xunit", "regression")]
        public void Display_Assem2_AllDigitsMultiIncrement_Correct() {
            var sut = new CompleteVersion(new VersionUnit("2", "", VersionIncrementBehaviour.ContinualIncrement),
                                          new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                                          new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                                          new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement)
                                          );

            var fut2 = sut.GetDisplayType(FileUpdateType.Assembly2);
            var verStringFor2 = sut.GetVersionString(fut2);
            Assert.Equal("2.0", verStringFor2);

            sut.PerformIncrement();
            verStringFor2 = sut.GetVersionString(fut2);

            Assert.Equal("3.1", verStringFor2);

            sut.PerformIncrement();
            verStringFor2 = sut.GetVersionString(fut2);

            Assert.Equal("4.2", verStringFor2);
        }


        [Fact]
        [Trait("xunit", "usecase")]
        public void UseCase_PliskyFileTypes_Works() {

            var sut = new CompleteVersion(new VersionUnit("2"), new VersionUnit("0", "."), new VersionUnit("Unicorn", "-"), new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement));
            var fut4 = sut.GetDisplayType(FileUpdateType.Assembly4);
            var fut2 = sut.GetDisplayType(FileUpdateType.Assembly2);
            var futInfo = sut.GetDisplayType(FileUpdateType.AssemblyInformational);

            var verStringFor4 = sut.GetVersionString(fut4);
            var verStringFor2 = sut.GetVersionString(fut2);
            var verStringForInfo = sut.GetVersionString(futInfo);

            // Initial values not incremented
            Assert.Equal("2.0.0.0", verStringFor4);
            Assert.Equal("2.0", verStringFor2);
            Assert.Equal("2.0-Unicorn.0", verStringForInfo);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);
            verStringFor2 = sut.GetVersionString(fut2);
            verStringForInfo = sut.GetVersionString(futInfo);

            // Values Following Single Increment
            Assert.Equal("2.0.0.1", verStringFor4);
            Assert.Equal("2.0", verStringFor2);
            Assert.Equal("2.0-Unicorn.1", verStringForInfo);

            sut.PerformIncrement();
            verStringFor4 = sut.GetVersionString(fut4);
            verStringFor2 = sut.GetVersionString(fut2);
            verStringForInfo = sut.GetVersionString(futInfo);

            // Values Following Second Increment
            Assert.Equal("2.0.0.2", verStringFor4);
            Assert.Equal("2.0", verStringFor2);
            Assert.Equal("2.0-Unicorn.2", verStringForInfo);


        }

        [Fact]
        public void IncrementAndUpdateThrowsIfNoDirectory() {
            VersioningTask sut = new VersioningTask();
            Assert.Throws<DirectoryNotFoundException>(() => { sut.IncrementAndUpdateAll(); });
        }

        [Fact]
        [Trait("xunit", "exploratory")]
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

            Assert.Equal("1.1.1.1", sut.VersionString); //, "The version string should be set post update");
            var jp = new JsonVersionPersister(tfn2);
            Assert.Equal(sut.VersionString, jp.GetVersion().GetVersionString()); //, "The update should be persisted");
            Assert.True(ts.DoesFileContainThisText(tfn1, "AssemblyVersion(\"1.1"), "The target filename was not updated");
            Assert.True(ts.DoesFileContainThisText(tfn1, "AssemblyInformationalVersion(\"1.1.1.1"), "The target filename was not updated");
            Assert.True(ts.DoesFileContainThisText(tfn1, "AssemblyFileVersion(\"1.1.1.1"), "The target filename was not updated");
        }

        [Fact]
        [Trait("xunit", "exlporatory")]
        public void MinimatchSyntax_Research() {
            var mtchs = new List<Tuple<string, bool>>();
            mtchs.Add(new Tuple<string, bool>(@"C:\temp\verworking\assemblyinfo.cs", true));
            CheckTheseMatches(mtchs, @"**\assemblyinfo.cs");
        }

        [Fact]
        [Trait("xunit", "exlporatory")]
        public void MiniMatchSyntax_FindAssemblyInfo() {
            var mtchs = new List<Tuple<string, bool>>();
            mtchs.Add(new Tuple<string, bool>(@"C:\temp\te st\properties\assemblyinfo.cs", true));
            mtchs.Add(new Tuple<string, bool>(@"C:\te mp\test\assemblyinfo.cs", false));
            mtchs.Add(new Tuple<string, bool>(@"C:\te mp\t e s t\properties\notassemblyinfo.cs", false));
            mtchs.Add(new Tuple<string, bool>(@"C:\temp\test\properties\assemblyinfo.cs.txt", false));
            mtchs.Add(new Tuple<string, bool>(@"C:\a\1\s\PliskyLibrary\PliskyLib\Properties\AssemblyInfo.cs", true));
            string againstThis = @"**\properties\assemblyinfo.cs";
            CheckTheseMatches(mtchs, againstThis);

            var mm2 = new Minimatcher(@"C:\temp\test\testfile.tst", new Options { AllowWindowsPaths = true });
            Assert.True(mm2.IsMatch(@"C:\temp\test\testfile.tst"), "Cant match on full filename");
        }

        private static void CheckTheseMatches(List<Tuple<string, bool>> mtchs, string againstThis) {
            var mm = new Minimatcher(againstThis, new Options { AllowWindowsPaths = true, IgnoreCase = true });
            int i = 0;
            foreach (var v in mtchs) {
                i++;
                bool isMatch = mm.IsMatch(v.Item1);
                Assert.Equal(v.Item2, isMatch); //, "Mismatch " + i.ToString());
            }
        }

        private string CreateStoredVersionNumer() {
            var fn = uth.NewTemporaryFileName(true);
            CompleteVersion cv = GetDefaultVersion();
            JsonVersionPersister jvp = new JsonVersionPersister(fn);
            jvp.Persist(cv);
            return fn;
        }

        private static CompleteVersion GetDefaultVersion() {
            return new CompleteVersion(
                new VersionUnit("0", "", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("0", ".", VersionIncrementBehaviour.ContinualIncrement)
                );
        }

        [Fact]
        public void VersionStorage_SavesCorrectly() {
            var msut = new MockVersionStorage("itsamock");
            VersionStorage sut = msut;

            var cv = new CompleteVersion(new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"));
            sut.Persist(cv);
            Assert.True(msut.PersistWasCalled, "The persist method was not called");
            Assert.Equal(msut.VersionStringPersisted, "1111"); //, "The wrong version string was persisted");
        }

        [Fact]
        public void VersionStorage_Json_Saves() {
            string fn = uth.NewTemporaryFileName(true);
            var sut = new JsonVersionPersister(fn);
            var cv = new CompleteVersion(new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"), new VersionUnit("1"));
            sut.Persist(cv);
            Assert.True(File.Exists(fn), "The file must be created");
        }

        [Fact]
        public void VersionStorage_Json_Loads() {
            string fn = uth.NewTemporaryFileName(true);
            var sut = new JsonVersionPersister(fn);
            var cv = new CompleteVersion(new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement), new VersionUnit("Alpha", "-"), new VersionUnit("1"), new VersionUnit("1", "", VersionIncrementBehaviour.ContinualIncrement));
            sut.Persist(cv);

            var cv2 = sut.GetVersion();

            Assert.Equal(cv.GetVersionString(), cv2.GetVersionString()); //, "The loaded type was not the same as the saved one, values");
            cv.PerformIncrement(); cv2.PerformIncrement();
            Assert.Equal(cv.GetVersionString(), cv2.GetVersionString()); //, "The loaded type was not the same as the saved one, behaviours");
        }

        [Fact]
        public void VersionStoreAndLoad_StoresUpdatedValues() {
            string fn = uth.NewTemporaryFileName(true);
            var sut = new JsonVersionPersister(fn);
            var cv = new CompleteVersion(new VersionUnit("1", "", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement),
                new VersionUnit("1", ".", VersionIncrementBehaviour.ContinualIncrement));

            cv.PerformIncrement();
            var beforeStore = cv.GetVersionString();
            sut.Persist(cv);
            var cv2 = sut.GetVersion();

            Assert.Equal(cv.GetVersionString(), cv2.GetVersionString()); //, "The two version strings should match");
            Assert.Equal("2.2.2.2", cv2.GetVersionString()); //, "The loaded version string should keep the increment");

            cv.PerformIncrement(); cv2.PerformIncrement();
            Assert.Equal(cv.GetVersionString(), cv2.GetVersionString()); //, "The two version strings should match");
        }

        [Fact]
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

            Assert.Equal(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.Assembly4)); //, "The file update type assembly was not returned correctly");
            Assert.Equal(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.AssemblyFile)); //, "The file update type AssemblyFile was not returned correctly");
            Assert.Equal(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.AssemblyInformational)); //, "The file update type AssemblyInformational was not returned correctly");
            Assert.Equal(DisplayType.NoDisplay, cv2.GetDisplayType(FileUpdateType.Wix)); //, "The file update type assembly was not returned correctly");
        }

        public void Dispose() {
            uth.ClearUpTestFiles();
        }
    }
}