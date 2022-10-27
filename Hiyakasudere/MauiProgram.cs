using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using Hiyakasudere.Data.Internal.Data.Post;
using Blazored.Modal;
using Hiyakasudere.Data.Internal.MultiplatformInterfaces;
using Hiyakasudere.Data.Internal.Functionality.ImageUtils;

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

		builder.Services.AddSingleton<IYanderePostService, YanderePostService>();
        builder.Services.AddSingleton<ISafebooruPostService, SafebooruPostService>();
		builder.Services.AddSingleton<IPostTranslationService, PostTranslationService>();
        builder.Services.AddSingleton<IAppConfigService, AppConfigService>();
        builder.Services.AddSingleton<IImageNetUtils, ImageNetUtils>();
        



        return builder.Build();
	}
}
