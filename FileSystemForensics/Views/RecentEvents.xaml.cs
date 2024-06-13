using FileSystemForensics.Models;
using FileSystemForensics.Models.WinUIModels;
using FileSystemForensics.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileSystemForensics.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RecentEvents : Page
{
    public AppViewModel ViewModel => MainWindow.Current.ViewModel;


    public RecentEvents()
    {
        this.InitializeComponent();
        this.Loaded += RecentEventsPage_Loaded;
        FileSystemMonitoringService.OnFileSystemEvent += OnFileSystemEvent_Handler;
    }


    #region Page Event Handlers

    public async void RecentEventsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ReloadList();
    }

    public async void RecentEventsButton_Click(object sender, RoutedEventArgs e)
    {
        await ReloadList();
    }

    public async void OnFileSystemEvent_Handler(FileSystemEvent e)
    {
        await ReloadList();
    }
    #endregion

    #region Private Methods

    private async Task ReloadList()
    {
        var events = await ViewModel.ForensicsContext.FileSystemEvents.ToListAsync();
        this.DispatcherQueue.TryEnqueue(() =>
        {
            RecentEventsDataGrid.ItemsSource = new ObservableCollection<FileSystemEvent>(events);
        });
    }

    #endregion

}
