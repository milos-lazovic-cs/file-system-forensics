using FileSystemForensics.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using ForensicsFileInfo = FileSystemForensics.Models.FileInfo;
using ForensicsDirectoryInfo = FileSystemForensics.Models.DirectoryInfo;

namespace FileSystemForensics.Database;
public class ForensicsContext : DbContext
{
    public ForensicsContext()
    {

    }

    public ForensicsContext(DbContextOptions<ForensicsContext> options)
        : base(options)
    {

    }

    public virtual DbSet<FileSystemEvent> FileSystemEvents { get; set; }
    public virtual DbSet<ForensicsFileInfo> FileInfos { get; set; }
    public virtual DbSet<ForensicsDirectoryInfo> DirectoryInfos { get; set; }
    public virtual DbSet<Watcher> Watchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Watcher>(entity =>
        {
            entity.Ignore(x => x.Site);
            entity.Ignore(x => x.EnableRaisingEvents);
            entity.Ignore(x => x.Container);
            entity.Ignore(x => x.FileSystemEvents);
            entity.Ignore(x => x.Filter);
            entity.Ignore(x => x.Filters);
            entity.Ignore(x => x.IncludeSubdirectories);
            entity.Ignore(x => x.InternalBufferSize);
            entity.Ignore(x => x.NotifyFilter);
            entity.Ignore(x => x.SynchronizingObject);
        });

        modelBuilder.Entity<FileSystemEvent>(entity =>
        {
            entity.Property(e => e.WatcherChangeTypes)
                .HasConversion(wct => wct.ToString(),
                    wct => (WatcherChangeTypes)Enum.Parse(typeof(WatcherChangeTypes), wct));
        });
    }

}
