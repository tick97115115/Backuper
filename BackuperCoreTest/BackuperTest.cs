using BackuperCore;
using System.IO;

namespace BackuperCoreTest
{
    public class BackupTasktTest
    {
        // use TestFolder class create temp folder asynchronously, test BackupAsync function in BackupTask class
        [Fact]
        public async Task BackupAsyncTest()
        {
            var testFolder = new TestFolder(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            await testFolder.CreateAsync();
            var dupPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var backuper = new Backuper(dupPath, ":memory:");
            var task = await backuper.CreateTaskAsync("test", "test", testFolder.SrcPath);
            await task.BackupAsync();
            Assert.True(File.Exists(task.DupPath));
            File.Delete(task.DupPath);

        }
        // use TestFolder class create temp folder asynchronously, test RestoreAsync function in BackupTask class
        [Fact]
        public async Task RestoreAsyncTest()
        {
            var testFolder = new TestFolder(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            await testFolder.CreateAsync();
            var dupPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var backuper = new Backuper(dupPath, ":memory:");
            var task = await backuper.CreateTaskAsync("test", "test", testFolder.SrcPath);
            await task.BackupAsync();
            testFolder.Dispose();
            await task.RestoreAsync();
            Assert.True(Directory.Exists(task.SrcFolder));
            File.Delete(task.DupPath);
        }
    }

    // a class used to create a test folder asynchronously at Temp Folder with some random text file and sub-directory within it.
    public class TestFolder: IDisposable
    {
        public string SrcPath { get; set; }
        public TestFolder(string path)
        {
            SrcPath = path;
        }
        public async Task CreateAsync()
        {
            Directory.CreateDirectory(SrcPath);
            var subDir1 = Path.Combine(SrcPath, Path.GetRandomFileName());
            var subDir2 = Path.Combine(SrcPath, Path.GetRandomFileName());
            Directory.CreateDirectory(subDir1);
            Directory.CreateDirectory(subDir2);
            var testFile1 = Path.Combine(SrcPath, Path.GetRandomFileName());
            var testFile2 = Path.Combine(subDir1, Path.GetRandomFileName());
            var testFile3 = Path.Combine(subDir2, Path.GetRandomFileName());
            File.WriteAllText(testFile1, "test");
            File.WriteAllText(testFile2, "test");
            File.WriteAllText(testFile3, "test");
        }
        public void Dispose()
        {
            Directory.Delete(SrcPath, true);
            GC.SuppressFinalize(this);
        }

        ~TestFolder()
        {
            this.Dispose();
        }
    }
}
