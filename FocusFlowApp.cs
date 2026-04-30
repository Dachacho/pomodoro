using Tessera;
using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace FocusFlow;

internal sealed class FocusFlowApp : TesseraApp
{
    private readonly FocusFlowState _state = new();
    private readonly TesseraTheme _theme = FocusFlowTheme.Default;

    private readonly Label _titleChip = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Label _modeChip = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Label _timeDisplay = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Label _sessionChip = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Label _statusChip = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly ProgressBar _progress = new()
    {
        Title = "progress",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0),
        ShowFocusMarker = false
    };

    private readonly StatsCard _stats = new()
    {
        Title = "session",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        ShowFocusMarker = false
    };

    private readonly Label _keybinds = new()
    {
        Title = "keys",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0)
    };

    private readonly StatusBar _footer = new() { Fill = ' ' };

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
                _state.Skip();
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
                _state.Tick();
                return null;

            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshControls();

        var shellWidth = Math.Max(52, Math.Min(72, context.Width - 4));
        var shellHeight = Math.Max(22, Math.Min(28, context.Height - 2));

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            window.Body(body => body.Center(
                center => center.Column(column =>
                {
                    column.Gap(1);

                    // Title
                    column.Auto(row => row.Center(_titleChip));

                    // Mode indicator
                    column.Auto(row => row.Center(_modeChip));

                    // Big time
                    column.Auto(row => row.Center(_timeDisplay));

                    // Status chips
                    column.Fixed(1, row => row.Center(chips => chips.Row(r =>
                    {
                        r.Gap(2);
                        r.Auto(_sessionChip);
                        r.Auto(_statusChip);
                    })));

                    // Progress
                    column.Fixed(3, row => row.Center(p => p.Row(r => r.Fixed(42, _progress))));

                    // Stats
                    column.Fixed(7, row => row.Center(s => s.Row(r => r.Fixed(30, _stats))));

                    // Keybinds
                    column.Fixed(5, row => row.Center(k => k.Row(r => r.Fixed(42, _keybinds))));
                }),
                shellWidth,
                shellHeight));
            window.Footer(1, _footer);
        });
    }

    private void ApplyTheme()
    {
        _progress.ApplyTheme(_theme);
        _stats.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        // Title chip
        _titleChip.TextStyle = FocusFlowTheme.Chip(FocusFlowTheme.Bg0, FocusFlowTheme.Yellow);

        // Time display - big and bright
        _timeDisplay.TextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Fg0).WithBold();

        // Session chip
        _sessionChip.TextStyle = FocusFlowTheme.Chip(FocusFlowTheme.Bg0, FocusFlowTheme.Aqua);

        // Status chip
        _statusChip.TextStyle = FocusFlowTheme.Chip(FocusFlowTheme.Bg0, FocusFlowTheme.Blue);

        // Progress bar
        _progress.TitleStyle = _theme.Text.Secondary.WithBold();
        _progress.LabelStyle = _theme.Text.Primary.WithBold();
        _progress.TrackStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Bg3);
        _progress.BorderStyleText = _theme.Border.Default;

        // Stats card
        _stats.TitleStyle = _theme.Text.Secondary.WithBold();
        _stats.KeyStyle = _theme.Text.Muted;
        _stats.ValueStyle = _theme.Accent.Primary;
        _stats.BorderStyleText = _theme.Border.Default;

        // Keybinds
        _keybinds.TitleStyle = _theme.Text.Secondary.WithBold();
        _keybinds.TextStyle = _theme.Text.Muted;
        _keybinds.BorderStyleText = _theme.Border.Default;

        // Footer
        _footer.LeftTextStyle = FocusFlowTheme.Chip(FocusFlowTheme.Bg1, FocusFlowTheme.Orange);
        _footer.RightTextStyle = _theme.Text.Muted;
        _footer.FillStyle = _theme.Surface.Panel;
    }

    private void RefreshControls()
    {
        // Title
        _titleChip.Text = "  focusflow  ";

        // Mode with color
        var modeText = _state.ModeDisplay.ToLowerInvariant();
        _modeChip.Text = $"  {modeText}  ";
        _modeChip.TextStyle = FocusFlowTheme.ModeStyle(_state.Mode);

        // Time
        _timeDisplay.Text = _state.TimeDisplay;

        // Session info
        _sessionChip.Text = $"  {_state.SessionDisplay.ToLowerInvariant()}  ";

        // Status
        var statusText = _state.IsRunning ? "▶ running" : "⏸ paused";
        _statusChip.Text = $"  {statusText}  ";
        _statusChip.TextStyle = _state.IsRunning
            ? FocusFlowTheme.Chip(FocusFlowTheme.Bg0, FocusFlowTheme.Green)
            : FocusFlowTheme.Chip(FocusFlowTheme.Bg0, FocusFlowTheme.Blue);

        // Progress
        _progress.SetValue(_state.Progress);
        _progress.FillStyle = FocusFlowTheme.ModeFill(_state.Mode);

        // Stats
        _stats.SetItems(_state.BuildStats());

        // Keybinds
        _keybinds.Text = string.Join('\n',
            "spc/enter  toggle timer",
            "r reset · s skip · q quit",
            "w work · b break · l long");

        // Footer
        _footer.LeftText = $"  focusflow  ";
        _footer.RightText = $"{_state.CompletedSessions} done · {_state.TotalWorkMinutes}m focused";
    }
}

internal sealed record TickMessage : Message;
