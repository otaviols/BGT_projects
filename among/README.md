# Among Us — Audiogame

A social deduction game played entirely by ear. You are a crewmate trying to finish the ship's
tasks, or an impostor trying to eliminate everyone without being found out.

Everything is spoken by the screen reader and positioned in space: you can tell where every person,
every object and every body is from sound alone.

> **Beta.** The game is playable from start to finish, but still under test. If something breaks,
> the `crash.log` file in the game folder records what happened — it helps a lot.

*Este manual também está disponível em português: [LEIAME.md](LEIAME.md).*

## Getting started

1. Open the game and choose **Connect** in the main menu. It already knows the server address;
   there is nothing to type.
2. Create an account (username and password) or sign in with one you already have. The game
   remembers the last user who signed in on this machine and puts the cursor straight on the
   password.
3. Pick a match from the list, or create your own.
4. In the waiting room, the host presses Enter to start. At least 3 players are required; the
   maximum is 15.

**Before playing:** the main menu has a **Learn the game sounds** option. It plays every sound in
the game along with its name. It is well worth going through it once — the whole game depends on
recognising these sounds.

## Updates

On startup the game checks whether a new version is out. If there is one, it tells you what changed
and asks whether you want to update. If you accept, the game **updates itself**: it closes,
downloads the new version, installs it and reopens. Nothing to download by hand, no folder to
unpack.

You can decline and keep playing the version you have — the question comes back next time you open
it.

Your settings and your account survive an update: preferences live in the user data folder, and your
account lives on the server.

If anything goes wrong along the way (the connection drops, say), the game is reopened on the
previous version, which is still intact, and you get a message explaining what happened. The old
version is only replaced once the new one has arrived in full and been verified.

With no internet the check simply does not happen and nothing is said.

## Sending feedback

The lobby list has a **Send a message to the people who make the game** option. Write what you
thought, what broke or what you would do differently — the game confirms once the message is stored.

Sent along automatically: the game version, the language, whether you were in a match, your role,
which room you were in, and the `crash.log` (the file that records why the game closed on its own,
if that happened). You do not have to note any of it down — that is exactly what usually goes
missing when trying to fix a problem.

While the game is in beta this is the most useful channel: one report with context is worth more
than ten "it didn't work".

## Settings

From the main menu, under **Settings**:

**Volumes** — one slider per sound family: master, ship ambience, footsteps, object beacons and
radar, tasks, deaths and alarms, and menus. Moving a slider plays a sample of that family at the new
volume, so you can tune everything by ear without entering a match. If other people's footsteps are
getting lost under the ambience, this is where you fix it.

**Keys** — every in-game key can be remapped. Pick the action, press Enter, then press the new key.
If the key is already taken, the game tells you which action owns it instead of letting two actions
fight over it. There is also "Restore default keys". The keys listed in this manual are the
defaults.

**Voice and screen reader** — choose the system voice, its rate and volume, and whether the game
should use the screen reader when one is present. This is what makes the game work for people
playing without NVDA: turn off "use screen reader" and speed the system voice up to taste.

**Language** — the list shows every installed language. The change takes effect immediately, and the
game opens in the chosen language next time. The game ships with US English and Brazilian
Portuguese, and opens in English the first time.

Everything is saved immediately and applies from the next match on.

## Translating the game

Anyone can add a language, with no programming and without waiting for a new release:

1. In the game's `lang` folder, copy `en_US.json` to a new file named after your language code —
   `fr_FR.json`, for instance.
2. Translate the **values** (what comes after the colon). The **keys** (what comes before) never
   change — they are what the game looks up.
3. On the first line, write the language's name **in that language itself**, in `language.name`.
   That is what shows up in the list: someone looking for their language recognises "Français", not
   "French".
4. Open the game. The language is already there under **Settings → Language**.

**You do not have to translate everything at once.** Anything missing falls back to English, so you
can translate gradually, and an older translation keeps working when the game gains new text.

If you ever see a raw key on screen (something like `menu.connect`), that text is missing from both
your file and English — worth sending a message about it.

## How the sound works

- Left and right you perceive through stereo, as usual.
- North and south are conveyed by **pitch**: anything south of you sounds lower. High pitch =
  north, low pitch = south.
- The closer something is, the louder it is.
- Each floor type has its own footstep sound (the cafeteria is wood, storage is a metal conveyor,
  the greenhouse is soil, and so on). You can tell which room you are in from your own footsteps
  alone — and "Learn the game sounds" has all of them separated out.
- Each type of object has its own beacon: a continuous sound at its location that gets louder as you
  approach. That is how you find things.
- A fallen body emits a continuous sound until someone reports it.

## Keys in the waiting room

| Key | Action |
|---|---|
| Enter | start the match (host only) |
| P | who is in the lobby (with the count, and which are bots) |
| C | this match's rules (impostors, cooldowns, timers, sabotage) |
| B | add a bot (host only, up to 8) |
| Shift + B | remove the last bot |
| Y | write in chat |
| Comma / Period | move through messages |
| ESC | leave the match |

