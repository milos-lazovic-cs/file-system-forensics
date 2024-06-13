using FileSystemForensics.Models;
using FileSystemForensics.Models.WinUIModels;
using FileSystemForensics.ViewModels.Watchers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using ctWinUI = CommunityToolkit.WinUI.UI.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileSystemForensics.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Watchers : Page
{
    private DataGridDataSource _dataSource = new DataGridDataSource();
    private string _grouping;
    public AppViewModel ViewModel => MainWindow.Current.ViewModel;

    public Watchers()
    {
        this.InitializeComponent();
        this.Loaded += WatchersPage_Loaded;
    }

    private async void WatchersPage_Loaded(object sender, RoutedEventArgs e)
    {
        DataGrid.ItemsSource = new ObservableCollection<DataGridDataItem>(ViewModel.FsMonitoringService.GetWatchersAsDataGridDataItems());
        DataGrid.Columns[0].SortDirection = ctWinUI.DataGridSortDirection.Ascending;
    }

    public async void CreateWatcher_Click(object sender, RoutedEventArgs e)
    {
        var newMonitor = ViewModel.FsMonitoringService.CreateMonitor(DirectoryPathTextBox.Text);

        ReloadDataGrid();
    }

    public void ReloadDataGrid()
    {
        DataGrid.ItemsSource = new ObservableCollection<DataGridDataItem>(ViewModel.FsMonitoringService.GetWatchersAsDataGridDataItems());
    }

    public async void StartWatcherButton_Click(object sender, RoutedEventArgs e)
    {
        var watcherId = (Guid)((Button)sender).Tag;

        await ViewModel.FsMonitoringService.StartWathcer(watcherId);

        ReloadDataGrid();
    }

    public async void StopWatcherButton_Click(object sender, RoutedEventArgs e)
    {
        var watcherId = (Guid)((Button)sender).Tag;

        await ViewModel.FsMonitoringService.StopWathcer(watcherId);

        ReloadDataGrid();
    }

    public async void DeleteWatcherButton_Click(object sender, RoutedEventArgs e)
    {
        var watcherId = (Guid)((Button)sender).Tag;

        await ViewModel.FsMonitoringService.DeleteWatcher(watcherId);

        ReloadDataGrid();
    }

    public async void BrowseDirectiroy_Click(object sender, RoutedEventArgs e)
    {
        FolderPicker fileOpenPicker = new()
        {
            ViewMode = PickerViewMode.Thumbnail
        };

        nint windowHandle = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);
        var directory = await fileOpenPicker.PickSingleFolderAsync();


        if (directory != null)
        {
            DirectoryPathTextBox.Text = directory.Path;
        }
    }

    public async Task<IEnumerable<DataGridDataItem>> GetDataAsync()
    {
        var tmpList = new List<DataGridDataItem>
        {
            new DataGridDataItem
            {
                WatcherId = Guid.NewGuid(),
                WatcherStartedDate = DateTime.Now,
                WatcherDirectory = "C://Windows/User1/Documents/",
                WatcherStoppedDate = DateTime.Now
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

    private void DataGrid_Sorting(object sender, ctWinUI.DataGridColumnEventArgs e)
    {
        // Add sorting indicator, and sort
        var isAscending = e.Column.SortDirection == null || e.Column.SortDirection == ctWinUI.DataGridSortDirection.Descending;
        DataGrid.ItemsSource = _dataSource.SortData(e.Column.Tag.ToString(), isAscending);
        e.Column.SortDirection = isAscending
            ? ctWinUI.DataGridSortDirection.Ascending
            : ctWinUI.DataGridSortDirection.Descending;

        // Remove sorting indicators from other columns
        foreach (var column in DataGrid.Columns)
        {
            if (column.Tag.ToString() != e.Column.Tag.ToString())
            {
                column.SortDirection = null;
            }
        }
    }

    private void DataGrid_LoadingRowGroup(object sender, ctWinUI.DataGridRowGroupHeaderEventArgs e)
    {
        ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
        DataGridDataItem item = group.GroupItems[0] as DataGridDataItem;
        if (_grouping == "WatcherId")
        {
            e.RowGroupHeader.PropertyValue = item.WatcherId.ToString();
        }
        else
        {
            e.RowGroupHeader.PropertyValue = item.WatcherDirectory;
        }
    }

    private void FilterRankLow_Click(object sender, RoutedEventArgs e)
    {
        DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.DurationLow);
    }

    private void FilterRankHigh_Click(object sender, RoutedEventArgs e)
    {
        DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.DurationMid);
    }

    private void FilterHeightLow_Click(object sender, RoutedEventArgs e)
    {
        DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.DurationHigh);
    }

    private void FilterHeightHigh_Click(object sender, RoutedEventArgs e)
    {
        DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.DurationHigh);
    }

    private void FilterClear_Click(object sender, RoutedEventArgs e)
    {
        DataGrid.ItemsSource = _dataSource.FilterData(DataGridDataSource.FilterOptions.All);
    }

    private void ApplyGrouping(string grouping)
    {
        _grouping = grouping;
        DataGrid.RowGroupHeaderPropertyNameAlternative = _grouping;
        DataGrid.ItemsSource = _dataSource.GroupData(_grouping).View;
    }

    private void GroupByParentMountain_Click(object sender, RoutedEventArgs e)
    {
        ApplyGrouping("Parent_Mountain");
    }

    private void GroupByRange_Click(object sender, RoutedEventArgs e)
    {
        ApplyGrouping("Range");
    }

    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        DataGrid.ItemsSource = _dataSource.SearchData(args.QueryText);
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        DataGrid.ItemsSource = _dataSource.SearchData(SearchBox.Text);
    }
}
