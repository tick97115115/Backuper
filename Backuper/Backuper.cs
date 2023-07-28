using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace BackuperCore
{
    public class Backuper : IBackuper
    {
        private string _dupPath;
        private string _sqliteUrl;
        public Backuper(string dupPath, string sqliteUrl)
        {
            _dupPath = dupPath;
            _sqliteUrl = sqliteUrl;
        }
        public async Task<IBackupTask> CreateTaskAsync(string name, string description, string srcFolder)
        {
            var task = new TaskMetadata
            {
                Name = name,
                Description = description,
                SrcFolder = srcFolder,
                CreatedDate = new DateTime(),
                UpdatedDate = new DateTime(),
                Version = Guid.NewGuid().ToByteArray()
            };
            await EnsureTableCreatedAsync();
            using (var ctx = await GetOrmAsync())
            {
                await ctx.TaskMetadata.AddAsync(task);
                await ctx.SaveChangesAsync();
                return new BackupTask(
                    srcFolder,
                    Path.Combine(_dupPath, task.Id.ToString() + ".zip"),
                    ctx,
                    task);
            }
        }

        public async Task EnsureTableCreatedAsync()
        {
            var connection = new SqliteConnection($"Data Source={_sqliteUrl}");
            await connection.OpenAsync();

            var ctx = new AppDbCtx(new DbContextOptionsBuilder<AppDbCtx>().UseSqlite(connection).Options);
            await ctx.Database.EnsureCreatedAsync();
        }

        public AppDbCtx GetOrm()
        {
            var connection = new SqliteConnection($"Data Source={_sqliteUrl}");
            connection.Open();
            var ctx = new AppDbCtx(new DbContextOptionsBuilder<AppDbCtx>().UseSqlite(connection).Options);
            return ctx;
        }
        public async Task<AppDbCtx> GetOrmAsync()
        {
            var connection = new SqliteConnection($"Data Source={_sqliteUrl}");
            await connection.OpenAsync();
            var ctx = new AppDbCtx(new DbContextOptionsBuilder<AppDbCtx>().UseSqlite(connection).Options);
            return ctx;
        }

    }

    public class BackupTask: IBackupTask {
        private TaskMetadata _metadata;
        private string _srcFolder;
        private string _dupPath;
        private AppDbCtx _ctx;
        public string DupPath { get => _dupPath; }
        public string SrcFolder { get => _srcFolder; }
        public BackupTask(string srcFolder, string dupPath, AppDbCtx ctx, TaskMetadata metadata)
        {
            _srcFolder = srcFolder;
            _dupPath = Path.Combine(dupPath, metadata.Id + ".zip");
            _metadata = metadata;
            _ctx = ctx;
        }
        public Task DeleteTaskAndDuplicateAsync()
        {
            return Task.Run(() =>
            {
                if (File.Exists(_dupPath))
                {
                    File.Delete(_dupPath);
                }
                _ctx.TaskMetadata.Remove(_metadata);
                _ctx.SaveChanges();
            });
        }


        // compressing srcFolder to dupPath as zip file, and update metadata
        public Task BackupAsync()
        {
            return Task.Run(() =>
            {
                ZipFile.CreateFromDirectory(_srcFolder, _dupPath);
                _metadata.UpdatedDate = new DateTime();
                _ctx.Update(_metadata);
                _ctx.SaveChanges();
            });
        }
        // uncompressing zip file from dupPath to srcFolder, and update metadata
        public Task RestoreAsync()
        {
            return Task.Run(() =>
            {
                if (Directory.Exists(_srcFolder))
                {
                    Directory.Delete(_srcFolder, true);
                }
                try
                {
                    ZipFile.ExtractToDirectory(_dupPath, _srcFolder);
                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_srcFolder));
                    ZipFile.ExtractToDirectory(_dupPath, _srcFolder);
                }
                _metadata.UpdatedDate = new DateTime();
                _ctx.Update(_metadata);
                _ctx.SaveChanges();
            });
        }
    }
}