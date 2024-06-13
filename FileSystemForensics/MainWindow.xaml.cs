using FileSystemForensics.Database;
using FileSystemForensics.Models.WinUIModels;
using FileSystemForensics.Services;
using FileSystemForensics.Services.Interfaces;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileSystemForensics;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public partial class MainWindow : Window
{
    public AppViewModel ViewModel { get; set; } = new AppViewModel();
    public static MainWindow Current { get; set; }

    public MainWindow(FileSystemMonitoringService fsMonitoring,
        ForensicsContext forensicsContext)
    {
        //FsMonitoringService = fsMonitoring;
        ViewModel.FsMonitoringService = fsMonitoring;
        ViewModel.ForensicsContext = forensicsContext;

        this.InitializeComponent();
        Current = this;
    }

    private void myButton_Click(object sender, RoutedEventArgs e)
    {
        //myButton.Content = "Clicked";

        //var _fsMonitor = FsMonitoringService.CreateMonitor("C:\\Users\\Milos Lazovic\\Desktop\\");

    }

}


// Tab 1: Home - Lista postojecih watchera, kad su kreirani i koliko dugo rade, koje direktorijume nadgledaju
// Tab 2: Kreiranje watcher-a, selekcija direktorijuma
// Tab 3: Graficki prikaz
// Tab 4: Recent events
// Dark mode, background mode, add to hidden icons