using Tessera;
using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace FocusFlow;

internal sealed class FocusFlowApp : TesseraApp
{
    private readonly FocusFlowState _state = new();
    private readonly TesseraTheme _theme = FocusFlowTheme.Default;

    private readonly Label _timerPanel = new()
    {
        Title = "timer",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0),
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };

    private readonly ProgressBar _progressBar = new()
    {
        Title = "progress",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0),
        ShowFocusMarker = false
    };

    private readonly StatsCard _statsCard = new()
    {
        Title = "stats",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0),
        ShowFocusMarker = false
    };

    private readonly StatusBar _footer = new() { Fill = ' ' };

    // Clean ASCII art digits (5 lines tall)
    private static readonly string[][] Digits =
    [
        [" ██▀▀██ ", "██    ██", "██    ██", "██    ██", " ██▄▄██ "], // 0
        ["   ██   ", "  ███   ", "   ██   ", "   ██   ", "  ████  "], // 1
        [" ██▀▀██ ", "     ██ ", "  ▄▄██  ", " ██     ", " ███████"], // 2
        [" ██▀▀██ ", "     ██ ", "   ▀▀██ ", "     ██ ", " ██▄▄██ "], // 3
        [" ██  ██ ", " ██  ██ ", " ███████", "     ██ ", "     ██ "], // 4
        [" ███████", " ██     ", " ██▀▀██ ", "     ██ ", " ██▄▄██ "], // 5
        [" ██▀▀▀  ", " ██     ", " ██▀▀██ ", " ██  ██ ", " ██▄▄██ "], // 6
        [" ███████", "     ██ ", "    ██  ", "   ██   ", "   ██   "], // 7
        [" ██▀▀██ ", " ██  ██ ", " ██▀▀██ ", " ██  ██ ", " ██▄▄██ "], // 8
        [" ██▀▀██ ", " ██  ██ ", " ██▄▄██ ", "     ██ ", " ██▄▄██ "], // 9
    ];

    private static readonly string[] Colon = ["        ", "   ██   ", "        ", "   ██   ", "        "];

    public FocusFlowApp()
    {
        ApplyTheme();
    }

    public override TesseraEffect? Initialize()
    {
        return TesseraEffects.Periodic(TimeSpan.FromSeconds(1), _ => new TickMessage());
    }

    public override TesseraEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key when key.IsCharacter('q'):
            case KeyPressed key2 when key2.IsCharacter('c', ModifierKeys.Ctrl):
                return TesseraEffects.Quit;

            case KeyPressed key when key.IsCharacter(' '):
            case KeyPressed key2 when key2.Is(Key.Enter):
                _state.ToggleRunning();
                return null;

            case KeyPressed key when key.IsCharacter('r'):
                _state.Reset();
                return null;

            case KeyPressed key when key.IsCharacter('s'):
                _state.Skip();
                return null;

            case TickMessage:
                _state.Tick();
                return null;

            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshControls();

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            window.Body(body => body.Column(main =>
            {
                main.Weighted(1, _timerPanel);
                main.Fixed(3, _progressBar);
                main.Fixed(5, _statsCard);
            }));
            window.Footer(1, _footer);
        });
    }

    private void ApplyTheme()
    {
        _progressBar.ApplyTheme(_theme);
        _statsCard.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        _timerPanel.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);

        _progressBar.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Yellow).WithBold();
        _progressBar.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);
        _progressBar.LabelStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Fg).WithBold();
        _progressBar.TrackStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);

        _statsCard.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Aqua).WithBold();
        _statsCard.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);
        _statsCard.KeyStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgDim);
        _statsCard.ValueStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Aqua).WithBold();

        _footer.LeftTextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Green).WithBold();
        _footer.RightTextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgDim);
    }

    private void RefreshControls()
    {
        // Nerd font icons:  focus,  coffee/break,  sleep/long
        var modeText = _state.Mode switch
        {
            TimerMode.Work => " focus",
            TimerMode.ShortBreak => " break",
            TimerMode.LongBreak => " long break",
            _ => " focus"
        };
        _timerPanel.Title = modeText;
        _timerPanel.TitleStyle = FocusFlowTheme.ModeTitle(_state.Mode);

        // Build ASCII time display
        var minutes = _state.SecondsRemaining / 60;
        var seconds = _state.SecondsRemaining % 60;
        var m1 = minutes / 10;
        var m2 = minutes % 10;
        var s1 = seconds / 10;
        var s2 = seconds % 10;

        var asciiLines = new string[5];
        for (var i = 0; i < 5; i++)
        {
            asciiLines[i] = $"{Digits[m1][i]}{Digits[m2][i]}{Colon[i]}{Digits[s1][i]}{Digits[s2][i]}";
        }

        // Nerd font:  play,  pause
        var status = _state.IsRunning ? " running" : " paused";
        var session = _state.SessionDisplay.ToLowerInvariant();

        _timerPanel.Text = string.Join('\n',
            "",
            asciiLines[0],
            asciiLines[1],
            asciiLines[2],
            asciiLines[3],
            asciiLines[4],
            "",
            $"{status}  ·  {session}");
        _timerPanel.TextStyle = FocusFlowTheme.ModeTitle(_state.Mode);

        _progressBar.SetValue(_state.Progress);
        _progressBar.FillStyle = FocusFlowTheme.ModeBar(_state.Mode);

        _statsCard.SetItems(_state.BuildStats());

        var hint = !_state.IsRunning && _state.Progress == 0
            ? "󱁐 start"
            : _state.IsRunning
                ? "󱁐 pause"
                : "󱁐 resume";
        _footer.LeftText = $" {hint}   r reset   s skip   q quit ";
        _footer.RightText = $"  {_state.CompletedSessions}    {_state.TotalWorkMinutes}m ";
    }
}

internal sealed record TickMessage : Message;
