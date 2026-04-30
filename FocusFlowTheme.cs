using Tessera;
using Tessera.Styles;

namespace FocusFlow;

internal static class FocusFlowTheme
{
    public static TesseraTheme Default => new()
    {
        Text = new TesseraThemeTextTokens
        {
            Primary = Foreground(0xF0F6FC),
            Secondary = Foreground(0x8B949E),
            Muted = Foreground(0x484F58),
            Inverse = Foreground(0x0D1117)
        },
        Surface = new TesseraThemeSurfaceTokens
        {
            Base = Background(0x0D1117),
            Panel = Background(0x161B22),
            Overlay = Background(0x21262D)
        },
        Border = new TesseraThemeBorderTokens
        {
            Default = Foreground(0x30363D),
            Strong = Foreground(0x58A6FF),
            Focused = Foreground(0x58A6FF).WithBold(),
            Error = Foreground(0xF85149).WithBold()
        },
        State = new TesseraThemeStateTokens
        {
            Success = Foreground(0x3FB950).WithBold(),
            Warning = Foreground(0xD29922).WithBold(),
            Error = Foreground(0xF85149).WithBold(),
            Info = Foreground(0x58A6FF).WithBold()
        },
        Accent = new TesseraThemeAccentTokens
        {
            Primary = Foreground(0xFF6B6B).WithBold(),
            Secondary = Foreground(0x4ECDC4).WithBold()
        },
        Selection = new TesseraThemeSelectionTokens
        {
            Background = Background(0x388BFD26),
            Foreground = Foreground(0xF0F6FC).WithBold()
        },
        Focus = new TesseraThemeFocusTokens
        {
            Ring = Foreground(0x58A6FF).WithBold(),
            Title = Foreground(0x58A6FF).WithBold(),
            Border = Foreground(0x58A6FF).WithBold(),
            Marker = "●"
        }
    };

    public static TesseraStyle Foreground(int rgb)
    {
        return TesseraStyle.Empty.WithForeground(Hex(rgb));
    }

    public static TesseraStyle Background(int rgb)
    {
        return TesseraStyle.Empty.WithBackground(Hex(rgb));
    }

    public static TesseraStyle Surface(int fg, int bg)
    {
        return Foreground(fg).Merge(Background(bg));
    }

    public static TesseraStyle GetModeAccent(TimerMode mode) => mode switch
    {
        TimerMode.Work => Foreground(0xFF6B6B).WithBold(),
        TimerMode.ShortBreak => Foreground(0x4ECDC4).WithBold(),
        TimerMode.LongBreak => Foreground(0x9B59B6).WithBold(),
        _ => Foreground(0xFF6B6B).WithBold()
    };

    public static TesseraStyle GetModeBackground(TimerMode mode) => mode switch
    {
        TimerMode.Work => Background(0xFF6B6B),
        TimerMode.ShortBreak => Background(0x4ECDC4),
        TimerMode.LongBreak => Background(0x9B59B6),
        _ => Background(0xFF6B6B)
    };

    private static AnsiColor Hex(int rgb)
    {
        var r = (byte)((rgb >> 16) & 0xFF);
        var g = (byte)((rgb >> 8) & 0xFF);
        var b = (byte)(rgb & 0xFF);
        return AnsiColor.Rgb(r, g, b);
    }
}
