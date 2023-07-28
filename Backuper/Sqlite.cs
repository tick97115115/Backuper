using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackuperCore;

namespace BackuperCore
{
    public class AppDbCtx: DbContext
    {
        public DbSet<TaskMetadata> TaskMetadata { get; set; }
        public string DbPath { get; }
        public AppDbCtx(DbContextOptions<AppDbCtx> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskMetadata>()
                .Property(p => p.Version)
                .IsConcurrencyToken();
            modelBuilder.Entity<TaskMetadata>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }

    }

    public class TaskMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SrcFolder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}

        [ConcurrencyCheck]
        public byte[] Version { get; set; }
    }
}
