using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.Models;
public class FileInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public FileAttributes Attributes { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public DirectoryInfo DirectoryInfo { get; set; }
    public bool Exists { get; set; }
    public string Extension { get; set; }
    public string FullName { get; set; }
    public string FullPath { get; set; }
    public bool IsReadOnly { get; set; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public long Length { get; set; }
    public string LinkTarget { get; set; }
    public string Name { get; set; }
    //public string OriginalPath { get; set; }

    // Navigation property
    public Guid FileSystemEventId { get; set; }
    public FileSystemEvent FileSystemEvent { get; set; }

}
