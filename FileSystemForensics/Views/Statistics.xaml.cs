using FileSystemForensics.Models.WinUIModels;
using FileSystemForensics.ViewModels.Pies;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileSystemForensics.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Statistics : Page
{
    public AppViewModel ViewModel => MainWindow.Current.ViewModel;

    private const int PieChartFontSize = 16;
    public IEnumerable<ISeries> Series { get; set; } = new[] { 6, 5, 4, 3, 2 }.AsPieSeries();

    public Statistics()
    {
        this.InitializeComponent();
        this.Loaded += Statistics_Loaded;
    }

    public void Statistics_Loaded(object sender, RoutedEventArgs e)
    {
        var createdCount = ViewModel.ForensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Created)
            .Count();

        var changedCount = ViewModel.ForensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Changed)
            .Count();

        var renamedCount = ViewModel.ForensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Renamed)
            .Count();

        var deletedCount = ViewModel.ForensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Deleted)
            .Count();

        PieViewModel.Series = new List<ISeries>
        {
            new PieSeries<int>
            {
                Values = new List<int>{ createdCount },
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = PieChartFontSize,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => "Created",
                DataLabelsRotation = LiveCharts.CotangentAngle,
                Fill = new SolidColorPaint(SKColors.Salmon)
            },
            new PieSeries<int>
            {
                Values = new List<int>{ changedCount },
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = PieChartFontSize,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => "Changed",
                DataLabelsRotation = LiveCharts.CotangentAngle,
                Fill = new SolidColorPaint(SKColors.PaleVioletRed)
            },
            new PieSeries<int>
            {
                Values = new List<int>{ renamedCount },
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = PieChartFontSize,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => "Renamed",
                DataLabelsRotation = LiveCharts.CotangentAngle,
                Fill = new SolidColorPaint(SKColors.CadetBlue)
            },
            new PieSeries<int>
            {
                Values = new List<int>{ deletedCount },
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = PieChartFontSize,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => "Deleted",
                DataLabelsRotation = LiveCharts.CotangentAngle,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            }
        };

    }
}
