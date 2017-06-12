using Minimatch;
using Plisky.Plumbing;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Plisky.Build {
    public class VersionFileUpdater {
        private const string ASMFILE_FILEVER_TAG = "AssemblyFileVersion";
        private const string ASMFILE_VER_TAG = "AssemblyVersion";
        private const string ASMFILE_INFVER_TAG = "AssemblyInformationalVersion";

        private Minimatcher AssemblyMM;
        private Minimatcher InfoMM;
        private Minimatcher WixMM;
        private IHookVersioningChanges hook;

        private CompleteVersion cv;
        private string RootPath;

        public VersionFileUpdater() {
        }

        public VersionFileUpdater(CompleteVersion cv, IHookVersioningChanges actions = null) {
            this.cv = cv;
        }

        public Regex GetRegex(string targetAttribute) {
            return new Regex("\\s*\\[\\s*assembly\\s*:\\s*" + targetAttribute + "\\s*\\(\\s*\\\"\\s*[0-9A-z\\-.*]*\\s*\\\"\\s*\\)\\s*\\]", RegexOptions.IgnoreCase);
        }

        public void PerformUpdate(string fl, FileUpdateType fut) {
            Bilge.VerboseLog("Perform update requested " + fut.ToString(), fl);
            string versonToWrite = cv.GetVersionString(cv.GetDisplayType(fut));
            switch (fut) {
                case FileUpdateType.Assembly4:
                case FileUpdateType.Assembly2:
                    UpdateCSFileWithAttribute(fl, ASMFILE_VER_TAG, versonToWrite);
                    break;
                case FileUpdateType.AssemblyInformational:
                    UpdateCSFileWithAttribute(fl, ASMFILE_INFVER_TAG, versonToWrite);
                    break;
                case FileUpdateType.AssemblyFile:
                    UpdateCSFileWithAttribute(fl, ASMFILE_FILEVER_TAG, versonToWrite);
                    break;
                case FileUpdateType.Wix:
                    break;
                default:
                    break;
            }

        }

        /*  private void RecurseRootDirectoryAndUpdateFiles() {
              Bilge.Log("Starting to look for files to replace...");
              int filesChecked = 0;
              if (hook != null) {
                  hook.PreUpdateAllAction(RootPath);
              }

              foreach (var fl in Directory.EnumerateFiles(RootPath, "*.*", SearchOption.AllDirectories)) {
                  filesChecked++;
                  PerformUpdate(fl);
              }

              if (hook != null) {
                  hook.PostUpdateAllAction(RootPath);
              }

              Bilge.Log("File update has completed - " + filesChecked.ToString());
          }

          */

        private bool CheckForAssemblyVersion(string fl) {
            if (AssemblyMM == null) { return false; }
            string assemblyVerString = cv.GetVersionString(DisplayType.Short);
            return CheckAndUpdate(fl, AssemblyMM, assemblyVerString, (theFile, theVn) => {
                UpdateCSFileWithAttribute(fl, ASMFILE_VER_TAG, theVn);
            });
        }

        private bool CheckForInformationalVersion(string fl) {

            if (InfoMM == null) { return false; }
            string assemblyVerString = cv.GetVersionString(DisplayType.Short);

            return CheckAndUpdate(fl, InfoMM, assemblyVerString, (theFile, theVn) => {
                UpdateCSFileWithAttribute(fl, ASMFILE_INFVER_TAG, theVn);
            });

        }

        private bool CheckForWix(string fl) {

            if (WixMM == null) { return false; }
            string assemblyVerString = cv.GetVersionString(DisplayType.Short);

            return CheckAndUpdate(fl, WixMM, assemblyVerString, (theFile, theVn) => {
                // TODO : UpdateWixFileWithVersion(fl, theVn);
            });
        }



        private bool CheckAndUpdate(string fl, Minimatcher assemblyMM, string versionValue, Action<string, string> p) {
            Bilge.Assert(p != null, "The action used cant be null");

            Bilge.VerboseLog("Checking file :" + fl);

            bool result = assemblyMM.IsMatch(fl);
            if ((result) && (File.Exists(fl))) {
                Bilge.Log($"Updating VersioningFile File ({fl}) to ({versionValue})");

                hook?.PreUpdateFileAction(fl); // PreUpdateAllAction?.Invoke(fl);
                //PreUpdateAction?.Invoke(fl);

                p(fl, versionValue);

                hook?.PostUpdateFileAction(fl);


            }
            return result;
        }

        /// <summary>
        /// Either updates an existing version number in a file or creates a new (very basic) assembly info file and adds the verison number to it.  The
        /// version is stored in the attribute that is supplied as the second parameter.
        /// </summary>
        /// <param name="fileName">The full path to the file to either update or create</param>
        /// <param name="targetAttribute">The name of the attribute to write the verison number into</param>
        /// <param name="vn">The verison number to apply to the code</param>
        private void UpdateCSFileWithAttribute(string fileName, string targetAttribute, string versionValue) {
            #region entry code
            Bilge.Assert(!string.IsNullOrEmpty(fileName), "fileName is null, internal consistancy error.");
            Bilge.Assert(!string.IsNullOrEmpty(targetAttribute), "target attribute cant be null, internal consistancy error");
            Bilge.Assert(versionValue != null, "vn cant be null, internal consistancy error");
            #endregion (entry code)

            Bilge.Log(string.Format("VersionSupport, Asked to update CS file with the {0} attribute", targetAttribute), "Full Filename:" + fileName);

            var outputFile = new StringBuilder();


            if (!File.Exists(fileName)) {
                Bilge.VerboseLog("There was no file, creating file and adding attribute");
                outputFile.Append("using System.Reflection;\r\n");
                outputFile.Append($"[assembly: {targetAttribute}(\"{versionValue}\")]\r\n");
            } else {

                // If it does exist we need to verify that it is not readonly.
                if ((File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    Bilge.Warning("The file is readonly, removing attribs so I can write on it", "fname [" + fileName + "]");
                    File.SetAttributes(fileName, (File.GetAttributes(fileName) ^ FileAttributes.ReadOnly));
                }


                // Put this in to identify if there were duplicate entries discovered in the file, this should not be valid but helps to reassure that its not the verisoner that has
                // introduced a compile error into the code.                
                bool replacementMade = false;

                Regex r = new Regex("\\[\\s*assembly\\s*:\\s*" + targetAttribute + "\\s*\\(\\s*\\\"\\s*[0-9*]+.[0-9*]+.[0-9*]+.[0-9*]+\\s*\\\"\\s*\\)\\s*\\]", RegexOptions.IgnoreCase);

                Regex r = GetRegex(targetAttribute);
                using (StreamReader sr = new StreamReader(fileName)) {
                    string nextLine = null;
                    while ((nextLine = sr.ReadLine()) != null) {

                        if ((!nextLine.Trim().StartsWith("//")) && (r.IsMatch(nextLine))) {

                            if (replacementMade) {
                                // One would hope that this would not occur outside of testing, yet surprisingly enough this is not the case.
                                throw new ArgumentException("Invalid CSharp File, duplicate verison attribute discovered", fileName);
                            }

                            //  its the line we are to replace
                            outputFile.Append("[assembly: " + targetAttribute + "(\"");
                            outputFile.Append(versionValue);
                            outputFile.Append("\")]\r\n");
                            replacementMade = true;
                        } else {
                            // All lines except the one we are interested in are copied across.
                            outputFile.Append(nextLine + "\r\n");
                        }

                    }

                    if (!replacementMade) {
                        Bilge.Warning("No " + targetAttribute + " found in file, appending new one.");
                        outputFile.Append($"\r\n[assembly: {targetAttribute}(\"{versionValue}\")]\r\n");
                    }
                }
            }

            File.WriteAllText(fileName, outputFile.ToString(), Encoding.UTF8);

            Bilge.Log("The attribute " + targetAttribute + " was applied to the file " + fileName + " Successfully.");
        }
    }
}