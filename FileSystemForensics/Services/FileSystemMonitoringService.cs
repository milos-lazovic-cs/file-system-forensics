using FileSystemForensics.Database;
using FileSystemForensics.Mappers;
using FileSystemForensics.Models;
using FileSystemForensics.ViewModels.RealTime;
using FileSystemForensics.ViewModels.Watchers;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace FileSystemForensics.Services;
public class FileSystemMonitoringService
{
    private string _directoryPath;
    public List<Watcher> Watchers = new List<Watcher>();
    private ForensicsContext _context;
    ILogger _logger;
    private readonly SemaphoreSlim _fileCreatedLock = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _fileRenamedLock = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _fileChangedLock = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _fileDeletedLock = new SemaphoreSlim(1, 1);


    public FileSystemMonitoringService(ForensicsContext context,
        ILogger<FileSystemMonitoringService> logger)
    {
        _context = context;
        _logger = logger;

        Watchers = _context.Watchers.ToList();
        InitializeWatchers();
    }


    public delegate void FileSystemEventDelegate(FileSystemEvent e);
    public static event FileSystemEventDelegate OnFileSystemEvent;
    public void RiseFileSystemEvent(FileSystemEvent e)
    {
        RealTimeViewModel.EventBuffer++;
        if (OnFileSystemEvent != null)
        {
            OnFileSystemEvent.Invoke(e);
        }
    }

    #region Public Methods

    public void InitializeWatchers()
    {
        foreach (var watcher in Watchers)
        {
            if (watcher.Enabled)
            {
                watcher.NotifyFilter =
                    NotifyFilters.FileName |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.LastWrite |
                    NotifyFilters.LastAccess;

                watcher.Created += OnFileCreated;
                watcher.Deleted += OnFileDeleted;
                watcher.Changed += OnFileChanged;
                watcher.Renamed += OnFileRenamed;

                watcher.Start();
            }
            else
                watcher.Stop();
        }
    }

    public Watcher CreateMonitor(string directoryPath)
    {
        var watcher = new Watcher(directoryPath);

        watcher.NotifyFilter = NotifyFilters.FileName |
            NotifyFilters.DirectoryName |
            NotifyFilters.LastWrite |
            NotifyFilters.LastAccess;

            //NotifyFilters.Attributes |
            //NotifyFilters.Size |
            //NotifyFilters.CreationTime;

        watcher.Created += OnFileCreated;
        watcher.Deleted += OnFileDeleted;
        watcher.Changed += OnFileChanged;
        watcher.Renamed += OnFileRenamed;

        Watchers.Add(watcher);

        _context.Watchers.Add(watcher);
        _context.SaveChanges();

        return watcher;
    }

    public List<DataGridDataItem> GetWatchersAsDataGridDataItems()
    {
        return Watchers.Select(watcher => new DataGridDataItem
        {
            WatcherId = watcher.Id,
            WatcherDirectory = watcher.Path,
            Enabled = watcher.Enabled,
            WatcherStartedDate = watcher.DateStarted,
            WatcherStoppedDate = watcher.DateStopped
        }).ToList();
    }

    public async Task<bool> StartWathcer(Guid id)
    {
        var watcher = Watchers.Find(watcher => watcher.Id == id);
        if (watcher == null)
            return false;

        watcher.Start();

        _context.Watchers.Update(watcher);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> StopWathcer(Guid id)
    {
        var watcher = Watchers.Find(watcher => watcher.Id == id);
        if (watcher == null)
            return false;

        watcher.Stop();

        _context.Watchers.Update(watcher);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteWatcher(Guid id)
    {
        var watcher = Watchers.Find(watcher => watcher.Id == id);
        if (watcher == null)
            return false;

        watcher.Stop();
        _context.Watchers.Remove(watcher);
        await _context.SaveChangesAsync();

        Watchers.Remove(watcher);

        return true;
    }

    #endregion


    #region File System Event Handlers

    // TODO: How to subscribe to event which is does not return Task but my
    // event handler does?
    private async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        await _fileCreatedLock.WaitAsync();

        try
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(e.FullPath);
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(e.FullPath);
            _logger.LogInformation($"File created: {e.FullPath}");

            FileSystemEvent fileSystemEvent = new()
            {
                Id = Guid.NewGuid(),
                WatcherId = (sender as Watcher).Id,
                WatcherChangeTypes = e.ChangeType,
                FileName = e.Name,
                FullPath = e.FullPath,
                FileInfo = fileInfo.ToFileInfo()
            };

            await _context.AddAsync(fileSystemEvent);
            await _context.SaveChangesAsync();

            RiseFileSystemEvent(fileSystemEvent);
        }
        finally
        {
            _fileCreatedLock.Release();
        }
    }

    private async void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        await _fileDeletedLock.WaitAsync();

        try
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(e.FullPath);
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(e.FullPath);
            _logger.LogInformation($"File deleted: {e.FullPath}");

            FileSystemEvent fileSystemEvent = new()
            {
                Id = Guid.NewGuid(),
                WatcherId = (sender as Watcher).Id,
                WatcherChangeTypes = e.ChangeType,
                FileName = e.Name,
                FullPath = e.FullPath,
                FileInfo = fileInfo.ToFileInfo()
            };

            await _context.FileSystemEvents.AddAsync(fileSystemEvent);
            await _context.SaveChangesAsync();

            RiseFileSystemEvent(fileSystemEvent);
        }
        finally
        {
            _fileDeletedLock.Release();
        }
    }

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        await _fileChangedLock.WaitAsync();

        try
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(e.FullPath);
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(e.FullPath);
            _logger.LogInformation($"File changed: {e.FullPath}");

            FileSystemEvent fileSystemEvent = new()
            {
                Id = Guid.NewGuid(),
                WatcherId = (sender as Watcher).Id,
                WatcherChangeTypes = e.ChangeType,
                FileName = e.Name,
                FullPath = e.FullPath,
                FileInfo = fileInfo.ToFileInfo()
            };

            await _context.FileSystemEvents.AddAsync(fileSystemEvent);
            await _context.SaveChangesAsync();

            RiseFileSystemEvent(fileSystemEvent);
        }
        finally
        {
            _fileChangedLock.Release();
        }
    }

    private async void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        await _fileRenamedLock.WaitAsync();

        try
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(e.FullPath);
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(e.FullPath);
            _logger.LogInformation($"File renamed: {e.FullPath}");

            FileSystemEvent fileSystemEvent = new()
            {
                Id = Guid.NewGuid(),
                WatcherId = (sender as Watcher).Id,
                WatcherChangeTypes = e.ChangeType,
                FileName = e.Name,
                FullPath = e.FullPath,
                FileInfo = fileInfo.ToFileInfo()
            };

            await _context.FileSystemEvents.AddAsync(fileSystemEvent);
            await _context.SaveChangesAsync();

            RiseFileSystemEvent(fileSystemEvent);
        }
        finally
        {
            _fileRenamedLock?.Release();
        }
    }

    #endregion




}
