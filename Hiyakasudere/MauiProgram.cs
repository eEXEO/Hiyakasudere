using Microsoft.AspNetCore.Components.WebView.Maui;
using Hiyakasudere.Data;
using Microsoft.Extensions.DependencyInjection;
using Hiyakasudere.Data.ExternalAPI.Yandere;
using Hiyakasudere.Data.Internal.Config;

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
		builder.Services.AddSingleton<YanderePostService>();
		builder.Services.AddScoped<IAppConfigService, AppConfigService>();

        return builder.Build();
	}
}
