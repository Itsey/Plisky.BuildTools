using System;

namespace Plisky.Build {
    public abstract class VersionStorage {
        protected string InitValue = null;
        protected abstract void ActualPersist(CompleteVersion cv);
        protected abstract CompleteVersion ActualLoad();

        public VersionStorage(string initialisationValue) {
            InitValue = initialisationValue;
        }

        public void Persist(CompleteVersion cv) {
            ActualPersist(cv);
        }

        public CompleteVersion GetVersion() {
            return ActualLoad();
        }
    }
}