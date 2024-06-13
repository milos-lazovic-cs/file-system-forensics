using FileSystemForensics.Database;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace FileSystemForensics.ViewModels.Bars;
public class BarViewModel : ObservableObject
{
    private readonly ForensicsContext _forensicsContext;
    private const int TopNCagegories = 5;

    public BarViewModel()
    {
        _forensicsContext = App.ServiceProvider.GetRequiredService<ForensicsContext>();

        ReloadSeries();
    }

    public static ISeries[] Series { get; set; } = null;
    public static Axis[] XAxes { get; set; } = null;

    public void ReloadSeries()
    {
        var types = _forensicsContext.FileSystemEvents
            .Include(e => e.FileInfo)
            .Select(fi => fi.FileInfo.Extension)
            .ToList();

        Dictionary<string, int> typesMap = new();

        foreach (var type in types)
        {
            var typeCount = typesMap.GetValueOrDefault(type, 0);
            if (typeCount > 0)
                typesMap[type] += 1;
            else
                typesMap.Add(type, 1);
        }

        var topExtensions = typesMap.OrderByDescending(t => t.Value)
            .Take(TopNCagegories)
            .Select(t => t.Key)
            .ToList();

        var topCounts = typesMap.OrderByDescending(t => t.Value)
            .Take(TopNCagegories)
            .Select(t => t.Value)
            .ToList();

        Series = new ColumnSeries<int>[1]
        {
            new ColumnSeries<int>
            {
                Name = "Extensions",
                Values = topCounts
            }
        };

        XAxes = new Axis[]
        {
            new Axis
            {
                Labels = topExtensions,
                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true,
                ForceStepToMin = true,
                MinStep = 1
            }
        };
    }
}
