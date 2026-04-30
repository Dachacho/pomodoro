using Tessera;
using FocusFlow;

var app = TesseraApplication.CreateBuilder()
    .UseApp<FocusFlowApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = FocusFlowTheme.Default;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "focusflow",
            EnableFocusReporting = true
        };
    })
    .Build();

await app.RunAsync();
