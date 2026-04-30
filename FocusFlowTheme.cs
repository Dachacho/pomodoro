using Tessera;
using Tessera.Styles;

namespace FocusFlow;

internal static class FocusFlowTheme
{
    // Gruvbox Dark palette
    public const int Bg0 = 0x282828;
    public const int Bg1 = 0x3c3836;
    public const int Bg2 = 0x504945;
    public const int Bg3 = 0x665c54;
    public const int Bg4 = 0x7c6f64;
    public const int Fg0 = 0xfbf1c7;
    public const int Fg1 = 0xebdbb2;
    public const int Fg2 = 0xd5c4a1;
    public const int Fg3 = 0xbdae93;
    public const int Fg4 = 0xa89984;
    public const int Red = 0xfb4934;
    public const int Green = 0xb8bb26;
    public const int Yellow = 0xfabd2f;
    public const int Blue = 0x83a598;
    public const int Purple = 0xd3869b;
    public const int Aqua = 0x8ec07c;
    public const int Orange = 0xfe8019;

    public static TesseraTheme Default => new()
    {
        Text = new TesseraThemeTextTokens
        {
            Primary = Foreground(Fg1),
            Secondary = Foreground(Fg3),
            Muted = Foreground(Fg4),
            Inverse = ForegroundBackground(Bg0, Fg0)
        },
        Surface = new TesseraThemeSurfaceTokens
        {
            Base = Background(Bg0),
            Panel = Background(Bg1),
            Overlay = Background(Bg2)
        },
        Border = new TesseraThemeBorderTokens
        {
            Default = Foreground(Bg3),
            Strong = Foreground(Fg4),
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
            Background = Background(Bg2),
            Foreground = Foreground(Fg0).WithBold()
        },
        Focus = new TesseraThemeFocusTokens
        {
            Ring = Foreground(Yellow).WithBold(),
            Title = Foreground(Yellow).WithBold(),
            Border = Foreground(Yellow).WithBold(),
            Marker = "▸"
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

    public static TesseraStyle Chip(int fg, int bg)
    {
        return ForegroundBackground(fg, bg).WithBold();
    }

    public static int ModeColor(TimerMode mode) => mode switch
    {
        TimerMode.Work => Red,
        TimerMode.ShortBreak => Green,
        TimerMode.LongBreak => Purple,
        _ => Red
    };

    public static TesseraStyle ModeStyle(TimerMode mode) => Foreground(ModeColor(mode)).WithBold();

    public static TesseraStyle ModeFill(TimerMode mode) => ForegroundBackground(Bg0, ModeColor(mode)).WithBold();

    private static (byte R, byte G, byte B) Split(int color)
    {
        var r = (byte)((color >> 16) & 0xFF);
        var g = (byte)((color >> 8) & 0xFF);
        var b = (byte)(color & 0xFF);
        return (r, g, b);
    }
}
