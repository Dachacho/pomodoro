using Tessera;
using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace FocusFlow;

internal sealed class FocusFlowApp : TesseraApp
{
    private readonly FocusFlowState _state = new();
    private readonly TesseraTheme _theme = FocusFlowTheme.Default;

    // Main timer display
    private readonly Label _timerPanel = new()
    {
        Title = "timer",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1)
    };

    // Progress bar
    private readonly ProgressBar _progressBar = new()
    {
        Title = "progress",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0),
        ShowFocusMarker = false
    };

    // Stats panel
    private readonly StatsCard _statsCard = new()
    {
        Title = "session",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        ShowFocusMarker = false
    };

    // Action panel - tells user what to do
    private readonly Label _actionPanel = new()
    {
        Title = "action",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0)
    };

    // Keybinds panel
    private readonly Label _keysPanel = new()
    {
        Title = "controls",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0)
    };

    private readonly StatusBar _footer = new() { Fill = '─' };

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

            // SPACE = main action (start/pause)
            case KeyPressed key when key.IsCharacter(' '):
            case KeyPressed key2 when key2.Is(Key.Enter):
                _state.ToggleRunning();
                return null;

            // R = reset current timer
            case KeyPressed key when key.IsCharacter('r'):
                _state.Reset();
                return null;

            // S = skip to next phase
            case KeyPressed key when key.IsCharacter('s'):
                _state.Skip();
                return null;

            // 1/2/3 = quick switch modes (stops timer, resets)
            case KeyPressed key when key.IsCharacter('1'):
                _state.SetMode(TimerMode.Work);
                return null;

            case KeyPressed key when key.IsCharacter('2'):
                _state.SetMode(TimerMode.ShortBreak);
                return null;

            case KeyPressed key when key.IsCharacter('3'):
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

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            window.Body(body => body.Row(main =>
            {
                // Left side: Timer + Progress
                main.Weighted(2, left => left.Column(stack =>
                {
                    stack.Weighted(1, _timerPanel);
                    stack.Fixed(4, _progressBar);
                    stack.Fixed(6, _actionPanel);
                }));

                // Right side: Stats + Keys
                main.Weighted(1, right => right.Column(stack =>
                {
                    stack.Weighted(1, _statsCard);
                    stack.Fixed(7, _keysPanel);
                }), new Thickness(1, 0, 0, 0));
            }));
            window.Footer(1, _footer);
        });
    }

    private void ApplyTheme()
    {
        _progressBar.ApplyTheme(_theme);
        _statsCard.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        // Timer panel
        _timerPanel.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);

        // Progress bar
        _progressBar.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Yellow).WithBold();
        _progressBar.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);
        _progressBar.LabelStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Fg).WithBold();
        _progressBar.TrackStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);

        // Stats card
        _statsCard.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Cyan).WithBold();
        _statsCard.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);
        _statsCard.KeyStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgDim);
        _statsCard.ValueStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Cyan).WithBold();

        // Action panel
        _actionPanel.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Green).WithBold();
        _actionPanel.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);

        // Keys panel
        _keysPanel.TitleStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Orange).WithBold();
        _keysPanel.BorderStyleText = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);
        _keysPanel.TextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgDim);

        // Footer
        _footer.LeftTextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Cyan).WithBold();
        _footer.RightTextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgDim);
        _footer.FillStyle = FocusFlowTheme.Foreground(FocusFlowTheme.FgMuted);
    }

    private void RefreshControls()
    {
        // Timer panel - show current state clearly
        var modeText = _state.Mode switch
        {
            TimerMode.Work => "🔴 FOCUS TIME",
            TimerMode.ShortBreak => "🟢 SHORT BREAK",
            TimerMode.LongBreak => "🟣 LONG BREAK",
            _ => "FOCUS"
        };

        _timerPanel.Title = modeText.ToLowerInvariant();
        _timerPanel.TitleStyle = FocusFlowTheme.ModeTitle(_state.Mode);

        var statusLine = _state.IsRunning
            ? "▶▶▶  RUNNING  ▶▶▶"
            : "⏸⏸⏸  PAUSED  ⏸⏸⏸";

        _timerPanel.Text = string.Join('\n',
            "",
            "",
            $"          {_state.TimeDisplay}",
            "",
            $"       {statusLine}",
            "",
            $"     {_state.SessionDisplay}");
        _timerPanel.TextStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Fg).WithBold();

        // Progress bar
        _progressBar.SetValue(_state.Progress);
        _progressBar.FillStyle = FocusFlowTheme.ModeBar(_state.Mode);

        // Action panel - clear instruction
        string actionText;
        TesseraStyle actionStyle;

        if (!_state.IsRunning && _state.Progress == 0)
        {
            actionText = ">>> PRESS [SPACE] TO START <<<";
            actionStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Green).WithBold();
        }
        else if (_state.IsRunning)
        {
            actionText = "timer running... [SPACE] to pause";
            actionStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Yellow).WithBold();
        }
        else
        {
            actionText = "[SPACE] resume  [R] reset  [S] skip";
            actionStyle = FocusFlowTheme.Foreground(FocusFlowTheme.Cyan).WithBold();
        }

        _actionPanel.Text = string.Join('\n',
            "",
            $"  {actionText}",
            "",
            $"  switch: [1] focus  [2] break  [3] long");
        _actionPanel.TextStyle = actionStyle;

        // Stats
        _statsCard.SetItems(_state.BuildStats());

        // Keys panel
        _keysPanel.Text = string.Join('\n',
            "",
            "  [SPACE]  start / pause",
            "  [R]      reset timer",
            "  [S]      skip to next",
            "  [1/2/3]  switch mode",
            "  [Q]      quit");

        // Footer
        _footer.LeftText = $" focusflow │ {_state.ModeDisplay.ToLowerInvariant()} ";
        _footer.RightText = $" {_state.CompletedSessions} sessions │ {_state.TotalWorkMinutes}m total ";
    }
}

internal sealed record TickMessage : Message;
