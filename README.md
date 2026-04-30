# FocusFlow

A pomodoro timer TUI built to test out [Tessera](../tessera), a .NET terminal UI framework.

## Screenshot

```
╭─  focus ─────────────────────────────────────────────────────────╮
│                                                                   │
│        ██▀▀██  ██▀▀██          ██▀▀██  ██▀▀██                    │
│       ██    ██     ██    ██   ██       ██    ██                   │
│       ██    ██   ▄▄██         ██▀▀██   ██▀▀██                     │
│       ██    ██  ██       ██        ██  ██    ██                   │
│        ██▄▄██  ███████         ██▄▄██   ██▄▄██                    │
│                                                                   │
│                  running  ·  session 1 of 4                      │
╰───────────────────────────────────────────────────────────────────╯
╭─ progress ────────────────────────────────────────────────────────╮
│ ████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ │
╰───────────────────────────────────────────────────────────────────╯
╭─ stats ───────────────────────────────────────────────────────────╮
│ Pomodoros      0                                                  │
│ Focus          0m                                                 │
│ Long Break     in 4                                               │
╰───────────────────────────────────────────────────────────────────╯
 󱁐 pause   r reset   s skip   q quit              0    0m
```

## Features

- Big ASCII art timer display
- Follows the pomodoro technique cycle automatically
  - 25 min focus → 5 min break → repeat
  - Long break (15 min) after every 4 pomodoros
- Gruvbox color theme (high contrast for transparent terminals)
- Nerd font icons
- Keyboard-only controls

## Controls

| Key | Action |
|-----|--------|
| `Space` | Start / Pause |
| `r` | Reset current timer |
| `s` | Skip to next phase |
| `q` | Quit |

## Requirements

- .NET 10
- A terminal with true color support
- A [Nerd Font](https://www.nerdfonts.com/) for icons

## Run

```bash
dotnet run
```

## Why

This was built to test out Tessera's capabilities:
- Layout system (`Screen.Build`, `Column`, `Row`, `Weighted`, `Fixed`)
- Controls (`Label`, `ProgressBar`, `StatsCard`, `StatusBar`)
- Theming (`TesseraTheme`, `TesseraStyle`, `AnsiColor`)
- Periodic effects (`TesseraEffects.Periodic`)
- Keyboard input handling

## License

MIT
