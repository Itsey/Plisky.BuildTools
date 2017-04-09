using Plisky.Helpers;
using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plisky.Build.Tests {
    public class TestSupport {
        private UnitTestHelper uth;

        public TestSupport(UnitTestHelper newuth) {
            uth = newuth;
        }
        public string GetFileAsTemporary(string srcFile) {

            string fn = uth.NewTemporaryFileName(true);
            Bilge.VerboseLog($"Copying {srcFile} as {fn}",Environment.CurrentDirectory);
            File.Copy(srcFile, fn);
            return fn;
        }

        public  bool DoesFileContainThisText(string fn, string v) {
            string s = File.ReadAllText(fn);
            Bilge.VerboseLog("Checking file " + fn, s);
            return s.Contains(v);
        }
    }
}
