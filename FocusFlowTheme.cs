using Tessera;
using Tessera.Styles;

namespace FocusFlow;

internal static class FocusFlowTheme
{
    // btop-inspired dark theme with vibrant accents
    public const int Bg = 0x0d0d0d;
    public const int BgPanel = 0x1a1a1a;
    public const int BgHighlight = 0x262626;
    public const int BgSelected = 0x333333;

    public const int Fg = 0xe6e6e6;
    public const int FgDim = 0x808080;
    public const int FgMuted = 0x4d4d4d;

    // btop-style accents
    public const int Cyan = 0x6be5fd;
    public const int Magenta = 0xd68ad6;
    public const int Green = 0x97e768;
    public const int Red = 0xf76e6e;
    public const int Yellow = 0xf7d26a;
    public const int Blue = 0x7aa2f7;
    public const int Orange = 0xf7a55d;

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
            Background = Background(BgSelected),
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
