using iTombola.Cognitive.Services;
using iTombola.Core.Implementations;
using iTombola.Core.Interfaces;
using iTombola.Core.Utilities;
using iTombola.MockServices;
using iTombola.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace iTombola
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host { get; private set; }

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;

            IConfiguration myConfig = new ConfigurationBuilder()
              .SetBasePath(Path.GetDirectoryName(assemblyPath))
              .AddJsonFile("settings.json")
              .AddJsonFile("local.settings.json", true)
              .Build();

            services.AddSingleton<IConfiguration>(myConfig);
            services.AddLogging(configure => configure.AddConsole());

            //services.AddScoped<IImageAnalyzer, MockImageAnalyzer>();
            //services.AddScoped<IAudioConverter, MockAudioConverter>();

            // Cognitive Services implementation
            services.AddScoped<IAudioConverter, FileSystemAudioConverter>();
            services.AddScoped<IImageAnalyzer, ImageAnalyzer>();

            //services.AddScoped<IDescriptionsRepository, NullDescriptionsRepository>();
            services.AddScoped<IDescriptionsRepository, CsvDescriptionsRepository>();

            services.AddScoped<ITombolaService, TombolaService>();

            services.AddScoped<DialectsService>();

            services.AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await Host.StartAsync();

            var mainWindow = Host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            var config = Host.Services.GetRequiredService<IConfiguration>();

            var tempFileFolder = config["AudioConverter:AudioFilePath"];
            if (Directory.Exists(FilePathUtility.GetAbsolutePath(tempFileFolder)))
            { Directory.Delete(FilePathUtility.GetAbsolutePath(tempFileFolder), true); }

            await Host.StopAsync();



            base.OnExit(e);
        }
    }
}
