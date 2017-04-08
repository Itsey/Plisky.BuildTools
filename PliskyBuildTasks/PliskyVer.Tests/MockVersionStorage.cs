using System;

namespace Plisky.Build.Tests {
    internal class MockVersionStorage : VersionStorage {
        

        public MockVersionStorage(string v): base(v) {
        
        }

        public bool PersistWasCalled { get; private set; }
        public string VersionStringPersisted { get; private set; }

        protected override CompleteVersion ActualLoad() {
            throw new NotImplementedException();
        }

        protected override void ActualPersist(CompleteVersion cv) {
            PersistWasCalled = true;
            VersionStringPersisted = cv.GetVersionString();
        }
    }
}