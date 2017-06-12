using Newtonsoft.Json;
using System.IO;

namespace Plisky.Build {

    public class JsonVersionPersister : VersionStorage {

        public JsonVersionPersister(string initialisationValue) : base(initialisationValue) {
        }

        protected override CompleteVersion ActualLoad() {
            string txt = File.ReadAllText(InitValue);
            return JsonConvert.DeserializeObject<CompleteVersion>(txt);
        }

        protected override void ActualPersist(CompleteVersion cv) {
            string val = JsonConvert.SerializeObject(cv);
            File.WriteAllText(InitValue, val);
        }
    }
}