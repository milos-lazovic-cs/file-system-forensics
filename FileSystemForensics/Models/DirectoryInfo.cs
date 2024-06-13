using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.Models;
public class DirectoryInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public FileAttributes Attributes { get; set; }
    public DateTime CreattionTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public bool Exists { get; set; }
    public string Extension { get; set; }
    public string FullName { get; set; }
    public string FullPath { get; set; }
    public DateTime LastAccessTime {  get; set; }
    public DateTime LastAccessTimeUtc { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public string LinkTarget { get; set; }
    public string Name { get; set; }
    public string OriginalPath { get; set; }


    // Navigation property
    public Guid FileInfoId { get; set; }
    public FileInfo FileInfo { get; set; }



}
