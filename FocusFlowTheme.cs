using Tessera;
using Tessera.Styles;

namespace FocusFlow;

internal static class FocusFlowTheme
{
    // Bright colors for transparent terminals
    public const int Bg = 0x1a1a2e;
    public const int BgPanel = 0x16213e;
    public const int BgHighlight = 0x0f3460;

    // Bright, saturated foreground colors
    public const int Fg = 0xffffff;
    public const int FgDim = 0xcccccc;
    public const int FgMuted = 0x888888;

    // Vivid accent colors - high saturation for visibility
    public const int Cyan = 0x00fff5;
    public const int Magenta = 0xff00ff;
    public const int Green = 0x00ff88;
    public const int Red = 0xff5555;
    public const int Yellow = 0xffff00;
    public const int Blue = 0x5588ff;
    public const int Orange = 0xff9500;
    public const int Pink = 0xff77aa;

    public static TesseraTheme Default => new()
    {
        Text = new TesseraThemeTextTokens
        {
            Primary = Foreground(Fg),
            Secondary = Foreground(FgDim),
            Muted = Foreground(FgMuted),
            Inverse = ForegroundBackground(Bg, Fg)
        },
        Surface = new TesseraThemeSurfaceTokens
        {
            Base = Background(Bg),
            Panel = Background(BgPanel),
            Overlay = Background(BgHighlight)
        },
        Border = new TesseraThemeBorderTokens
        {
            Default = Foreground(FgMuted),
            Strong = Foreground(FgDim),
            Focused = Foreground(Cyan).WithBold(),
            Error = Foreground(Red).WithBold()
        },
        State = new TesseraThemeStateTokens
        {
            Success = Foreground(Green).WithBold(),
            Warning = Foreground(Yellow).WithBold(),
            Error = Foreground(Red).WithBold(),
            Info = Foreground(Blue).WithBold()
        },
        Accent = new TesseraThemeAccentTokens
        {
            Primary = Foreground(Cyan).WithBold(),
            Secondary = Foreground(Magenta).WithBold()
        },
        Selection = new TesseraThemeSelectionTokens
        {
            Background = Background(BgHighlight),
            Foreground = Foreground(Fg).WithBold()
        },
        Focus = new TesseraThemeFocusTokens
        {
            Ring = Foreground(Cyan).WithBold(),
            Title = Foreground(Cyan).WithBold(),
            Border = Foreground(Cyan).WithBold(),
            Marker = "●"
        }
    };

    public static TesseraStyle Foreground(int color)
    {
        var (r, g, b) = Split(color);
        return TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(r, g, b));
    }

    public static TesseraStyle Background(int color)
    {
        var (r, g, b) = Split(color);
        return TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(r, g, b));
    }

    public static TesseraStyle ForegroundBackground(int fg, int bg)
    {
        var (fgR, fgG, fgB) = Split(fg);
        var (bgR, bgG, bgB) = Split(bg);
        return TesseraStyle.Empty
            .WithForeground(AnsiColor.Rgb(fgR, fgG, fgB))
            .WithBackground(AnsiColor.Rgb(bgR, bgG, bgB));
    }

    public static int ModeColor(TimerMode mode) => mode switch
    {
        TimerMode.Work => Red,
        TimerMode.ShortBreak => Green,
        TimerMode.LongBreak => Magenta,
        _ => Red
    };

    public static TesseraStyle ModeTitle(TimerMode mode) => Foreground(ModeColor(mode)).WithBold();
    public static TesseraStyle ModeFill(TimerMode mode) => ForegroundBackground(0x000000, ModeColor(mode)).WithBold();
    public static TesseraStyle ModeBar(TimerMode mode) => Foreground(ModeColor(mode)).WithBold();

    private static (byte R, byte G, byte B) Split(int color)
    {
        var r = (byte)((color >> 16) & 0xFF);
        var g = (byte)((color >> 8) & 0xFF);
        var b = (byte)(color & 0xFF);
        return (r, g, b);
    }
}
