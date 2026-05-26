<img width="1000" height="266" alt="image" src="https://github.com/user-attachments/assets/292e3fee-5283-4f59-a556-4c1e1112c087" />


# Super Mega Baseball 4 League Import Tool (LIT)

**SMB4 League Import Tool**, or **LIT**, is a save-management utility for **Super Mega Baseball 4**.
It helps you register, unregister, manage, and export custom league/franchise save files so SMB4 can actually see them in-game.
Pretty much, if you have a `league-*.sav` file that exists in your save folder but does not show up in SMB4, this tool helps connect it to your `master.sav` registration list.

---

## What This Tool Does

LIT reads your SMB4 save folder, finds your `master.sav`, scans your `league-*.sav` files, and shows which leagues/franchises are currently registered. 
SMB4 only loads leagues and franchises that are listed in the registration table inside `master.sav`.

LIT lets you:
- See registered and unregistered leagues/franchises
- Register or unregister saves with one checkbox
- Export individual `league-*.sav` files for sharing
- Detect missing save files
- Detect filename/internal GUID mismatches
- Repair mismatched filenames after asking first
- Save changes with timestamped backups
- Open diagnostic logs when troubleshooting

---

## What This Tool Does **Not** Do

LIT does **not** edit the contents of your league or franchise saves.

It doesn't modify:

- Rosters
- Players
- Teams
- Uniforms
- Stats
- Franchise progress
- League rules
- Gameplay data

It mainly updates which league/franchise save files SMB4 should load from `master.sav`.

---

## Quick Tutorial

### 1. Close Super Mega Baseball 4

Do this before editing saves. No hero ball here.

### 2. Back up your saves

The tool creates backups when saving changes, but you should still keep your own backup.

### 3. Launch the tool

Run:

```text
SMB4LeagueImportTool.exe
```

### 4. Select your SMB4 saves folder

Click:

```text
Select SMB4 Saves Folder
```

Choose the folder that contains:

```text
master.sav
league-*.sav
```

For Steam installs, the folder is commonly somewhere under:

```text
Steam\userdata\<SteamUserId>\1487210\remote
```

`1487210` is the Steam App ID for Super Mega Baseball 4.

### 5. Load your leagues/franchises

Click:

```text
Load All League/Franchise Saves
```

The tool will scan your saves folder and show:
- Default leagues
- Custom leagues
- Franchise saves
- Registration status
- League names
- GUIDs
- Save filenames

### 6. Check or uncheck Registered

Checked:

```text
The league/franchise should appear in SMB4.
```

Unchecked:

```text
The save file stays on disk, but SMB4 should not load it from the registration list.
```

### 7. Click Save Changes

The tool updates `master.sav` and creates a timestamped backup.

Backup files look like:

```text
master.sav.20260403-153012-123.bak
```

### 8. Launch SMB4 and verify

Open SMB4 and confirm your leagues/franchises appear as expected.

---

## Features

### Register / Unregister Leagues and Franchises

Use the **Registered** checkbox to control whether SMB4 loads a league or franchise.
The save file itself remains in your folder either way.

---

### Safer `master.sav` Saving

When saving changes, LIT:

- Rebuilds the registration list
- Keeps default leagues first
- Avoids duplicate entries
- Warns about missing save files
- Creates timestamped backups
- Avoids directly overwriting the live save file mid-process

This doesn't make save editing magically risk-free, but it is much safer than raw manual editing.

---

### Missing Save Detection

If `master.sav` references a league/franchise file that is missing, LIT will warn you. That helps avoid broken registrations where SMB4 expects a save file that no longer exists.

---

### Filename Repair

Each `league-*.sav` file has an internal GUID. 
Sometimes files are copied, renamed, exported, or shared manually, and the filename no longer matches the internal GUID.

LIT can detect this and ask whether you want to repair the filename. It does **not** silently rename files behind your back.

---

### Export Saves

Select a league or franchise row and click:

