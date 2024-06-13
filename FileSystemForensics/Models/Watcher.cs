using System;
using System.Collections.Generic;
using System.IO;

namespace FileSystemForensics.Models;


public class Watcher : FileSystemWatcher
{
    public Guid Id { get; set; }
    public DateTime? DateStarted { get; set; }
    public DateTime? DateStopped { get; set; }

    // Navigation to dependent entity
    public List<FileSystemEvent> FileSystemEvents { get; set; }

    public bool Enabled
    {
        get { return base.EnableRaisingEvents; }
        set { base.EnableRaisingEvents = value; }
    }

    public Watcher()
    {

    }

    public Watcher(string directoryPath)
        : base(directoryPath)
    {
        Id = Guid.NewGuid();
    }

    public void Start()
    {
        DateStopped = null;
        DateStarted = DateTime.Now;
        Enabled = true;
    }

    public void Stop()
    {
        DateStopped = DateTime.Now;
        Enabled = false;
    }

}
