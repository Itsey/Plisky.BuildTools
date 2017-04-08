using Plisky.Helpers;
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
            File.Copy(srcFile, fn);
            return fn;
        }

        public  bool DoesFileContainThisText(string fn, string v) {
            return File.ReadAllText(fn).Contains(v);
        }
    }
}
