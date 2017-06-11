
using Plisky.Plumbing;
using System;


namespace VersionerIntegrationTestClient {

    internal class Program {

        private static void Main() {
            Bilge.Log("Test Console app online");
          /*  var mvs = new MockVersionService();
            var vn = Guid.NewGuid();
            var vn2 = Guid.NewGuid();
            mvs.RegisterVersion(vn, "default", new VersionNumber(3, 1, 1, 1));
            mvs.RegisterVersion(vn2, "default", new VersionNumber(4, 1, 1, 1));

            var sut = new BuildVersioner(mvs);

            sut.VersionNumberKey = vn.ToString();
            sut.RootPath = @"D:\Files\Git2015\Plisky.net\PliskyVersioning\_Dependencies\TestData\TestFileStructure";
            sut.AssemblyFilesToUpdate = "**\\Properties\\assemblyinfo.cs";
            sut.AssemblyFilesUseBuildVersionNumber = false;
            sut.FileVersionFilesToUpdate = "**\\Properties\\assemblyinfo.cs";
            sut.FileFilesUseBuildVersionNumber = true;
            sut.IncrementAndApplyVersion();*/

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}