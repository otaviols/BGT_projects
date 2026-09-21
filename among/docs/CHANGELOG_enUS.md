# Changelog

Everything that changed in the game, newest first.

*Também disponível em português: NOVIDADES.md, nesta mesma pasta.*

---

## 0.29.4

- Fixed: ghosts started hearing the sabotage alarm again after a meeting, and kept hearing it when
  they died in the middle of one. Sabotage no longer bothers the dead at any point.

## 0.29.3

- **Fixing a sabotage as a group now makes sense.** The panel closes by itself when somebody solves
  it before you, an already fixed panel says so instead of letting you type the code for nothing,
  and this works for both oxygen and comms.
- **The medbay scanner takes one player at a time.** If someone is being scanned, the game says it is
  busy instead of letting everyone pile in and scramble the scan.
- Ghosts are no longer bothered by sabotages: no more alarm, and no more being offered a panel they
  cannot fix.
- Fixed for good: players who did not press Enter on the results screen were still left out of the
  next round. The screen already closed by itself, but the notice that the match had started was
  being discarded right afterwards.
- Fixed: opening the settings from inside the waiting room dropped you out of it. The screen held the
  game without talking to the server, and in under twenty seconds it concluded you had disconnected.
- The **speech and screen reader** screen was rewritten and now speaks your language — it used to be
  the engine's default screen, always in English. Picking a voice or changing the rate now speaks a
  sample right away.

## 0.29.2

- **One account, one session.** Signing in with the same account somewhere else disconnects the
  previous session, which hears why before dropping. Before, the same name could be connected as
  many times as it liked.
- Fixed: players who left the results screen open without pressing Enter were not pulled into the
  next round. They were left hearing the match happen without them, unable to play. The screen now
  closes by itself when the new match starts.
- Fixed: with the "sabotaged comms silence the meeting" rule **off**, voices were still cut during
  meetings anyway.
- **Dead impostors can sabotage again**, as in the original game. It is what they have left — killing,
  venting and locking doors end with death — and without it, dying early as an impostor meant
  becoming a spectator.

## 0.29.1

- **A volume of its own for the sabotage alarm** — the one that keeps ringing while a sabotage is
  up. It is the only sound in the game that never stops, and until now turning it down also turned
  down deaths, body reports and victory, which share the same category.

## 0.29.0

- **Tasks are now short, long or common**, and the list (T key) says which is which — you can plan
  your route knowing whether a task costs a trip across the ship or is solved on the spot. The host
  chooses **how many of the tasks are long** in the room rules.
- **A new room: the oxygen room**, opening off the cargo corridor between storage and electrical.
  One of the two oxygen panels moved there from electrical — with both in the same room, cutting the
  lights and cutting the air were the same run to the same place. It has its own floor and ambience,
  so you know you walked into it from the footsteps alone.
- **A second vent network**, linking the corridor between the games room and the greenhouse to the
  security corridor — the north and mid-east of the map had no vents at all. The two networks are
  separate: you never come out of one into the other. And these are CORRIDOR vents, where people
  walk past: the lid opening is heard by anyone crossing, so it is a fast route that is risky to use.
- **Multi-step tasks.** "Fuel Engines" is now picking up the can in storage and carrying it to the
  reactor: the beacon moves on its own, the game tells you where to go, and the task list shows
  which step you are on and where it is. Each step has its own beacon, so you can tell by ear where
  you pick up from where you deliver. Being interrupted by a meeting midway loses nothing — you
  come back holding the can.
- **F5 refreshes the list**, both in the hall and in the match list. The hall also says who is there
  as soon as you open it.
- **Your microphone volume** in the voice chat settings, for when you come through too quiet or too
  loud on the other side.
- **F3 opens the settings from inside the waiting room**, with no need to leave and lose your spot.
  The ping key works there too now.
- Two new room rules: the **emergency button cooldown** (it was always 15 seconds) and **"sabotaged
  comms silence the meeting"**, which is on by default: sabotaging comms before a body turns up
  makes the whole discussion happen in text only. Hosts who prefer meetings to always have voice can
  turn it off in the room rules.
- With the **lights out**, the radar sees much less than before: now it only picks up players who
  are almost on top of you.

## 0.28.0

- **Hall.** The match menu now shows **who is online** outside the rooms and lets you **chat** with
  them — it's where you agree on who creates the match, instead of guessing whether anyone else is
  on the server.
- **The room now survives the match.** Everyone goes back together to the same waiting room, with
  the same rules, ready to play again. Before, the room ended with the match and the group had to
  reorganize every round.
- **Voice chat during the comms sabotage**: the radio goes out for every living player until the
  panel is fixed. Voices are never cut during a meeting.
- **More from the talk key**: hold Shift and the game says who is speaking right now; Ctrl opens the
  list to mute someone; and, for those using the automatic microphone, the key alone turns the
  microphone on and off.
