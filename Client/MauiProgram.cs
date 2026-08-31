using Client.Services;
using Microsoft.Extensions.Logging;

namespace Client;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5074/")
        });
        builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);
        builder.Services.AddTransient<IServerProxy, ServerProxy>();
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SignupPage>();

        return builder.Build();
    }
}