```text
Export .sav file
```

This copies the selected save file to another location for sharing or backup. It doesn't modify the original file.

---

### Logs

LIT writes diagnostic logs to help troubleshoot issues.

Logs include:

- Tool version
- Windows/.NET version
- Selected saves folder
- Load/save/export events
- Filename repair results
- Cleanup events
- Errors and exceptions
- Wine/Proton environment details when applicable

On native Windows, logs are usually stored here:

```text
%LOCALAPPDATA%\SMB4LeagueImportTool\Logs\SMB4LeagueImportTool.log
```

You can also click:

```text
Open Logs Folder
```

inside the tool.

---

## Steam Cloud Warning

Steam Cloud may overwrite local save changes after you edit `master.sav`.

Recommended workflow:
1. Close SMB4.
2. Back up your saves.
3. Temporarily disable Steam Cloud if needed.
4. Use LIT.
5. Launch SMB4 and confirm your saves work.
6. Re-enable Steam Cloud only after confirming everything looks right.

Steam Cloud is useful, but when you are editing save files manually, it can also be the helpful friend who puts your tools back in the wrong drawer.

---

## Linux / Steam Deck / Wine / Proton Notes

LIT is still a Windows application. There is no native Linux build yet.

However, v0.19 includes best-effort compatibility improvements for users running the tool through Wine, Proton, Steam Deck, or Protontricks.

The tool can now:
- Detect when it appears to be running under Wine or Proton
- Log Wine/Proton environment details
- Detect the expected Super Mega Baseball 4 Steam App ID: `1487210`
- Warn if it appears to be running inside the wrong Protontricks prefix
- Use a more Linux-friendly log path when possible
- Fall back to the Wine prefix's Windows-style LocalAppData path if needed

For Protontricks users, choose:

```text
Super Mega Baseball 4 (1487210)
```

When browsing for saves on Linux or Steam Deck, your SMB4 save folder may appear through Wine as something like:

```text
Z:\home\<user>\.local\share\Steam\userdata\<SteamUserId>\1487210\remote
```

or:

```text
Z:\home\<user>\.steam\steam\userdata\<SteamUserId>\1487210\remote
```

Exact paths may vary depending on Steam install location, Steam Deck setup, Flatpak installs, custom Steam libraries, and Wine/Proton configuration.

This is compatibility support, not native Linux support.

---

## Troubleshooting

### The tool says `master.sav` was not found

You probably selected the wrong folder.

Pick the folder that directly contains:

```text
master.sav
```

---

### My league/franchise does not appear in SMB4

Check that:

- The save is registered in LIT
- The matching `league-*.sav` file exists
- SMB4 was closed when you saved changes
- Steam Cloud did not overwrite your local saves

---

### The tool warns about a missing save

That means `master.sav` references a save file that is not present in the selected folder.

You can either restore the missing `league-*.sav` file or unregister the missing entry.

---

### The tool warns about Steam Cloud

Steam Cloud appears to be enabled.

Back up your saves and consider disabling Steam Cloud temporarily while editing.

---

### The tool behaves strangely under Wine/Proton

Use the logs.

Click:

```text
Open Logs Folder
```

or manually check the log path shown by the tool.

When reporting an issue, include:

- Tool version
- OS/platform
- Whether you used Windows, Wine, Proton, Steam Deck, or Protontricks
- Whether Steam Cloud was enabled
- What you were trying to do
- Relevant log contents

---

## Download

Download the latest release from the GitHub Releases page.

Use the packaged ZIP file, extract it, then run:

```text
SMB4LeagueImportTool.exe
```

Do not run the EXE directly from inside the ZIP.

---

## License

Released under the MIT License.
See `LICENSE` for details.

---

## Disclaimer

This is a community tool and is not affiliated with, endorsed by, or supported by Metalhead Software, Electronic Arts, or Super Mega Baseball 4. Use at your own risk, and always back up your saves.