- **Dead players now hear everyone with no distance**: other ghosts become a general chat, and the
  meeting comes through in full. Before, voices came from where each person was — and since a ghost
  stays where they died, that was almost silence.
- Fixed: dying during a sabotage left you blinded by it until someone fixed the panel. Sabotage no
  longer affects ghosts at any point.
- **You are no longer deaf inside a task.** Chat, sabotages and the oxygen countdown are now spoken
  while you are in a task, a repair panel or the message box — before, they only arrived when you
  closed the screen. Nothing drops the task for you: you get the warning, and leaving is your call.
- **Fixed the game going "not responding" mid-match.** It happened after spending a while on a
  screen that holds the game — a task, the cameras, the message box — during a busy match.
- **The radar tells you the direction again.** The beep's pitch now says whether the target is north
  (higher) or south (lower), both in the normal radar and in the locked one. Before, the pitch
  varied with distance and swallowed exactly the information sound alone cannot give.
- The wiring task confirms again when you connect the right pair.
- **Per-player voice volume**: in the player list (Ctrl plus the talk key) you can mute someone or
  just adjust their voice volume, from 0 to 300 percent. Saved by name, so it holds for later matches.

## 0.27.0

- **Remember me.** Tick the option on the sign-in screen and, next time, the game signs you in by
  itself. Your password is not stored on the computer: the server gives the game its own access,
  which you can cancel with "Sign out and forget this computer" in the match list.
- Voice chat: dead players now hear the meeting properly (the discussion came from far away,
  barely audible).

## 0.26.1

- Voice chat: other players' voices now come through clean and continuous. In 0.26.0 they
  sounded choppy and garbled for everyone.
- Typing the talk key in the chat box or in the room rules no longer opens the microphone.

## 0.26.0

- **Proximity voice chat.** Players near you on the ship hear what you say, from the side you're
  on; in the waiting room and during meetings everyone hears everyone. The dead only talk to the
  dead (but still hear everyone), and nobody hears you inside a vent. Hold **X** to talk — or, under
  Settings > Voice chat, let the microphone open by itself when you speak, with the sensitivity you
  prefer, and test your microphone before joining. The key can be changed under Keys. Ctrl plus the
  talk key opens the list to **mute** someone (they stay muted in later matches too). Voices have
  their own volume, and rooms have a "Voice chat" rule for those who prefer text only.
- **Choose the sound output device and the microphone**, under Settings.
- Fixed: when everyone voted for the same player and nobody skipped, the vote ended in a tie and
  nobody was ejected.
- Fixed: players who left the waiting room were still listed as being in the room.

## 0.25.2

Every task (and every repair) ends with the same completion sound. Before, each played a different
one at the end, on top of the right one.

## 0.25.1

- **When the server is about to update, you are warned first**: "the server will restart in so
  many minutes". No new match starts until then, and the ones in progress have that time to finish.
  Before, the server switched without warning and dropped everyone mid-game.
- If the connection to the server drops, the game now says so and returns to the main menu — before
  it sat frozen in an empty ship with no explanation.
- Chat: messages no longer interrupt speech, and the history cursor stays where you left it when a
  new message arrives. With the message box open you keep hearing the chat, and it closes on its own
  when the meeting ends.

## 0.25.0

- **Fair voting.** Someone is only ejected with more votes than "skip" and than anyone else.
  Before, nine people skipping and one voting got someone ejected.
- **Voting in two steps**: Enter marks a name, Enter again confirms. And a tick marks the last ten
  seconds of voting.
- **New room rule: open or secret votes.** Open, the game says who each player voted for as the
  vote is cast; secret, only that they voted.
- **Fixing sabotage takes work now.** Lights: switch the five breakers in Electrical back on — the
  panel says which are off. Oxygen: each panel speaks a five-digit code, the same on both.
  Communications: tune the radio with the arrows until the voice is free of static.
- New sounds: someone being ejected, each team's victory music, marking and confirming a vote, the
  repairs, and the right sounds for opening and closing tasks and menus.
- The radar lock (Q) now works in objects mode too. And the radar mode is back to just Ctrl + the
  radar key — the M key is gone.
- With communications sabotaged, the task list does not respond either.
- **Practice tasks and repairs**, in the main menu: any task or repair, alone, with no match. The
  "training mode" you asked for.

## 0.24.1

The medbay scanner's beacon got its final sound.

## 0.24.0

- **New task: the medical scan**, in medbay. Stand still on the scanner until it finishes. Anyone
  nearby hears the scanner running and the game says who is being scanned — since impostors cannot
  do tasks, it is proof that person is crew. Faking it produces no sound for others.
- **New room rule: say or not whether the ejected player was an impostor.** Off, every ejection
  becomes a doubt instead of an answer. It is in room creation and under the O key.
- New sounds in the asteroids task: the shot and the hit, coming from the right direction.

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
