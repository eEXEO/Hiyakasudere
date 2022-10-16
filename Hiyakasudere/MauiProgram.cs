using Microsoft.AspNetCore.Components.WebView.Maui;
using Hiyakasudere.Data;
using Microsoft.Extensions.DependencyInjection;
using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.Internal.Config;
using Hiyakasudere.Data.ExternalAPI.Safebooru;
using Hiyakasudere.Data.Internal.Data.Post;

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
		builder.Services.AddSingleton<IYanderePostService, YanderePostService>();
        builder.Services.AddSingleton<ISafebooruPostService, SafebooruPostService>();
		builder.Services.AddSingleton<IPostTranslationService, PostTranslationService>();
        builder.Services.AddSingleton<IAppConfigService, AppConfigService>();
		

        return builder.Build();
	}
}
