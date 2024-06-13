using CommunityToolkit.Mvvm.ComponentModel;
using FileSystemForensics.Database;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileSystemForensics.ViewModels.DateTimeChart;

public class DateTimeLineChart : ObservableObject
{
    private readonly ForensicsContext _forensicsContext;
    public static ISeries[] Series { get; set; } = null;
    public static Axis[] XAxes { get; set; } = null;

    public DateTimeLineChart()
    {
        _forensicsContext = App.ServiceProvider.GetRequiredService<ForensicsContext>();

        ReloadSeries();

    }

    public void ReloadSeries()
    {
        var events = _forensicsContext.FileSystemEvents
            .Select(events => events.Timestamp.Date)
            .OrderByDescending(timestamp => timestamp.Date)
            .ToList();

        Dictionary<DateTime, int> eventDatesAndCounts = new();
        foreach (var e in events)
        {
            var eventDate = eventDatesAndCounts.GetValueOrDefault(e);
            if (eventDate == 0)
                eventDatesAndCounts.Add(e, 1);
            else
                eventDatesAndCounts[e] += 1;
        }

        var dateTimeObservableCollection = new ObservableCollection<DateTimePoint>();

        foreach (var e in eventDatesAndCounts)
            dateTimeObservableCollection.Add(new DateTimePoint(e.Key, e.Value));

        Series = new LineSeries<DateTimePoint>[1]
        {
            new LineSeries<DateTimePoint>
            {
                Values = dateTimeObservableCollection
            }
        };


        XAxes = new Axis[]
        {
            new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd"))
        };
    }


}
