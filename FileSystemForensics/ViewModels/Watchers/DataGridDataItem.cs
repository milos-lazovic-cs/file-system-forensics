using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.ViewModels.Watchers;

public class DataGridDataItem : INotifyDataErrorInfo, IComparable
{
    private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    public Guid WatcherId { get; set; }
    public DateTime? WatcherStartedDate { get; set; }
    public DateTime? WatcherStoppedDate { get; set; }
    public string WatcherDirectory { get; set; }
    public bool Enabled { get; set; }


    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;



    bool INotifyDataErrorInfo.HasErrors
    {
        get
        {
            return _errors.Keys.Count > 0;
        }
    }

    IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
    {
        if (propertyName == null)
        {
            propertyName = string.Empty;
        }

        if (_errors.Keys.Contains(propertyName))
        {
            return _errors[propertyName];
        }
        else
        {
            return null;
        }
    }

    public int CompareTo(object obj)
    {
        return WatcherId == (obj as DataGridDataItem).WatcherId ? 1 : -1;
    }
}