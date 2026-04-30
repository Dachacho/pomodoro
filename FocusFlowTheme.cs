using Tessera;
using Tessera.Styles;

namespace FocusFlow;

internal static class FocusFlowTheme
{
    // Gruvbox high contrast - using bright variants
    public const int Bg = 0x1d2021;      // dark0_hard
    public const int BgPanel = 0x282828;  // dark0
    public const int BgHighlight = 0x3c3836; // dark1

    // Bright foregrounds for visibility on transparent
    public const int Fg = 0xfbf1c7;       // light0
    public const int FgDim = 0xebdbb2;    // light1
    public const int FgMuted = 0xd5c4a1;  // light2

    // Gruvbox bright accent colors
    public const int Red = 0xfb4934;      // bright red
    public const int Green = 0xb8bb26;    // bright green
    public const int Yellow = 0xfabd2f;   // bright yellow
    public const int Blue = 0x83a598;     // bright blue
    public const int Purple = 0xd3869b;   // bright purple
    public const int Aqua = 0x8ec07c;     // bright aqua
    public const int Orange = 0xfe8019;   // bright orange

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
            Focused = Foreground(Yellow).WithBold(),
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
            Primary = Foreground(Orange).WithBold(),
            Secondary = Foreground(Aqua).WithBold()
        },
        Selection = new TesseraThemeSelectionTokens
        {
            Background = Background(BgHighlight),
            Foreground = Foreground(Fg).WithBold()
        },
        Focus = new TesseraThemeFocusTokens
        {
            Ring = Foreground(Yellow).WithBold(),
            Title = Foreground(Yellow).WithBold(),
            Border = Foreground(Yellow).WithBold(),
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
        TimerMode.LongBreak => Purple,
        _ => Red
    };

    public static TesseraStyle ModeTitle(TimerMode mode) => Foreground(ModeColor(mode)).WithBold();
    public static TesseraStyle ModeFill(TimerMode mode) => ForegroundBackground(Bg, ModeColor(mode)).WithBold();
    public static TesseraStyle ModeBar(TimerMode mode) => Foreground(ModeColor(mode)).WithBold();

    private static (byte R, byte G, byte B) Split(int color)
    {
        var r = (byte)((color >> 16) & 0xFF);
        var g = (byte)((color >> 8) & 0xFF);
        var b = (byte)(color & 0xFF);
        return (r, g, b);
    }
}
