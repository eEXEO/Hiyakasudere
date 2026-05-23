using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.Internal.Database;
using Hiyakasudere.Data.Internal.Data.Post;
using Hiyakasudere.Data.Internal.Functionality.ImageUtils;
using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using Hiyakasudere.Data.ExternalAPI.Konachan;
using Hiyakasudere.Data.ExternalAPI.Gelbooru;
using Hiyakasudere.Data.ExternalAPI.Rule34;
using Hiyakasudere.ViewModels;
using Hiyakasudere.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hiyakasudere;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Ensure database created
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // Database
        var dbPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Hiyakasudere", "hiyakasudere.db");
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)!);
        services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

        // Platform services
        services.AddSingleton<Data.Internal.MultiplatformInterfaces.IFileManager, Data.Internal.MultiplatformInterfaces.CrossPlatformFileManager>();

        // Data services
        services.AddSingleton<IAppConfigService, AppConfigService>();
        services.AddSingleton<IYanderePostService, YanderePostService>();
        services.AddSingleton<ISafebooruPostService, SafebooruPostService>();
        services.AddSingleton<IKonachanPostService, KonachanPostService>();
        services.AddSingleton<IGelbooruPostService, GelbooruPostService>();
        services.AddSingleton<IRule34PostService, Rule34PostService>();
        services.AddSingleton<IPostTranslationService, PostTranslationService>();
        services.AddSingleton<IImageNetUtils, ImageNetUtils>();
        services.AddScoped<IFavoritesService, FavoritesService>();
        services.AddScoped<ISearchHistoryService, SearchHistoryService>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<BrowseViewModel>();
        services.AddTransient<FavoritesViewModel>();
        services.AddTransient<SettingsViewModel>();
    }
}
