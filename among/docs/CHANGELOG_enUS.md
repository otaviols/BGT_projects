# Changelog

Everything that changed in the game, newest first.

*Também disponível em português: [CHANGELOG_ptBR.md](CHANGELOG_ptBR.md).*

This file is the **source** of the release notes the game announces when updating: the `version.json`
published to the site is generated from the first entry here (see `tools/make_version_json.py`).
Writing the note in two places would be a guarantee that one day they disagree.

---

## 0.17.1

The release notes the game announces when updating now show up **in your language**. Before they
always came in Portuguese, even for people playing in English — which is the game's default.

## 0.17.0

**New: the security camera station.** One player at a time watches a room from a distance, hearing
what happens there as if standing in it. Arrows change room, the radar key says who is in it, ESC
switches off.

While watching you stand still and deaf to your own surroundings — someone can walk right up to you
without you noticing. And anyone in the watched room hears a radio playing somewhere far off:
subtle, but whoever pays attention notices they are being watched.

Sabotaged communications take the cameras down.

## 0.16.0

Housekeeping release: nothing changes for players. Under the hood, roles now declare what they can
do (groundwork for new roles), sounds were organised into folders by category, and the manuals got a
place of their own.

## 0.15.0

- **Sabotage no longer affects ghosts.** After dying you keep the radar, your task markers and body
  sounds. Sabotage exists to pressure those who still have something to lose.
- The error log now covers the menus too, and gets written even when the game folder is protected.
- The temporary folder left behind by an update is cleaned up on its own.

## 0.14.2

New sound for the start of watering, in the greenhouse task.

## 0.14.1

- **Matches of up to 15 players** (was 10), with five new colours.
- Automatic updates no longer fail on machines where the temporary folder is blocked: the game looks
  for somewhere it can actually write.

## 0.13.0

- In the waiting room, **P** says who is there and **C** says the match rules.
- The lobby list shows impostors, kill cooldown and sabotage on each entry.
- Lobby creation gained fine adjustments over the preset: players, impostors, tasks, meeting timers
  and sabotage.

## 0.12.0

- **Pluggable languages**: drop a file in the `lang` folder and the language shows up in the game;
  anything untranslated falls back to English.
- Messages coming from the server are now translated into each player's own language.
- Fixed the registration warnings, which were showing untranslated.

## 0.11.1

- The game now opens in **English** by default; anyone who prefers another language switches once in
  Settings and the choice sticks.
- Fixed the manual about vents: you get in with **Enter**, and **V** only serves to leave or travel
  once you are already inside.

## 0.11.0

**New: send a message to the people who make the game**, straight from the lobby list. The version,
what you were doing and the error log are sent along automatically.

## 0.10.1

Fixed the **T** key during meetings: it did not respond because the key was read inside a stretch of
code the game skips precisely while a meeting is happening.

## 0.10.0

- Two impostors instead of three in matches of up to 14 players.
- Ghosts now hear themselves bumping into walls.
- During a meeting, **T** says how much time is left.
- The oxygen sabotage announces the time left every 30 seconds and counts down the last 10.
- The asteroid task gained an asteroid coming from ahead, shot with the up arrow.

## 0.9.3

The window title now shows the game version.

## 0.9.2

- **Automatic updates actually work**: before, they only opened the download site.
- Fixed the learn-the-sounds menu, where footsteps and the death sound played nothing.

## 0.9.1

Fixed the learn-the-sounds menu: footsteps and the death sound played nothing in the installed game.

## 0.9.0

First beta release.

- Automatic updates.
- Language choice in settings.
- Fuel task back to its old shape (release when the filling tone matches the reference tone).
- Watering task reworked: you have to carry the watering can to the right bed.
