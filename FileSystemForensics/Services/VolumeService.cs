using FileSystemForensics.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.Services;

public class VolumeService
{
    public List<Volume> Volumes { get; set; } = new List<Volume>();

    public VolumeService()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        foreach (DriveInfo drive in drives)
        {
            Volume volume = new Volume
            {
                Name = drive.Name,
                Label = drive.VolumeLabel,
                Type = drive.DriveType.ToString(),
                Format = drive.DriveFormat,
                TotalSize = drive.TotalSize,
                SpaceAvailable = drive.AvailableFreeSpace,
            };
            volume.SpaceUsed = drive.TotalSize - drive.AvailableFreeSpace;

            Volumes.Add(volume);
        }
    }



}
