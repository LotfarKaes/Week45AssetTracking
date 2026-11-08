# Week 45 — Asset Tracking System (Complete — Levels 1–5)

Console application in **C#** and **.NET 10** for the *Weekly Mini Project — Asset Tracking* assignment.

All five levels are implemented in this folder.

---

## Run

```bash
dotnet run
```

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

---

## Level summary

| Level | Topic |
|-------|--------|
| **1** | `Asset`, `Computer`, `MobilePhone`, `List<Asset>` |
| **2** | Sort by type + date, end-of-life yellow/red |
| **3** | Offices SEK/USD/TRY, currency conversion |
| **4** | Unique IDs, age, `ICompanyAsset`, menu |
| **5** | **JSON** save/load, **auto-save**, **export report**, duplicate ID checks |

---

## Level 5 — File management

### `assets.json`

The app stores:

- **`nextId`** — next free ID (IDs are never reused after load).
- **`assets`** — each row with `assetKind` (`Computer` or `MobilePhone`), fields, and `isLaptop` for computers.

Example:

```json
{
  "nextId": 9,
  "assets": [
    {
      "assetKind": "Computer",
      "id": 1,
      "brand": "Apple",
      "model": "MacBook Pro",
      "purchaseDate": "2024-03-15T00:00:00",
      "priceUsd": 1500,
      "priceLocal": 15750,
      "office": "Sweden",
      "isLaptop": true
    }
  ]
}
```

Uses **`System.Text.Json`** with indented output and enum names for offices.

### When data is saved

- **Auto-save** after add, edit, remove, sample load, currency refresh, and on **exit**.
- **Manual save** — menu **10**.
- **Load** — menu **11**, or **on startup** if `assets.json` exists.

### Duplicate asset IDs

When loading, the app:

- Skips **duplicate IDs inside the same file** (with a warning).
- Keeps **`nextId`** at least `max(id) + 1` so new assets never collide.

### Text report — `asset-report.txt`

Menu **12** writes a plain-text report (sorted by office, then date), including end-of-life status lines where relevant.

`assets.json` and `asset-report.txt` are listed in `.gitignore` so personal test data stays local.

---

## Main menu (Level 5)

| # | Action |
|---|--------|
| 1 | Add asset |
| 2 | View — type → date |
| 3 | View — office → date |
| 4 | Search |
| 5 | View by ID |
| 6 | Edit by ID |
| 7 | Remove by ID |
| 8 | Statistics |
| 9 | Refresh currency rates |
| **10** | **Save JSON** |
| **11** | **Load JSON** |
| **12** | **Export report** |
| 0 | Exit (auto-save) |

Type **`exit`** on any prompt to quit safely.

---

## Project layout

```
Week45AssetTracking/
├── Program.cs
├── Models/
│   ├── AssetCatalogFile.cs    # Level 5 DTOs
│   ├── Asset.cs
│   ├── ICompanyAsset.cs
│   └── …
├── Services/
│   ├── AssetManager.cs        # ToCatalogFile, LoadFromCatalog
│   ├── AssetFileService.cs    # Level 5
│   └── CurrencyService.cs
└── Helpers/
```

---

## Test Level 5 (grading)

1. Run app → load sample data → confirm `(Auto-saved to assets.json)`.
2. Exit and run again → choose **y** to load `assets.json` → same assets and IDs.
3. Menu **12** → open `asset-report.txt`.
4. Menu **7** remove an asset → check JSON updated after auto-save.
5. Manually duplicate an `id` in JSON → load → warning and skip duplicate.

---

## Git — Level 5 commit

```bash
git add Week45AssetTracking/
git commit -m "Week45 Level 5: JSON save/load, auto-save, export report, duplicate ID checks"
git push
```

---

## Troubleshooting

| Issue | Fix |
|-------|-----|
| Load failed | Check JSON is valid; use menu 10 after a good run to reset file. |
| Auto-save failed | Folder write permissions; use menu 10. |
| Empty after load | File may be `[]` / empty assets — load sample or add assets. |

---

**Author:** Lotfar Kaes — Asset Tracking, Levels 1–5 (complete mini project).