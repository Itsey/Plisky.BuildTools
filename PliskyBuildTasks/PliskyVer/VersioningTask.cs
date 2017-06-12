using Minimatch;
using System;
using System.Collections.Generic;
using System.IO;

namespace Plisky.Build {
    public class VersioningTask {
        protected Dictionary<string, List<FileUpdateType>> pendingUpdates = new Dictionary<string, List<FileUpdateType>>();
        protected CompleteVersion ver;
        protected List<string> messageLog = new List<string>();

        public delegate void LogEventHandler(object sender, LogEventArgs e);
        public event LogEventHandler Logger=null;
        public string[] LogMessages {
            get {
                return messageLog.ToArray();
            } }
        private string Test(string thisone) {
            int idx = thisone.IndexOf("]}#");
            string write = thisone.Substring(idx + 7);
            if (Logger!=null) {
                //Console.WriteLine("Calling logger 3");
                Logger(this, new LogEventArgs() {
                    Severity = "INFO",
                    Text = "LOGGER"+write
                });
            }
            Console.WriteLine("CW "+write);
            //messageLog.Add("LG " + write);
            return thisone;
        }
        public VersioningTask() {
            Bilge.CurrentTraceLevel = System.Diagnostics.TraceLevel.Verbose;
            Bilge.Log("PliskyVersioning Online.");            
            Bilge.QueueMessages = false;
            Bilge.EnableEnhancements = true;
            Bilge.CustomTagReplacementHandler = Test;
        }

        public string PersistanceValue { get; set; }
        public string VersionString { get; set; }
        public string BaseSearchDir { get; set; }

        public void AddUpdateType(string minmatchPattern, FileUpdateType updateToPerform) {
            Bilge.VerboseLog("Adding Update Type " + minmatchPattern);
            if (!pendingUpdates.ContainsKey(minmatchPattern)) {
                pendingUpdates.Add(minmatchPattern, new List<FileUpdateType>());
            }
            pendingUpdates[minmatchPattern].Add(updateToPerform);
        }

      

        public void SetAllVersioningItems(string verItemsSimple) {
            Bilge.Log("SetAllVersioningITems");
            if (verItemsSimple.Contains(Environment.NewLine)) {
                // The TFS build agent uses \n not Environment.Newline for its line separator, however unit tests use Environment.Newline
                // so replacing them with \n to make the two consistant.
                verItemsSimple = verItemsSimple.Replace(Environment.NewLine, "\n");
            }
            string[] allLines = verItemsSimple.Split(new string[] { "\n" },StringSplitOptions.RemoveEmptyEntries);
            foreach(var ln in allLines) {
                string[] parts = ln.Split('!');
                if (parts.Length!=2) {
                    throw new InvalidOperationException($"The versioning item string was in the wrong format [{ln}] ");
                }
                FileUpdateType ft =  GetFileTypeFromString(parts[1]);
                AddUpdateType(parts[0], ft);
            }
        }

        private FileUpdateType GetFileTypeFromString(string v) {
            switch (v) {
                case "ASSEMBLY":
                case "ASSEMBLY2":
                case "ASSEMBLY4": return FileUpdateType.Assembly;
                case "INFO": return FileUpdateType.AssemblyInformational;
                case "FILE": return FileUpdateType.AssemblyFile;
                case "WIX": return FileUpdateType.Wix;
                default: throw new InvalidOperationException($"The verisoning string {v} is not valid.");                    
            }
        }

        public void IncrementAndUpdateAll() {
            Bilge.VerboseLog("IncrementAndUpdateAll called");
            ValidateForUpdate();
            LoadVersioningComponent();
            Bilge.VerboseLog("Versioning Loaded ");
            ver.PerformIncrement();
            Bilge.VerboseLog("Saving");
            SaveVersioningComponent();
            Bilge.VerboseLog($"Searching {BaseSearchDir} there are {pendingUpdates.Count} pends.");
            foreach (var v in Directory.EnumerateFiles(BaseSearchDir, "*.*", SearchOption.AllDirectories)) {
                // Check every file that we have returned.
                foreach (var chk in pendingUpdates.Keys) {
                    var mm = new Minimatcher(chk, new Options { AllowWindowsPaths = true, IgnoreCase = true });
                    Bilge.VerboseLog($"Checking {chk} against {v}");
                    if (mm.IsMatch(v)) {
                        Bilge.Log("Match...");
                        // TODO Cache this and make it less loopey
                        VersionFileUpdater sut = new VersionFileUpdater(ver);
                        foreach (var updateType in pendingUpdates[chk]) {
                            Bilge.VerboseLog($"Perform update {v}");
                            sut.PerformUpdate(v, updateType);
                        }

                    }
                }
            }
            VersionString = ver.GetVersionString();
        }

        private void SaveVersioningComponent() {
            var jvg = new JsonVersionPersister(PersistanceValue);
            jvg.Persist(ver);
        }

        private void ValidateForUpdate() {
            if ((String.IsNullOrEmpty(BaseSearchDir))||(!Directory.Exists(BaseSearchDir))) {
                throw new DirectoryNotFoundException("The BaseSearchDirectory has to be specified");
            }
        }

        private void LoadVersioningComponent() {
            var jvg = new JsonVersionPersister(PersistanceValue);
            ver = jvg.GetVersion();
        }
    }
}