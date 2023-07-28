using BackuperCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.IO;
using Xunit;

namespace BackuperCoreTest
{
    public class SqliteTest
    {
        [Fact]
        async public void ConnectSqliteAndAddRow()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbCtx>()
                .UseSqlite(connection)
                .Options;

            using (var context = new AppDbCtx(options))
            {
                context.Database.EnsureCreated();
                
            }

            using (var context = new AppDbCtx(options))
            {
                var newItem = new TaskMetadata { Name = "test", Description = "test", SrcFolder= "folder", CreatedDate = new DateTime(), UpdatedDate = new DateTime(), Version = Guid.NewGuid().ToByteArray() };
                context.TaskMetadata.Add(newItem);
                context.SaveChanges();
            }

            using (var context = new AppDbCtx(options))
            {
                var savedItem = context.TaskMetadata.FirstOrDefault(item =>  item.Name == "test");
                Assert.Equal(savedItem.Name, "test");
            }
        }

        [Fact]
        async public void GetDbCtxtest()
        {
            var backuper = new Backuper("testDupath", ":memory:");
            using (var ctx = await backuper.EnsureTableCreatedAsync())
            {
                await ctx.TaskMetadata.AddAsync(new TaskMetadata { Name = "test2", Description = "test2", SrcFolder="folder", CreatedDate = new DateTime(), UpdatedDate = new DateTime(), Version = Guid.NewGuid().ToByteArray() }); ;
                await ctx.SaveChangesAsync();
                var result = await ctx.TaskMetadata.Where(item => item.Name == "test2").FirstOrDefaultAsync();
                Assert.Equal(result.Name, "test2");
            }
        }
    }
}