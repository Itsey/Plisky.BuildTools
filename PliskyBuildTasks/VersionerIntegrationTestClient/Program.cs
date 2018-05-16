
using Plisky.Build;
using Plisky.Plumbing;
using PliskyDirtyWebStorage;
using System;


namespace VersionerIntegrationTestClient {

    internal class Program {

        private static void Main() {
            Bilge.Log("Test Console app online");

            VersionStorage mvs = new PDirtyStorage("http://localhost/PSDirty/home/getversioninfo?id=ddb172c4-f37e-49d3-9841-586cb0ace48d");
            
            var sut = new VersioningTask();
            sut.InjectStore(mvs);
            sut.BaseSearchDir = @"D:\Temp\_Deleteme\aaaVersioningTestFolder";
            sut.AddUpdateType("**\\Properties\\commonassemblyinfo.cs",FileUpdateType.Assembly2);
            sut.AddUpdateType("**\\Properties\\commonassemblyinfo.cs", FileUpdateType.AssemblyInformational);
            sut.AddUpdateType("**\\Properties\\assemblyinfo.cs", FileUpdateType.AssemblyFile);

            sut.IncrementAndUpdateAll();

           /* sut.VersionNumberKey = vn.ToString();
            sut.RootPath = @"D:\Files\Git2015\Plisky.net\PliskyVersioning\_Dependencies\TestData\TestFileStructure";
            sut.AssemblyFilesToUpdate = "**\\Properties\\assemblyinfo.cs";
            sut.AssemblyFilesUseBuildVersionNumber = false;
            sut.FileVersionFilesToUpdate = "**\\Properties\\assemblyinfo.cs";
            sut.FileFilesUseBuildVersionNumber = true;
            sut.IncrementAndApplyVersion();*/


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