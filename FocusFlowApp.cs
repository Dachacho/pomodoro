using Tessera;
using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace FocusFlow;

internal sealed class FocusFlowApp : TesseraApp
{
    private readonly FocusFlowState _state = new();

    private readonly Label _header = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Label _modeLabel = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Label _timeLabel = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Label _sessionLabel = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly ProgressBar _progress = new()
    {
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0),
        ShowFocusMarker = false
    };

    private readonly StatsCard _stats = new()
    {
        Title = "stats",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        ShowFocusMarker = false
    };

    private readonly Label _bindsHelp = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly StatusBar _footer = new() { Fill = ' ' };

    public FocusFlowApp()
    {
        ConfigureTheme();
    }

    public override TesseraEffect? Initialize()
    {
        return TesseraEffects.Periodic(TimeSpan.FromSeconds(1), _ => new TimerTickMessage());
    }

    public override TesseraEffect? Update(Message message)
    {
        switch (message)
        {
            // Quit: q or Ctrl+C
            case KeyPressed key when key.IsCharacter('q'):
            case KeyPressed key2 when key2.IsCharacter('c', ModifierKeys.Ctrl):
                return TesseraEffects.Quit;

            // Start/Pause: Space or Enter or p
            case KeyPressed key when key.IsCharacter(' '):
            case KeyPressed key2 when key2.Is(Key.Enter):
            case KeyPressed key3 when key3.IsCharacter('p'):
                _state.ToggleRunning();
                return null;

            // Reset: r
            case KeyPressed key when key.IsCharacter('r'):
                _state.Reset();
                return null;

            // Skip to next: s or n
            case KeyPressed key when key.IsCharacter('s'):
            case KeyPressed key2 when key2.IsCharacter('n'):
                _state.Skip();
                return null;

            // Switch to work mode: w
            case KeyPressed key when key.IsCharacter('w'):
                _state.SetMode(TimerMode.Work);
                return null;

            // Switch to break mode: b
            case KeyPressed key when key.IsCharacter('b'):
                _state.SetMode(TimerMode.ShortBreak);
                return null;

            // Switch to long break: l
            case KeyPressed key when key.IsCharacter('l'):
                _state.SetMode(TimerMode.LongBreak);
                return null;

            // Timer tick
            case TimerTickMessage:
                _state.Tick();
                return null;

            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshControls();

        var cardWidth = Math.Max(50, Math.Min(70, context.Width - 4));
        var cardHeight = Math.Max(20, Math.Min(26, context.Height - 2));

        return Screen.Build(window =>
        {
            window.Body(body => body.Center(
                center => center.Column(column =>
                {
                    column.Gap(1);

                    // Header
                    column.Auto(row => row.Center(_header));

                    // Mode chip
                    column.Auto(row => row.Center(_modeLabel));

                    // Big time
                    column.Auto(row => row.Center(_timeLabel));

                    // Session info
                    column.Auto(row => row.Center(_sessionLabel));

                    // Progress bar
                    column.Fixed(3, row => row.Center(
                        p => p.Row(r => r.Fixed(44, _progress))));

                    // Stats
                    column.Fixed(7, row => row.Center(
                        s => s.Row(r => r.Fixed(32, _stats))));

                    // Keybinds help
                    column.Auto(row => row.Center(_bindsHelp));
                }),
                cardWidth,
                cardHeight));

            window.Footer(1, _footer);
        });
    }

    private void ConfigureTheme()
    {
        var theme = FocusFlowTheme.Default;

        _progress.ApplyTheme(theme);
        _stats.ApplyTheme(theme);
        _footer.ApplyTheme(theme);

        // Header styling
        _header.TextStyle = FocusFlowTheme.Foreground(0xfabd2f).WithBold();

        // Time display - big and bold
        _timeLabel.TextStyle = FocusFlowTheme.Foreground(0xfbf1c7).WithBold();

        // Session info - muted
        _sessionLabel.TextStyle = FocusFlowTheme.Foreground(0xa89984);

        // Keybinds help - dim
        _bindsHelp.TextStyle = FocusFlowTheme.Foreground(0x665c54);

        // Stats card
        _stats.TitleStyle = FocusFlowTheme.Foreground(0xbdae93).WithBold();
        _stats.KeyStyle = FocusFlowTheme.Foreground(0xa89984);
        _stats.ValueStyle = FocusFlowTheme.Foreground(0xfe8019).WithBold();
        _stats.BorderStyleText = FocusFlowTheme.Foreground(0x504945);

        // Progress bar
        _progress.TitleStyle = FocusFlowTheme.Foreground(0xbdae93);
        _progress.LabelStyle = FocusFlowTheme.Foreground(0xebdbb2);
        _progress.TrackStyle = FocusFlowTheme.Foreground(0x504945);
        _progress.BorderStyleText = FocusFlowTheme.Foreground(0x504945);

        // Footer
        _footer.LeftTextStyle = FocusFlowTheme.Foreground(0xbdae93);
        _footer.RightTextStyle = FocusFlowTheme.Foreground(0x665c54);
        _footer.FillStyle = FocusFlowTheme.Background(0x3c3836);
    }

    private void RefreshControls()
    {
        // Header
        _header.Text = "┃ focusflow ┃";

        // Mode chip with color
        var modeText = _state.ModeDisplay.ToLowerInvariant();
        _modeLabel.Text = $"[ {modeText} ]";
        _modeLabel.TextStyle = FocusFlowTheme.GetModeStyle(_state.Mode);

        // Big time display
        _timeLabel.Text = _state.TimeDisplay;

        // Session info
        _sessionLabel.Text = _state.SessionDisplay.ToLowerInvariant();

        // Progress bar
        _progress.SetValue(_state.Progress);
        _progress.FillStyle = FocusFlowTheme.GetModeFill(_state.Mode);

        // Stats
        _stats.SetItems(_state.BuildStats());

        // Keybinds
        _bindsHelp.Text = "spc/p pause · r reset · s skip · w work · b break · l long · q quit";

        // Footer
        var status = _state.IsRunning ? "▶ running" : "⏸ paused";
        _footer.LeftText = $"focusflow {status}";
        _footer.RightText = $"{_state.CompletedSessions} sessions · {_state.TotalWorkMinutes}m focused";
    }
}

internal sealed record TimerTickMessage : Message;
