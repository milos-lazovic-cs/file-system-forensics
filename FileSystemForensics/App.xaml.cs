using FileSystemForensics.Configuraiton;
using FileSystemForensics.Database;
using FileSystemForensics.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FileSystemForensics;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    public static MainWindow MainWindow;
    private readonly IHost _host;
    private readonly IHostBuilder _hostBuilder;
    private readonly ILogger<App> _logger;
    private FileSystemWatcher _watcher;
    public static ServiceProvider ServiceProvider;
    public FileSystemForensics.Services.Interfaces.INavigation Navigation => MainWindow;


    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        _hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Add logging provider
                services.AddLogging(builder => builder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(context.Configuration)
                    .CreateLogger()));

                // Add db provider
                services.AddDbContext<ForensicsContext>(options =>
                    options.UseSqlite(context.Configuration.GetConnectionString("SqliteConnection"))
                    .EnableSensitiveDataLogging(), ServiceLifetime.Transient);

                // Add services
                services.AddSingleton<FileSystemMonitoringService>();

                ServiceProvider = services.BuildServiceProvider();
            })
            .ConfigureAppConfiguration((context, configuration) =>
            {
                // Add configuration
                configuration.AddJsonFile("Config/appsettings.json",
                    optional: false,
                    reloadOnChange: true);
            });


        _host = _hostBuilder.Build();



        // Update/create database
        _host.Services.UpdateDatabase();

        this.InitializeComponent();

        _logger = _host.Services.GetRequiredService<ILogger<App>>();
        _logger.LogInformation("Starting up.");
        _logger.LogInformation("{AssemblyName}, {AssemlyVersion}", Assembly.GetEntryAssembly()?.GetName().Name, Assembly.GetEntryAssembly()?.GetName().Version);
    }


    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        await _host!.StartAsync();
        var fsMonitoringService = (FileSystemMonitoringService)_host.Services.GetRequiredService(typeof(FileSystemMonitoringService));
        var forensicsContext = (ForensicsContext)_host.Services.GetRequiredService(typeof(ForensicsContext));

        MainWindow = new MainWindow(fsMonitoringService, forensicsContext);
        MainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1600, 1000));
        MainWindow.Activate();
    }



}
