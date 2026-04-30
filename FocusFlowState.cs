using Tessera.Controls;

namespace FocusFlow;

internal enum TimerMode
{
    Work,
    ShortBreak,
    LongBreak
}

internal sealed class FocusFlowState
{
    private const int WorkMinutes = 25;
    private const int ShortBreakMinutes = 5;
    private const int LongBreakMinutes = 15;
    private const int SessionsBeforeLongBreak = 4;

    public TimerMode Mode { get; private set; } = TimerMode.Work;
    public int SecondsRemaining { get; private set; }
    public int TotalSeconds { get; private set; }
    public bool IsRunning { get; private set; }
    public int CompletedSessions { get; private set; }
    public int TotalWorkMinutes { get; private set; }

    public FocusFlowState()
    {
        ResetTimer();
    }

    public double Progress => TotalSeconds > 0 ? 1.0 - ((double)SecondsRemaining / TotalSeconds) : 0;

    public string TimeDisplay
    {
        get
        {
            var minutes = SecondsRemaining / 60;
            var seconds = SecondsRemaining % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    public string ModeDisplay => Mode switch
    {
        TimerMode.Work => "FOCUS TIME",
        TimerMode.ShortBreak => "SHORT BREAK",
        TimerMode.LongBreak => "LONG BREAK",
        _ => "FOCUS"
    };

    public string SessionDisplay => $"Session {CompletedSessions + 1} of {SessionsBeforeLongBreak}";

    public void ToggleRunning()
    {
        IsRunning = !IsRunning;
    }

    public void Tick()
    {
        if (!IsRunning || SecondsRemaining <= 0)
            return;

        SecondsRemaining--;

        if (SecondsRemaining == 0)
        {
            CompleteCurrentSession();
        }
    }

    public void Reset()
    {
        IsRunning = false;
        ResetTimer();
    }

    public void Skip()
    {
        CompleteCurrentSession();
    }

    public void SetMode(TimerMode mode)
    {
        Mode = mode;
        IsRunning = false;
        ResetTimer();
    }

    private void CompleteCurrentSession()
    {
        IsRunning = false;

        if (Mode == TimerMode.Work)
        {
            CompletedSessions++;
            TotalWorkMinutes += WorkMinutes;

            Mode = CompletedSessions % SessionsBeforeLongBreak == 0
                ? TimerMode.LongBreak
                : TimerMode.ShortBreak;
        }
        else
        {
            Mode = TimerMode.Work;
        }

        ResetTimer();
    }

    private void ResetTimer()
    {
        TotalSeconds = Mode switch
        {
            TimerMode.Work => WorkMinutes * 60,
            TimerMode.ShortBreak => ShortBreakMinutes * 60,
            TimerMode.LongBreak => LongBreakMinutes * 60,
            _ => WorkMinutes * 60
        };
        SecondsRemaining = TotalSeconds;
    }

    public List<StatItem> BuildStats() =>
    [
        new("Sessions", CompletedSessions.ToString()),
        new("Focus Time", $"{TotalWorkMinutes} min"),
        new("Current", ModeDisplay),
        new("Streak", $"{CompletedSessions % SessionsBeforeLongBreak}/{SessionsBeforeLongBreak}")
    ];
}
