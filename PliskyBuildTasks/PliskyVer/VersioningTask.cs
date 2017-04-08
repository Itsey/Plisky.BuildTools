using Minimatch;
using System;
using System.Collections.Generic;
using System.IO;

namespace Plisky.Build {
    public class VersioningTask {
        Dictionary<string, List<FileUpdateType>> pendingUpdates = new Dictionary<string, List<FileUpdateType>>();
        CompleteVersion ver;

        public VersioningTask() {
        }

        public string PersistanceValue { get; set; }
        public string VersionString { get; set; }
        public string BaseSearchDir { get; set; }

        public void AddUpdateType(string minmatchPattern, FileUpdateType updateToPerform) {
            if (!pendingUpdates.ContainsKey(minmatchPattern)) {
                pendingUpdates.Add(minmatchPattern, new List<FileUpdateType>());
            }
            pendingUpdates[minmatchPattern].Add(updateToPerform);
        }

        public void IncrementAndUpdateAll() {
            ValidateForUpdate();
            LoadVersioningComponent();
            ver.PerformIncrement();
            SaveVersioningComponent();
            foreach (var v in Directory.EnumerateFiles(BaseSearchDir, "*.*", SearchOption.AllDirectories)) {
                // Check every file that we have returned.
                foreach (var chk in pendingUpdates.Keys) {
                    var mm = new Minimatcher(chk, new Options { AllowWindowsPaths = true });
                    if (mm.IsMatch(v)) {
                        // TODO Cache this and make it less loopey
                        VersionFileUpdater sut = new VersionFileUpdater(ver);
                        foreach (var updateType in pendingUpdates[chk]) {

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