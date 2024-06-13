using CommunityToolkit.Mvvm.ComponentModel;
using FileSystemForensics.Database;
using FileSystemForensics.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.ViewModels.Pies;
public class PieViewModel : ObservableObject
{
    private readonly ForensicsContext _forensicsContext;
    private const int PieChartFontSize = 16;

    public PieViewModel()
    {
        _forensicsContext = App.ServiceProvider.GetRequiredService<ForensicsContext>();

        ReloadSeries();
    }

    public static IEnumerable<ISeries> Series { get; set; } = null;

    public void ReloadSeries()
    {
        var createdCount = _forensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Created)
            .Count();

        var changedCount = _forensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Changed)
            .Count();

        var renamedCount = _forensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Renamed)
            .Count();

        var deletedCount = _forensicsContext.FileSystemEvents
            .Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Deleted)
            .Count();

        Series = new List<ISeries>
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
