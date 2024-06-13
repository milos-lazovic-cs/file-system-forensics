using CommunityToolkit.Mvvm.ComponentModel;
using FileSystemForensics.Database;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.ViewModels.RealTime;
public class RealTimeViewModel : ObservableObject
{
    private readonly ForensicsContext _forensicsContext;

    private readonly Random _random = new();
    private readonly List<DateTimePoint> _values = new();
    private readonly DateTimeAxis _customAxis;
    public ObservableCollection<ISeries> Series { get; set; }

    private static object _eventBufferLock = new object();
    private static int _eventBuffer;
    public static int EventBuffer
    {
        get
        {
            lock (_eventBufferLock)
            {
                return _eventBuffer;
            }
        }
        set
        {
            lock (_eventBufferLock)
            {
                _eventBuffer = value;
            }
        }
    }

    public Axis[] XAxes { get; set; }
    public object Sync { get; } = new object();
    public bool IsReading { get; set; } = true;

    public RealTimeViewModel()
    {
        _forensicsContext = App.ServiceProvider.GetRequiredService<ForensicsContext>();

        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = _values,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };

        _customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
        {
            CustomSeparators = GetSeparators(),
            AnimationsSpeed = TimeSpan.FromMilliseconds(0),
            SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
        };

        XAxes = new Axis[] { _customAxis };

        _ = ReadData();

    }

    private async Task ReadData()
    {
        // to keep this sample simple, we run the next infinite loop
        // in a real application you should stop the loop/task when the view is disposed

        while (IsReading)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Because we are updating the chart from a different thread
            // we need to use a lock to access the chart data.
            // this is not necessary if your changes are made in the UI thread.
            lock (Sync)
            {
                _values.Add(new DateTimePoint(DateTime.Now, EventBuffer));
                EventBuffer = 0;
                if (_values.Count > 60*6) _values.RemoveAt(0);

                // we need to update the separators every time we add a new point
                _customAxis.CustomSeparators = GetSeparators();
            }
        }
    }

    private double[] GetSeparators()
    {
        var now = DateTime.Now;

        return new double[]
        {
            now.AddMinutes(-45).Ticks,
            now.AddMinutes(-30).Ticks,
            now.AddMinutes(-20).Ticks,
            now.AddMinutes(-10).Ticks,
            now.AddMinutes(-1).Ticks,
            now.Ticks
        };
    }

    private static string Formatter(DateTime date)
    {
        var minsAgo = (DateTime.Now - date).TotalMinutes;

        return minsAgo < 1
            ? "now"
            : $"{minsAgo:N0}min ago";
    }

}
