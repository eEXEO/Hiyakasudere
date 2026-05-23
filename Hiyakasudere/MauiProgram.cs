using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using Hiyakasudere.Data.Internal.Data.Post;
using Blazored.Modal;
using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using Hiyakasudere.Data.Internal.Functionality.ImageUtils;
using Hiyakasudere.Data.ExternalAPI.Konachan;
using Hiyakasudere.Data.ExternalAPI.Gelbooru;
using Hiyakasudere.Data.ExternalAPI.Rule34;
using Hiyakasudere.Data.Internal.Database;
using Microsoft.EntityFrameworkCore;

namespace Hiyakasudere;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
				builder.Services.AddBlazorWebViewDeveloperTools();
		#endif
		#if WINDOWS
				builder.Services.AddTransient<IFileManager, Hiyakasudere.Platforms.Windows.Functionality.FileManager>();
		#endif

        builder.Services.AddBlazoredModal();

        // SQLite Database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "hiyakasudere.db");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));
        builder.Services.AddScoped<IFavoritesService, FavoritesService>();
        builder.Services.AddScoped<ISearchHistoryService, SearchHistoryService>();

        builder.Services.AddSingleton<IYanderePostService, YanderePostService>();
        builder.Services.AddSingleton<ISafebooruPostService, SafebooruPostService>();
		builder.Services.AddSingleton<IPostTranslationService, PostTranslationService>();
        builder.Services.AddSingleton<IKonachanPostService, KonachanPostService>();
        builder.Services.AddSingleton<IGelbooruPostService, GelbooruPostService>();
        builder.Services.AddSingleton<IRule34PostService, Rule34PostService>();
        builder.Services.AddSingleton<IAppConfigService, AppConfigService>();
        builder.Services.AddSingleton<IImageNetUtils, ImageNetUtils>();

        var app = builder.Build();

        // Ensure database is created
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        return app;
	}
}
