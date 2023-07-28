using Microsoft.EntityFrameworkCore;


namespace BackuperCore
{
    public interface IBackuper
    {
        public Task EnsureTableCreatedAsync();
        public Task<IBackupTask> CreateTaskAsync(string name, string description, string srcFolder);
        public AppDbCtx GetOrm();
        public Task<AppDbCtx> GetOrmAsync();
    }
    public interface IBackupTask
    {
        public string DupPath { get; }
        public string SrcFolder { get; }
        public Task DeleteTaskAndDuplicateAsync();
        public Task BackupAsync();
        public Task RestoreAsync();
    }
}
