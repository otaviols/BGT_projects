# Changelog

Everything that changed in the game, newest first.

*Também disponível em português: NOVIDADES.md, nesta mesma pasta.*

---

## 0.23.0

- **Spanish!** The game gained its first translation made by a player. It is under Settings, in the
  Language option.
- **Send a translation through the game.** Translated the game into your language? In the match
  list, choose "Send a translation to the people who make the game": it is reviewed and ships with
  the next version, and you get a reply inside the game. The manual explains how to make one. In
  the language list, the translator's name is spoken next to the language.

## 0.22.5

Fixed automatic updates, which in the previous two versions closed the game and never came back.
If you are on 0.22.3 or 0.22.4, download this version from the site once; from then on updates work
on their own again.

## 0.22.4

Cleanup of the texts that ship with the game: the version history lost an internal note that meant
nothing to players, and the manuals now refer to each other by the names that exist in the folder.

## 0.22.3

Mac version on the site, experimental, next to the Linux one. It is a disk image: open it and drag
the game to Applications. If you use Mac or Linux, tell us how it went through the in-game message.

## 0.22.2

Every action key in a match can now be remapped in settings — the radar lock (Q) and the radar mode
switch were missing; the mode switch now has the M key (Ctrl + Tab still works).

## 0.22.1

The manuals and the version history moved into a `docs` folder inside the game folder, instead of
sitting loose next to the executable.

## 0.22.0

- **What's new, inside the game.** The main menu gained "What's new": pick a version and hear what
  changed in it. The history also ships as a file in the game folder (CHANGELOG.md).
- **Music volume** is its own setting now. Today it is only the menu music, but whoever wants to
  hear the footsteps turns the music down and nothing else.
- **Linux version** on the site, experimental. On those systems updating is still manual: the game
  says so and opens the download page.

## 0.21.0

**Replies to your messages come through the game.** When the people who make the game answer, you
hear it on reaching the match list, and the "Replies to your messages" option reads your message and
the reply side by side. No email, no leaving the game.

## 0.20.0

Three things you asked for:

- **Explore the map**, in the main menu: walk around the ship alone, with no server and no
  pressure. Each room is named as you enter, Tab lists what is in it, Enter says what the nearest
  object is.
- **The host can change the rules with the room already open**, with the O key. The fields come
  with the current values, and everyone in the room is told about the change.
- **Radar lock**: after pointing at someone with Tab, Q makes the radar beep at that person on its
  own, with the pitch rising as they get closer. It releases by itself when they leave the room.

## 0.19.0

**New: private matches.** When creating a match, fill in the access code: the match disappears from
the list, and the only way in is choosing "Join a private match with a code" and typing the same
code. Inside the room, the C key repeats the code so you can pass it to friends. This was your most
repeated request.

Fixed: leaving a match midway and joining another got you kicked out of the new one when the old
one ended, as if you had won.

When you cannot join a match, the game now says why: wrong code, room full, or match already
started.

## 0.18.5

A new footstep for the wooden floor, balanced against the rest: this one came in louder than the
other floors rather than quieter.

## 0.18.4

The footstep on the wooden floor is a bit louder — it still sounded weak next to the other floors.

## 0.18.3

The footstep on the tiled floor is back to the old one. The new sound that came in the last version
was too muffled, and no amount of volume adjustment fixed it.

## 0.18.2

New sounds: footsteps, room ambiences and the emergency button alarm were replaced with better
versions. Footsteps gained more variations, so walking around the ship sounds less repetitive.

Some floors were much quieter than others — the tiled one nearly vanished. They are all balanced
now: you hear someone coming just as clearly on any floor.

The download is half the size, down from 60 MB to 29 MB, with no audible loss of quality.

## 0.18.1

- **Sabotaging communications now closes the cameras for whoever was watching.** The station used to
  shut down on the server while the player stayed stuck in it: standing still, hearing another room,
  unable to walk.
- **The security recording plays footsteps again.** It had been running silent since 0.16.0, and
  without the footsteps there was nothing to count.
- The recording can now have one to **six** people walking past, answered with keys 1 to 6.

## 0.18.0

**Anyone inside a vent no longer shows up on the cameras.** The list of who is in the watched room
also counted the hidden impostor, which gave away for free exactly the deduction vents exist to deny:
if the camera says someone is in an empty room, everybody knows what that means. The watched room now
shows only the people actually walking through it.

## 0.17.5

- In the waiting room, **P** (who is here) and **C** (the match rules) now answer for whoever just
  created the room. Before, they stayed silent until a second person joined.
- Opening the message box with **Y** now puts the cursor straight inside it: you could type and press
  Enter with nothing happening, because Tab had to be pressed first. The same applied to the username
  field on a first login, the match name when creating a room, and the volumes in settings.

## 0.17.4

In the security task where you review the recording, the tape was still rewinding when the first
footsteps started — and a footstep on top of the tape noise is exactly the one you cannot count. The
recording now starts only after the tape stops.

## 0.17.3

The death sound now plays **only for the person who died**. The killer and anyone nearby hear the
kill sound instead, which gives away the direction of the crime without saying who did it. Before,
both sounds played for everyone at once: it gave the kill away for free to anyone nearby and, to the
victim, it sounded as though somebody else had died.

## 0.17.2

Two fixes to the camera station:

- The radar key now says who is in the **watched room**. Before, it answered for the room your own
  character was standing in, which made the cameras useless for looking for someone.
- Rooms are now in alphabetical order, and the game announces the position ("Cafeteria, 3 of 11").
  Right arrow moves forward, left arrow goes back to the previous room, and the list wraps around at
  the ends — you can learn by heart how many presses each room takes.

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

Maintenance release. Nothing changes for players.

## 0.15.0

- **Sabotage no longer affects ghosts.** After dying you keep the radar, your task markers and body
  sounds. Sabotage exists to pressure those who still have something to lose.
- When the game closes because of an error, it can now record what happened in more situations — and
  it tells you when it could not, instead of leaving you looking for a file that is not there.
- Updating no longer leaves files behind in the game folder.

## 0.14.2

New sound for the start of watering, in the greenhouse task.

## 0.14.1

- **Matches of up to 15 players** (was 10), with five new colours.
- Automatic updates now work on computers where they used to fail.

## 0.13.0

- In the waiting room, **P** says who is there and **C** says the match rules.
- The lobby list shows impostors, kill cooldown and sabotage on each entry.
- Lobby creation gained fine adjustments over the preset: players, impostors, tasks, meeting timers
  and sabotage.

## 0.12.0

- **Anyone can translate the game** into another language, and it shows up in the language list
  without needing a new version. Anything untranslated shows in English. The manual explains how.
- Warnings coming from the server now show in your own language.
- Fixed the registration messages, which were showing untranslated.

## 0.11.1

- The game now opens in **English** by default; anyone who prefers another language switches once in
  Settings and the choice sticks.
- Fixed the manual about vents: you get in with **Enter**, and **V** only serves to leave or travel
  once you are already inside.

## 0.11.0

**New: send a message to the people who make the game**, straight from the lobby list. The version,
what you were doing and the error log are sent along automatically.

## 0.10.1

Fixed the **T** key during meetings: it did not respond.

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
