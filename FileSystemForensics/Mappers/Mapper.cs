using System;
using System.IO;
using ForensicsDirectoryInfo = FileSystemForensics.Models.DirectoryInfo;
using ForensicsFileInfo = FileSystemForensics.Models.FileInfo;

namespace FileSystemForensics.Mappers;
public static class Mapper
{
    public static ForensicsFileInfo ToFileInfo(this System.IO.FileInfo ioFileInfo)
    {
        var fileInfo = new ForensicsFileInfo
        {
            Id = Guid.NewGuid(),
            Attributes = ioFileInfo.Attributes,
            CreationTime = ioFileInfo.CreationTime,
            CreationTimeUtc = ioFileInfo.CreationTimeUtc,
            DirectoryInfo = ioFileInfo.Directory.ToDirectoryInfo(),
            Exists = ioFileInfo.Exists,
            Extension = ioFileInfo.Extension,
            FullName = ioFileInfo.FullName,
            FullPath = ioFileInfo.FullName,
            IsReadOnly = ioFileInfo.IsReadOnly,
            Name = ioFileInfo.Name,
            LastAccessTime = ioFileInfo.LastAccessTime,
            LastAccessTimeUtc = ioFileInfo.LastAccessTimeUtc,
            LastWriteTime = ioFileInfo.LastWriteTime,
            LastWriteTimeUtc = ioFileInfo.LastWriteTimeUtc,
            Length = ((ioFileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory &&
                      (int)ioFileInfo.Attributes != -1) ?                                                       // Note: If object is deleted FileInfo.Attributes has value -1.
                ioFileInfo.Length : 0,
            LinkTarget = ioFileInfo.LinkTarget
        };

        return fileInfo;
    }

    public static ForensicsDirectoryInfo ToDirectoryInfo(this System.IO.DirectoryInfo ioDirectoryInfo)
    {
        return new ForensicsDirectoryInfo
        {
            Id = Guid.NewGuid(),
            FullPath = ioDirectoryInfo.FullName,
            OriginalPath = ioDirectoryInfo.FullName,
            Attributes = ioDirectoryInfo.Attributes,
            LinkTarget = ioDirectoryInfo.LinkTarget,
            LastWriteTimeUtc = ioDirectoryInfo.LastWriteTimeUtc,
            LastWriteTime = ioDirectoryInfo.LastWriteTime,
            CreationTimeUtc = ioDirectoryInfo.CreationTimeUtc,
            CreattionTime = ioDirectoryInfo.CreationTime,
            Exists = ioDirectoryInfo.Exists,
            Extension = ioDirectoryInfo.Extension,
            FullName = ioDirectoryInfo.FullName,
            LastAccessTime = ioDirectoryInfo.LastAccessTime,
            LastAccessTimeUtc = ioDirectoryInfo.LastAccessTimeUtc,
            Name = ioDirectoryInfo.Name
        };
    }
}
