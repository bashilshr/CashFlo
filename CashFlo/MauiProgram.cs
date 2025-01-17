using Microsoft.Extensions.Logging;
using CashFlo;
using CashFlo.Services;
using CashFlo.Services.Interface;
using Radzen;

namespace CashFlo
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
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<IUserServices, UserServices>();
            builder.Services.AddScoped<ITransactionService, TransactionServices>();
            builder.Services.AddSingleton< UserServices>();
            builder.Services.AddScoped<IDebtServices, DebtServices>();
            builder.Services.AddSingleton<TransactionServices>();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();

#endif

            return builder.Build();
        }
    }
}
