using Microsoft.Extensions.Logging;
using Project_K.Services;
using Project_K.View;
using Project_K.ViewModel;

namespace Project_K
{
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
                    fonts.AddFont("fa-brands.otf", "FAB");
                    fonts.AddFont("fa-regular.otf", "FAR");
                    fonts.AddFont("fa-solid.otf", "FAS");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<RegisterService>();
            builder.Services.AddTransient<SecurityService>();
            builder.Services.AddTransient<EmailService>();
            builder.Services.AddSingleton<DatabaseUserService>();

            builder.Services.AddSingleton<IMediaPicker>(MediaPicker.Default);

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RecoverUsernameViewModel>();
            builder.Services.AddTransient<RecoverPasswordViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<RecoverUsernamePage>();

            return builder.Build();
        }
    }
}