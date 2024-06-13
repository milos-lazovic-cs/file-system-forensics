using FileSystemForensics.Database;
using FileSystemForensics.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemForensics.Models.WinUIModels;

public class AppViewModel : BaseBind
{
    public AppViewModel()
    {
        // ...
    }

    public FileSystemMonitoringService FsMonitoringService;
    public ForensicsContext ForensicsContext;

    // All common app data
    private string sampleCommonString;
    public String SampleCommonString
    {
        get { return sampleCommonString; }
        set {
                SetProperty(ref sampleCommonString, value);
                OnPropertyChanged(nameof(SampleDerivedProperty1));
                OnPropertyChanged(nameof(SampleDerivedProperty2));
        }
    }

    public String SampleDerivedProperty1 => "return something based on SampleCommonString";

    public String SampleDerivedProperty2
    {
        get
        {
            return "Same thing as SampleDerivedProperty1, but more explicit";
        }
    }

    // This is a property that you can use for functions and internal logic… but it CAN'T be binded
    public String SampleNOTBindableProperty { get; set; }

    public void SampleFunction()
    {
        // Insert code here.

        // The function has to be with NO parameters, in order to work with simple {x:Bind} markup.
        // If your function has to access some specific data, you can create a new bindable (or non) property, just as the ones above, and memorize the data there.
    }
}