<img width="1000" height="266" alt="image" src="https://github.com/user-attachments/assets/292e3fee-5283-4f59-a556-4c1e1112c087" />


# Super Mega Baseball 4 League Import Tool (LIT)

The SMB4 League Import Tool is designed to import, and register custom leagues for **Super Mega Baseball 4**.  
If someone shares a custom league with you, or you create multiple leagues yourself, this tool integrates them cleanly into your save structure.

---

## What This Tool Does

### 🔍 Reads your SMB4 save directory
Point the tool at the folder containing your SMB4 saves (the one with `master.sav`).  
From there, it automatically detects:

- `master.sav`
- all `league-*.sav` files (including any custom ones you downloaded or created)

---

### 📋 Shows which leagues are registered in the game
SMB4 only loads leagues listed in `t_league_savedatas` inside `master.sav`.  
LIT displays:

- which leagues/franchises are **registered** (visible in SMB4)
- which leagues/franchises are **unregistered** (save exists, but SMB4 won’t load it)
- default leagues (Super Mega League, Legends League, Creators Classic), locked and labeled clearly

---

### ✔️ Lets you register or unregister leagues with one click
- Checking **Registered** adds the league’s GUID to `master.sav`.
- Unchecking it removes the entry.  
SMB4 will only load leagues and franchises marked as registered.

---

### 💾 Saves changes safely
Clicking **Save Changes** updates your `master.sav` with:

- correct league GUIDs  
- proper ordering (default leagues always first)  
- no duplicates  
- no invalid/missing entries  

LIT also warns you if you attempt to register a league whose `.sav` file is missing.

---

### 📤 Export individual league/franchise saves
Selecting a row and clicking **Export .sav** allows you to copy that save file to any location — useful for sharing leagues with others.

---

### 🧹 Automatic cleanup
All temporary SQLite files created during decompression are stored in `_smb4_temp` and are removed automatically when the tool exits.

---

## Notes
- LIT does **not** modify the contents of league files themselves — it only manages how SMB4 loads them.
- All operations are safe, isolated, and revertible by restoring your original `master.sav`.

---

## License
MIT License — see `LICENSE` for details.
