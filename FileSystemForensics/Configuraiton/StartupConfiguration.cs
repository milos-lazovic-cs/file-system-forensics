using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FileSystemForensics.Database;

namespace FileSystemForensics.Configuraiton;
internal static class StartupConfiguration
{

    public static void UpdateDatabase(this IServiceProvider services)
    {
        using (var context = services.GetService<ForensicsContext>())
        {
            context?.Database.EnsureCreated();
        }
    }
}
