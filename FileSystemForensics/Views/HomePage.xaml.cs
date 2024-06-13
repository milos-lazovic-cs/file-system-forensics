using FileSystemForensics.Models;
using FileSystemForensics.Services;
using FileSystemForensics.ViewModels.Watchers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ctWinUI = CommunityToolkit.WinUI.UI.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileSystemForensics.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
///

public sealed partial class HomePage : Page
{
    public ObservableCollection<Volume> Volumes { get; set; } = new ObservableCollection<Volume>();
    private DataGridDataSource _dataSource = new DataGridDataSource();
    public VolumeInfo VolumeC = new VolumeInfo
    {
        Name = "C:",
        Type = "Fixed Disk",
        Size = 512,
        SpaceUsed = 127,
        SpaceAvailable = 385,
        PercentUsed = 24,
        PercendAvailable = 76
    };
    public VolumeService VolumeService { get; set; }

    public HomePage()
    {
        VolumeService = new VolumeService();
        this.InitializeComponent();
        DataContext = this;

        Volumes.Add(new Volume { Name = "Volume 1", Type = "Type A", TotalSize = 100, SpaceUsed = 50, SpaceAvailable = 50 });
        Volumes.Add(new Volume { Name = "Volume 2", Type = "Type B", TotalSize = 200, SpaceUsed = 100, SpaceAvailable = 100 });
        Volumes.Add(new Volume { Name = "Volume 2", Type = "Type B", TotalSize = 200, SpaceUsed = 100, SpaceAvailable = 100 });

        //this.DataGrid.Loaded += HomePage_Loaded;

        //DataTable transposedDataTable = TransposeDataTable((DataTable)this.DataGrid.ItemsSource);
        //this.DataGrid.ItemsSource = transposedDataTable.DefaultView;
    }

    //private DataTable TransposeDataTable(DataTable dt)
    //{
    //    DataTable dtTransposed = new DataTable();

    //    // Add columns
    //    foreach (DataRow row in dt.Rows)
    //    {
    //        dtTransposed.Columns.Add(row[0].ToString());
    //    }

    //    // Add rows
    //    for (int i = 1; i < dt.Columns.Count; i++)
    //    {
    //        DataRow newRow = dtTransposed.NewRow();
    //        newRow[0] = dt.Columns[i].ColumnName;
    //        for (int j = 0; j < dt.Rows.Count; j++)
    //        {
    //            newRow[j + 1] = dt.Rows[j][i];
    //        }
    //        dtTransposed.Rows.Add(newRow);
    //    }

    //    return dtTransposed;
    //}

    private async void HomePage_Loaded(object sender, RoutedEventArgs args)
    {
        //DataGrid.ItemsSource = await _dataSource.GetDataAsync();
    }


}

public class VolumeInfo
{
    public string Name { get; set; }
    public string Type { get; set; }
    public double Size { get; set; }
    public double SpaceUsed { get; set; }
    public double SpaceAvailable { get; set; }
    public double PercentUsed { get; set; }
    public double PercendAvailable { get; set; }
}