## Keys during a match

**Movement and exploration**

| Key | Action |
|---|---|
| W / A / S / D | walk north / west / south / east |
| Enter | interact with whatever is closest |
| Tab | radar: next target |
| Shift + Tab | radar: previous target |
| Ctrl + Tab | switch radar mode (players / room objects) |
| C | say which room you are in |
| T | your task list, your progress and the team's |
| F1 | measure ping to the server |
| ESC | leave the match (asks for confirmation) |

**Crewmate**

| Key | Action |
|---|---|
| R | report a body (you must be near it) |
| Enter at the button | call an emergency meeting (one per player) |

**Impostor**

| Key | Action |
|---|---|
| K | kill whoever is in range |
| Enter (at a vent) | enter the vent |
| V (inside a vent) | leave it or travel to another |
| G | sabotage menu |
| F | lock a room's doors |

**Meeting and chat**

| Key | Action |
|---|---|
| B | open the voting menu |
| T | how much time is left in this phase |
| Y | write in chat (meetings only) |
| Comma / Period | previous / next message |
| Shift + Comma | first message |
| Shift + Period | last message |
| Page Up / Page Down | switch between messages and game events |

## The radar (Tab)

The radar has two modes, switched with **Ctrl + Tab**:

- **Players** — cycles through whoever is in the same room as you. Plays a beep at the person's
  position and speaks their name.
- **Room objects** — cycles through what exists in the room you are in (tasks, vents, panels,
  button). Useful for learning a room and knowing where everything is.

The radar does not work while communications are sabotaged, in either mode.

## Tasks

Each crewmate gets a few tasks (5 in the Classic preset). When **all** tasks of **all** crewmates
are done, the crew wins. Press **T** at any time to hear your progress and the team's.

The eleven tasks:

- **Fix wiring** — connect the pairs of wires with matching tones.
- **Download data** — hold Space until it finishes.
- **Empty garbage** — hold Space, then release and press again when you hear the beep.
- **Align engine output** — use the arrows to bring the sound to the center (both ears equal) and
  Enter to lock it in.
- **Swipe card** — press Enter between the two beeps, at the right rhythm.
- **Unlock manifolds** — listen to the tone sequence and repeat it with keys 1 to 4.
- **Fuel engines** — the panel first plays the **tone of a full tank**. Then hold Space: the filling
  sound rises, and you release when it reaches that same tone. Releasing early means too little
  fuel; going past it overflows. The game does not signal the moment — you judge it.
- **Clear asteroids** — each asteroid comes from one direction: left, **ahead** or right. The
  sound centred between both ears is the one from ahead; shoot with the matching arrow (left, **up**
  or right).
- **Roll the dice** (games room) — the panel asks for a number and you roll until you get it. The
  higher the result, the higher the dice sounds.
- **Water the seedlings** (greenhouse) — three beds: one on the left, one ahead and one on the
  right. The watering can starts at the middle one, and you carry it to the requested bed with the
  left and right arrows before pressing Space. Each move sloshes the water on the side the can went
  to — that is how you know where it is. Pressing Space at the wrong bed waters nothing.
- **Review records** (security) — listen to the corridor recording and count how many people walked
  past. Each one crosses from one side, with their own floor and rhythm. Answer with keys 1 to 4.

**ESC cancels any task**; it stays pending and you can come back later. If a meeting starts in the
middle of a task, it closes on its own and nothing is lost.

**Common tasks:** swipe card is a common task. Either **all** crewmates get it in that match, or
**none** do — never just some, and it exists in only one place on the ship. Remember this: if
somebody claims they were swiping the card in a match where the card never showed up on your list,
that person is lying.

**Died? Keep doing your tasks.** Ghosts still count towards the crew's victory, walk through locked
doors, and are the team's best weapon once things start going wrong. A ghost passes through a closed
door but **not** through a wall — and hears the bump against it, so it can still find its way.

## Meetings and voting

A meeting starts when someone reports a body or presses the emergency button. Everyone is moved to
the cafeteria and movement locks.

Press **Y** to talk and **B** to open the voting menu when you are ready. Closing the voting menu
with ESC does **not** spend your vote: you can open it again. Voting ends as soon as everyone has
voted, or when time runs out (75 seconds in the Classic preset).

**During a meeting, T tells you how much time is left** — and which phase it is: still discussion,
or voting already running. Outside a meeting, T is your task list as always.

Chat only exists in the waiting room and during meetings. There is no chat during the match — not
even between impostors.

## Sabotage (impostor)

Three sabotages, in the **G** menu. One at a time, with a 30-second cooldown, and the same one
cannot be repeated twice in a row.

- **Lights** — bodies go silent, the radar only reaches whoever is right next to you, general
  hearing range drops and the emergency button stops working. Lasts until someone fixes the panel in
  Electrical.
- **Oxygen** — critical sabotage. The crew has 90 seconds to fix **two** panels, one in Admin and
  one in Electrical, or they lose the match. While it is active, nobody can call a meeting or report
  a body. The game announces the time left every 30 seconds, and counts down one by one over the
  **last 10** — if you hear the countdown start, there is no time to switch panels: finish the one
  you began.
