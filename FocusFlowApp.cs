using Tessera;
using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace FocusFlow;

internal sealed class FocusFlowApp : TesseraApp
{
    private readonly FocusFlowState _state = new();

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
        Title = "Progress",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(1, 0)
    };

    private readonly StatsCard _stats = new()
    {
        Title = "Statistics",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1)
    };

    private readonly Button _startButton = new() { Text = "Start", Padding = Thickness.Symmetric(3, 1) };
    private readonly Button _resetButton = new() { Text = "Reset", Padding = Thickness.Symmetric(3, 1) };
    private readonly Button _skipButton = new() { Text = "Skip", Padding = Thickness.Symmetric(3, 1) };

    private readonly StatusBar _footer = new() { Fill = ' ' };

    private readonly Control[] _focusOrder;
    private int _focusIndex;

    public FocusFlowApp()
    {
        _focusOrder = [_startButton, _resetButton, _skipButton];
        ConfigureTheme();
        WireEvents();
        _startButton.RequestFocus();
    }

    public override TesseraEffect? Initialize()
    {
        return TesseraEffects.Periodic(TimeSpan.FromSeconds(1), _ => new TimerTickMessage());
    }

    public override TesseraEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key when key.IsCharacter('c', ModifierKeys.Ctrl):
            case KeyPressed key2 when key2.IsCharacter('q'):
                return TesseraEffects.Quit;

            case KeyPressed key when key.Is(Key.Tab):
                FocusNext();
                return null;

            case KeyPressed key when key.IsCharacter(' '):
                _state.ToggleRunning();
                return null;

            case KeyPressed key when key.IsCharacter('r'):
                _state.Reset();
                return null;

            case KeyPressed key when key.IsCharacter('s'):
                _state.Skip();
                return null;

            case KeyPressed key when key.IsCharacter('w'):
                _state.SetMode(TimerMode.Work);
                return null;

            case KeyPressed key when key.IsCharacter('b'):
                _state.SetMode(TimerMode.ShortBreak);
                return null;

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

        var cardWidth = Math.Max(60, Math.Min(80, context.Width - 4));
        var cardHeight = Math.Max(18, Math.Min(24, context.Height - 2));

        return Screen.Build(window =>
        {
            window.Body(body => body.Center(
                center => center.Column(column =>
                {
                    column.Gap(1);

                    // Mode indicator
                    column.Auto(content => content.Center(_modeLabel));

                    // Big time display
                    column.Auto(content => content.Center(_timeLabel));

                    // Session info
                    column.Auto(content => content.Center(_sessionLabel));

                    // Progress bar
                    column.Fixed(3, progress => progress.Center(
                        row => row.Row(r => r.Fixed(40, _progress))));

                    // Control buttons
                    column.Fixed(3, actions => actions.Center(row => row.Row(buttons =>
                    {
                        buttons.Gap(2);
                        buttons.Fixed(14, _startButton);
                        buttons.Fixed(14, _resetButton);
                        buttons.Fixed(14, _skipButton);
                    })));

                    // Stats card
                    column.Fixed(8, stats => stats.Center(
                        row => row.Row(r => r.Fixed(36, _stats))));
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
        _startButton.ApplyTheme(theme);
        _resetButton.ApplyTheme(theme);
        _skipButton.ApplyTheme(theme);
        _footer.ApplyTheme(theme);

        _modeLabel.TextStyle = FocusFlowTheme.GetModeAccent(_state.Mode);
        _timeLabel.TextStyle = FocusFlowTheme.Foreground(0xF0F6FC).WithBold();
        _sessionLabel.TextStyle = FocusFlowTheme.Foreground(0x8B949E);

        ConfigureButton(_startButton, 0x238636);  // Green
        ConfigureButton(_resetButton, 0x6E7681); // Gray
        ConfigureButton(_skipButton, 0x8957E5);  // Purple

        _stats.TitleStyle = theme.Text.Secondary.WithBold();
        _stats.KeyStyle = theme.Text.Muted;
        _stats.ValueStyle = theme.Accent.Primary.WithBold();

        _footer.LeftTextStyle = theme.Text.Secondary;
        _footer.RightTextStyle = theme.Text.Muted;
        _footer.FillStyle = theme.Surface.Panel;
    }

    private static void ConfigureButton(Button button, int color)
    {
        button.LabelStyle = FocusFlowTheme.Foreground(0xFFFFFF).WithBold();
        button.SurfaceStyle = FocusFlowTheme.Background(color);
        button.FocusedSurfaceStyle = FocusFlowTheme.Background(color);
        button.PressedSurfaceStyle = FocusFlowTheme.Background(color - 0x101010);
    }

    private void WireEvents()
    {
        _startButton.Activated += (_, _) => _state.ToggleRunning();
        _resetButton.Activated += (_, _) => _state.Reset();
        _skipButton.Activated += (_, _) => _state.Skip();
    }

    private void FocusNext()
    {
        _focusIndex = (_focusIndex + 1) % _focusOrder.Length;
        _focusOrder[_focusIndex].RequestFocus();
    }

    private void RefreshControls()
    {
        _modeLabel.Text = $"  {_state.ModeDisplay}  ";
        _modeLabel.TextStyle = FocusFlowTheme.GetModeAccent(_state.Mode);

        _timeLabel.Text = _state.TimeDisplay;
        _sessionLabel.Text = _state.SessionDisplay;

        _progress.SetValue(_state.Progress);
        _progress.FillStyle = FocusFlowTheme.GetModeBackground(_state.Mode);

        _startButton.Text = _state.IsRunning ? "Pause" : "Start";

        _stats.SetItems(_state.BuildStats());

        _footer.LeftText = $"FocusFlow | {(_state.IsRunning ? "Running" : "Paused")}";
        _footer.RightText = "space: start/pause | r: reset | s: skip | w: work | b: break | q: quit";
    }
}

internal sealed record TimerTickMessage : Message;
