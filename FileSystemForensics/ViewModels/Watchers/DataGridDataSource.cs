using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Storage;

namespace FileSystemForensics.ViewModels.Watchers;
[Bindable]
public class DataGridDataSource
{
    private static ObservableCollection<DataGridDataItem> _items;
    private static List<Guid> _watchers;
    private static CollectionViewSource groupedItems;
    private string _cachedSortedColumn = string.Empty;

    // Loading data
    public async Task<IEnumerable<DataGridDataItem>> GetDataAsync()
    {
        var tmpList = new List<DataGridDataItem>
        {
            new DataGridDataItem
            {
                WatcherId = Guid.NewGuid(),
                WatcherStartedDate = DateTime.Now,
                WatcherDirectory = "C://Windows/User1/Documents/",
                WatcherStoppedDate = DateTime.Now,
            },
            new DataGridDataItem
            {
                WatcherId = Guid.NewGuid(),
                WatcherStartedDate = DateTime.Now,
                WatcherDirectory = "C://Windows/User1/Downloads/",
                WatcherStoppedDate = DateTime.Now,
            },
            new DataGridDataItem
            {
                WatcherId = Guid.NewGuid(),
                WatcherStartedDate = DateTime.Now,
                WatcherDirectory = "C://Windows/User1/Images/",
                WatcherStoppedDate = DateTime.Now
            }
        };

        return tmpList;
    }

    // Load mountains into separate collection for use in combobox column
    public async Task<IEnumerable<Guid>> GetWatchers()
    {
        if (_items == null || !_items.Any())
        {
            await GetDataAsync();
        }

        _watchers = _items?.OrderBy(x => x.WatcherStartedDate).Select(x => x.WatcherId).Distinct().ToList();

        return _watchers;
    }

    // Sorting implementation using LINQ
    public string CachedSortedColumn
    {
        get
        {
            return _cachedSortedColumn;
        }

        set
        {
            _cachedSortedColumn = value;
        }
    }

    public ObservableCollection<DataGridDataItem> SortData(string sortBy, bool ascending)
    {
        _cachedSortedColumn = sortBy;
        switch (sortBy)
        {
            case "WatcherId":
                if (ascending)
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherId ascending
                                                                      select item);
                }
                else
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherId descending
                                                                      select item);
                }

            case "WatcherDuration":
                if (ascending)
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherStoppedDate ascending
                                                                      select item);
                }
                else
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherStoppedDate descending
                                                                      select item);
                }

            case "WatcherCreatedDate":
                if (ascending)
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherStartedDate ascending
                                                                      select item);
                }
                else
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherStartedDate descending
                                                                      select item);
                }

            case "WatcherDirectory":
                if (ascending)
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherDirectory ascending
                                                                      select item);
                }
                else
                {
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      orderby item.WatcherDirectory descending
                                                                      select item);
                }

        }

        return _items;
    }

    // Grouping implementation using LINQ
    public CollectionViewSource GroupData(string groupBy = "WatcherCreatedDate")
    {
        ObservableCollection<GroupInfoCollection<DataGridDataItem>> groups = new ObservableCollection<GroupInfoCollection<DataGridDataItem>>();
        var query = from item in _items
                    orderby item
                    group item by item.WatcherStartedDate into g
                    select new { GroupName = g.Key, Items = g };


        foreach (var g in query)
        {
            GroupInfoCollection<DataGridDataItem> info = new GroupInfoCollection<DataGridDataItem>();
            info.Key = g.GroupName;
            foreach (var item in g.Items)
            {
                info.Add(item);
            }

            groups.Add(info);
        }

        groupedItems = new CollectionViewSource();
        groupedItems.IsSourceGrouped = true;
        groupedItems.Source = groups;

        return groupedItems;
    }

    public class GroupInfoCollection<T> : ObservableCollection<T>
    {
        public object Key { get; set; }

        public new IEnumerator<T> GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }

    // Filtering implementation using LINQ
    public enum FilterOptions
    {
        All = -1,
        DurationLow = 0,
        DurationMid = 1,
        DurationHigh = 2
    }

    public ObservableCollection<DataGridDataItem> FilterData(FilterOptions filterBy)
    {
        switch (filterBy)
        {
            case FilterOptions.All:
                return new ObservableCollection<DataGridDataItem>(_items);

            case FilterOptions.DurationLow:
                return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                  where item.WatcherStoppedDate < DateTime.Now
                                                                  select item);

            case FilterOptions.DurationMid:
                return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                  where item.WatcherStoppedDate > DateTime.Now
                                                                  select item);

            case FilterOptions.DurationHigh:
                return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                  where item.WatcherStoppedDate > DateTime.Now
                                                                  select item);
        }

        return _items;
    }

    public ObservableCollection<DataGridDataItem> SearchData(string queryText)
    {
        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                          where item.WatcherDirectory.Contains(queryText, StringComparison.InvariantCultureIgnoreCase)
                                                          select item);
    }
}