- **Communications** — takes down the radar (both modes), your task beacons and the nearby-body
  warning. Lasts until someone fixes the panel in Navigation.

**Doors (F)** — locks all of a room's corridors for 12 seconds, with a 25-second cooldown. There is
no fixing them: the doors reopen on their own. It is not a sabotage, so you can lock a room **and**
sabotage at the same time. Ghosts walk through locked doors.

**Vents** — impostor only. To **get in**, stand next to a vent and press **Enter**, like any other
object. Once **inside**, **V** opens the options: step out where you are, or travel to another vent
on the network. Entering a vent removes you from the map. The network links Navigation, Weapons and Reactor — the three
opposite corners of the ship, with no direct corridor between them. Vanish from one corner and
appear in another within seconds: it is a trip nobody can make on foot, which is exactly why it
works as an alibi.

## How you win

**The crew wins if:**
- all tasks from everyone are completed; or
- all impostors are ejected.

**Impostors win if:**
- they equal or outnumber the living crewmates; or
- oxygen runs out without being fixed.

## Bots

In the waiting room the host can add bots with **B** (up to 8) and remove them with **Shift + B**.
They walk the ship, do tasks, vote in meetings and can be drawn as impostors — when they are, they
hunt, kill when nobody is around, and sabotage. Useful to fill out a match or to practise alone.

## Match presets

| Preset | Players | Impostors | Tasks | Kill cooldown | Sabotage |
|---|---|---|---|---|---|
| Classic | up to 10 | automatic | 5 each | 25 s | on |
| Fast | up to 6 | 1 | 3 each | 15 s | off |
| Chaos | up to 10 | 3 | 4 each | 12 s | on |

The presets below are the starting point; a match tops out at **15 players**.

**You can adjust the preset when creating the lobby.** Besides the preset, the creation screen has
fields for maximum players, number of impostors, tasks per crewmate, kill cooldown, discussion time,
voting time, emergency meetings per player, and whether sabotage is in play.

**Every field may be left blank**, and blank means "use what the preset says" — so if you just want
to play, pick a preset, confirm and go. Anyone joining can hear what you changed by pressing **C**,
and anyone browsing the list already sees the summary.

---

## For developers

Written in [NVGT](https://nvgt.gg), an AngelScript engine for audio games.

### Layout

| Path | What it holds |
|---|---|
All the game code lives under `src/`. What is left at the top level is either an entry point, data
the game reads at runtime, or tooling — so the root stays readable as the project grows.

| Path | What it holds |
|---|---|
| `AmongUs.nvgt` | client entry point |
| `server_main.nvgt` | dedicated server entry point |
| `src/config/` | constants, player settings, keybindings, presets |
| `src/core/` | game state, player, client state, crash log, updater |
| `src/game/` | match loop, map, roles and the eleven task minigames |
| `src/network/` | ENet client and server, packet protocol |
| `src/ui/` | menus, lobby, meeting and settings screens |
| `src/database/` | SQLite account storage (server side) |
| `src/audio/` | sound catalog and spatial audio manager |
| `src/i18n.nvgt` | the translation engine |
| `lang/` | translation DATA only (`pt_BR.json`, `en_US.json`) |
| `sounds/` | source audio; packed into `sounds.dat` at build time |
| `tools/` | sound pack builder, sound checker, bot harness |
| `infra/` | Terraform, Dockerfile and Kubernetes manifests |

`lang/` stays outside `src/` on purpose: it is data the game opens by path at runtime, and that path
has to be the same whether you run from source or from a compiled build. The translation *engine* is
code, so it lives in `src/` — keeping the two apart is what lets the whole `lang/` folder be bundled
without shipping source alongside it.

### Building

```
nvgt tools/build_pack.nvgt          # regenerate sounds.dat from sounds/
nvgt -c -plinux server_main.nvgt    # server, for the container
nvgt -c AmongUs.nvgt                # client, for players
```

`sounds.dat` must exist before compiling the client — it is a `#pragma asset`, and the build stops
with `File not found: sounds.dat` if it is missing.

### Running a local server

```
nvgt server_main.nvgt
```

The client connects to the address in `DEFAULT_SERVER_HOST` (`src/config/game_constants.nvgt`). To point
a build at another server without recompiling, drop a `server.txt` next to the game — see
`src/config/server_address.nvgt`.

### Deploying

See [infra/README.md](infra/README.md). The server runs as a container on an AKS cluster and the
client is published to a static website on Azure Storage:

```
infra\deploy.ps1 -StorageAccount <name>
```

### Releasing a version

1. Bump `GAME_VERSION` in `src/config/game_constants.nvgt`.
2. Update `infra/site/version.json` with the same version and the release notes.
3. Build and run `deploy.ps1`.

The two versions must match: `version.json` is what installed clients compare themselves against, so
if it lags behind, nobody is told there is an update.
