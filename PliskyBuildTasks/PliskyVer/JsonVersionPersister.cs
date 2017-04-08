using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
