using System;
using System.IO;

namespace FileSystemForensics.Models;
public class FileSystemEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public WatcherChangeTypes WatcherChangeTypes { get; set; }
    public string FileName { get; set; }
    public string OldName { get; set; }
    public string FullPath { get; set; }
    public string OldFullPath { get; set; }

    // Foreign key and navigation to principal entity
    public Guid WatcherId { get; set; }
    public Watcher Watcher { get; set; }
    public FileInfo FileInfo { get; set; }
}
