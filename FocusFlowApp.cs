using Tessera;
using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace FocusFlow;

internal sealed class FocusFlowApp : TesseraApp
{
    private readonly FocusFlowState _state = new();
    private readonly TesseraTheme _theme = FocusFlowTheme.Default;

    // Timer panel (left side)
    private readonly Label _timerPanel = new()
    {
        Title = "timer",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1)
    };

    // Progress panel
    private readonly ProgressBar _progressBar = new()
    {
        Title = "elapsed",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0),
        ShowFocusMarker = false
    };

    // Session history sparkline
    private readonly Sparkline _sessionSpark = new()
    {
        Title = "session history",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0)
    };

    // Stats panel (right side)
    private readonly StatsCard _statsCard = new()
    {
        Title = "stats",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        ShowFocusMarker = false
    };

    // Mode selector panel
    private readonly Label _modePanel = new()
    {
        Title = "mode",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0)
    };

    // Keybinds panel
    private readonly Label _keysPanel = new()
    {
        Title = "keys",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0)
    };

    private readonly StatusBar _footer = new() { Fill = '─' };

    // Track session durations for sparkline
    private readonly List<double> _sessionHistory = [0, 0, 0, 0, 0, 0, 0, 0];

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
            case KeyPressed key3 when key3.IsCharacter('p'):
                _state.ToggleRunning();
                return null;

            case KeyPressed key when key.IsCharacter('r'):
                _state.Reset();
                return null;

            case KeyPressed key when key.IsCharacter('s'):
            case KeyPressed key2 when key2.IsCharacter('n'):
                var prevSessions = _state.CompletedSessions;
                _state.Skip();
                if (_state.CompletedSessions > prevSessions)
                    RecordSession();
                return null;

            case KeyPressed key when key.IsCharacter('w'):
                _state.SetMode(TimerMode.Work);
                return null;

            case KeyPressed key when key.IsCharacter('b'):
                _state.SetMode(TimerMode.ShortBreak);
                return null;

            case KeyPressed key when key.IsCharacter('l'):
                _state.SetMode(TimerMode.LongBreak);
                return null;

            case TickMessage:
                var prev = _state.CompletedSessions;
                _state.Tick();
                if (_state.CompletedSessions > prev)
                    RecordSession();
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
                // Top row: Timer + Stats
                main.Weighted(3, top => top.Row(row =>
                {
                    // Left: Timer display
                    row.Weighted(2, left => left.Column(stack =>
                    {
                        stack.Weighted(2, _timerPanel);
                        stack.Fixed(4, _progressBar);
                    }));
                    // Right: Stats + Mode
                    row.Weighted(1, right => right.Column(stack =>
                    {
                        stack.Weighted(1, _statsCard);
                        stack.Fixed(5, _modePanel);
                    }), new Thickness(1, 0, 0, 0));
                }));
                // Bottom row: History + Keys
                main.Fixed(6, bottom => bottom.Row(row =>
                {
                    row.Weighted(2, _sessionSpark);
                    row.Fixed(32, _keysPanel, new Thickness(1, 0, 0, 0));
                }));
            }));
            window.Footer(1, _footer);
        });
    }

    private void ApplyTheme()
    {
        _progressBar.ApplyTheme(_theme);
        _statsCard.ApplyTheme(_theme);
        _sessionSpark.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        // Timer panel - cyan title
        _timerPanel.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Cyan).WithBold();
        _timerPanel.BorderStyleText = _theme.Border.Default;
        _timerPanel.TextStyle = _theme.Text.Primary;

        // Progress bar
        _progressBar.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Yellow).WithBold();
        _progressBar.BorderStyleText = _theme.Border.Default;
        _progressBar.LabelStyle = _theme.Text.Primary.WithBold();
        _progressBar.TrackStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);

        // Session sparkline
        _sessionSpark.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Blue).WithBold();
        _sessionSpark.BorderStyleText = _theme.Border.Default;
        _sessionSpark.DataStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Blue).WithBold();

        // Stats card - magenta title
        _statsCard.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Magenta).WithBold();
        _statsCard.BorderStyleText = _theme.Border.Default;
        _statsCard.KeyStyle = _theme.Text.Muted;
        _statsCard.ValueStyle = _theme.Accent.Primary;

        // Mode panel - green title
        _modePanel.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Green).WithBold();
        _modePanel.BorderStyleText = _theme.Border.Default;
        _modePanel.TextStyle = _theme.Text.Secondary;

        // Keys panel - orange title
        _keysPanel.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Orange).WithBold();
        _keysPanel.BorderStyleText = _theme.Border.Default;
        _keysPanel.TextStyle = _theme.Text.Muted;

        // Footer
        _footer.LeftTextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Cyan).WithBold();
        _footer.RightTextStyle = _theme.Text.Muted;
        _footer.FillStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);
    }

    private void RefreshControls()
    {
        // Timer panel - big ASCII time with mode indicator
        var modeIcon = _state.Mode switch
        {
            TimerMode.Work => "◉ FOCUS",
            TimerMode.ShortBreak => "◎ BREAK",
            TimerMode.LongBreak => "◈ LONG BREAK",
            _ => "◉ FOCUS"
        };
        var statusIcon = _state.IsRunning ? "▶" : "⏸";
        var statusText = _state.IsRunning ? "running" : "paused";

        _timerPanel.Title = $"timer │ {modeIcon.ToLowerInvariant()}";
        _timerPanel.TitleStyle = FocusFlowTheme.ModeTitle(_state.Mode);
        _timerPanel.Text = string.Join('\n',
            "",
            $"        {_state.TimeDisplay}",
            "",
            $"     {statusIcon} {statusText}",
            "",
            $"     {_state.SessionDisplay.ToLowerInvariant()}");

        // Progress bar
        _progressBar.SetValue(_state.Progress);
        _progressBar.FillStyle = FocusFlowTheme.ModeBar(_state.Mode);

        // Stats
        _statsCard.SetItems(_state.BuildStats());

        // Mode panel
        var workMark = _state.Mode == TimerMode.Work ? "●" : "○";
        var breakMark = _state.Mode == TimerMode.ShortBreak ? "●" : "○";
        var longMark = _state.Mode == TimerMode.LongBreak ? "●" : "○";
        _modePanel.Text = string.Join('\n',
            $" {workMark} [w] work      25m",
            $" {breakMark} [b] break      5m",
            $" {longMark} [l] long      15m");

        // Session sparkline
        _sessionSpark.SetSamples(_sessionHistory);

        // Keys panel
        _keysPanel.Text = string.Join('\n',
            " [space] toggle  [r] reset",
            " [s] skip        [q] quit",
            " [w/b/l] switch mode");

        // Footer
        var elapsed = (_state.TotalSeconds - _state.SecondsRemaining) / 60;
        _footer.LeftText = $" focusflow ";
        _footer.RightText = $"{_state.CompletedSessions} sessions │ {_state.TotalWorkMinutes}m focused │ {elapsed}m elapsed ";
    }

    private void RecordSession()
    {
        _sessionHistory.RemoveAt(0);
        _sessionHistory.Add(_state.Mode == TimerMode.Work ? 25 : (_state.Mode == TimerMode.ShortBreak ? 5 : 15));
    }
}

internal sealed record TickMessage : Message;
