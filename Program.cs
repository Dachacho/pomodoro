using Tessera;
using FocusFlow;

var app = TesseraApplication.CreateBuilder()
    .UseApp<FocusFlowApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = FocusFlowTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "FocusFlow - Pomodoro Timer",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .Build();

await app.RunAsync();
