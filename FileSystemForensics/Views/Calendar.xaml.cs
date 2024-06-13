using CommunityToolkit.WinUI.UI;
using FileSystemForensics.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileSystemForensics.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Calendar : Page
{
    private List<DateTimeOffset> _highlightedDates = new List<DateTimeOffset>();
    public ObservableCollection<string> WatcherGuids = new ObservableCollection<string>();
    public List<DateTime> EventDateTimes = new List<DateTime>();
    private readonly ForensicsContext _forensicsContext;
    private Dictionary<DateTime, CalendarViewDayItem> _highlightedDays = new();
    private Guid ComboBoxSelectedItem { get; set; }

    public Calendar()
    {
        this.InitializeComponent();
        _forensicsContext = App.ServiceProvider.GetRequiredService<ForensicsContext>();

        this.Loaded += Calendar_Loaded;
    }

    private void Calendar_Loaded(object sender, RoutedEventArgs e)
    {
        var watcherIds = _forensicsContext.Watchers
            .Select(w => w.Id)
            .ToList();

        foreach (var id in watcherIds)
            WatcherGuids.Add(id.ToString());

        if (WatcherSelectionCombo.SelectedItem != null)
            ComboBoxSelectedItem = Guid.Parse(WatcherSelectionCombo.SelectedItem.ToString());
        else
            ComboBoxSelectedItem = Guid.Empty;

        EventDateTimes = _forensicsContext.FileSystemEvents
            .Where(e => e.WatcherId == ComboBoxSelectedItem)
            .Select(e => e.Timestamp.Date)
            .Distinct()
            .ToList();

    }

    public async void Calendar_SelectedDateChanged(object sender, CalendarViewSelectedDatesChangedEventArgs arg)
    {

        var events = _forensicsContext.FileSystemEvents
            .Where(e => e.WatcherId == Guid.Parse(WatcherSelectionCombo.SelectedItem.ToString()) &&
                    e.Timestamp.Date == FilesCalendar.SelectedDates.First().Date)
            .ToList();

        var created = events.Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Created).Count();
        var changed = events.Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Changed).Count();
        var renamed = events.Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Renamed).Count();
        var deleted = events.Where(e => e.WatcherChangeTypes == WatcherChangeTypes.Deleted).Count();

        var dialog = new ContentDialog
        {
            Title = "Event Information",
            Content = $"Created: {created}\nChanged: {changed}\nRenamed: {renamed}\nDelted: {deleted}",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        await dialog.ShowAsync();
    }

    //private void Add_Click(object sender, RoutedEventArgs e)
    //{
    //    _highlightedDates.Add(Picker.Date.Date);
    //    UpdateCalendar();
    //}


    private void Calendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
    {
        var displayedDays = FilesCalendar.FindDescendants()
            .Where(obj => obj is CalendarViewDayItem)
            .Cast<CalendarViewDayItem>();

        foreach (var day in displayedDays)
        {
            if (EventDateTimes.Contains(day.Date.Date))
            {
                day.Background = new SolidColorBrush(Colors.LightSlateGray);
                if (_highlightedDays.GetValueOrDefault(day.Date.Date) == null)
                    _highlightedDays[day.Date.Date] = day;
            }
        }
    }

    public void WatcherSelectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        var newSelectedItem = Guid.Parse(WatcherSelectionCombo.SelectedItem.ToString());
        if (ComboBoxSelectedItem == newSelectedItem)
            return;
        ComboBoxSelectedItem = newSelectedItem;

        foreach (var day in _highlightedDays.Values)
            day.Background = null;
        _highlightedDays.Clear();

        var displayedDays = FilesCalendar.FindDescendants()
            .Where(obj => obj is CalendarViewDayItem)
            .Cast<CalendarViewDayItem>();

        EventDateTimes = _forensicsContext.FileSystemEvents
            .Where(e => e.WatcherId == ComboBoxSelectedItem)
            .Select(e => e.Timestamp.Date)
            .Distinct()
            .ToList();

        foreach (var day in displayedDays)
        {
            if (EventDateTimes.Contains(day.Date.Date))
            {
                day.Background = new SolidColorBrush(Colors.LightSlateGray);
                if (_highlightedDays.GetValueOrDefault(day.Date.Date) == null)
                    _highlightedDays[day.Date.Date] = day;
            }
        }
    }

    public void Calendar_Event(object sender, object e)
    {

    }

    private void UpdateCalendar()
    {
        //var displayedDays = FilesCalendar.FindDescendants<CalendarViewDayItem>();
        var displayedDays = FilesCalendar.FindDescendants()
            .Where(obj => obj is CalendarViewDayItem)
            .Cast<CalendarViewDayItem>();

        foreach (var displayedDay in displayedDays)
        {
            if (_highlightedDates.Contains(displayedDay.Date.Date))
            {
                HighlightDay(displayedDay);
            }
            else
            {
                UnHighlightDay(displayedDay);
            }
        }
    }


    private static void HighlightDay(CalendarViewDayItem displayedDay)
    {
        displayedDay.Background = new SolidColorBrush(Colors.Red);
    }

    private static void UnHighlightDay(CalendarViewDayItem displayedDay)
    {
        displayedDay.Background = new SolidColorBrush(Colors.Transparent);
    }

}
