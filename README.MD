# Week 4–5 — Asset Tracking System (Levels 1–2)

Console application in **C#** and **.NET 10** for the *Weekly Mini Project — Asset Tracking* assignment.

**Current milestone:** Level 1 + **Level 2**. Push Level 2 as its own commit after Level 1 is on GitHub. Level 3 (offices + currency) comes next.

---

## Prerequisites

1. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
2. Terminal in this folder:

```bash
dotnet run
```

---

## Assignment levels in this folder

| Level | Status | What it adds |
|-------|--------|----------------|
| **1** | Done | `Asset`, `Computer`, `MobilePhone`, `List<Asset>`, constructors |
| **2** | Done | Sort by type + date, 3-year lifespan, **yellow/red** EOL highlights |
| **3** | Next | SEK/USD/TRY, currency conversion |
| **4** | Planned | IDs, age, menu |
| **5** | Planned | JSON files |

---

## Level 2 — Sorting & end-of-life

### Business rules

- Every asset lives **3 years** from the purchase date.
- **Yellow** row: **less than 3 months** until end-of-life.
- **Red** row: **less than 6 months** left (but not in the yellow window).
- **Magenta-style** row: lifespan already **expired**.

### Sorting (LINQ)

Assets are shown in this order:

1. **Asset type** — Desktop, then Laptop, then Mobile Phone (`AssetTypeKind` enum).
2. **Purchase date** — oldest first within each type.

```csharp
_assets.OrderBy(a => a.GetAssetTypeKind()).ThenBy(a => a.PurchaseDate)
```

### Color output

`ConsoleUiHelper.WriteAssetLine` reads `asset.GetEndOfLifeStatus()` and applies console colors, then **restores** the default colors (so the rest of the text stays normal).

### Sample data

On startup you can load **sample assets** from the PDF (MacBook, ThinkPad, iPhone, etc.). Two extra rows are dated so you see **RED** and **YELLOW** warnings immediately without waiting three years.

### Program flow (Level 2)

1. Optional **sample data**.
2. **Add** computers or phones (same as Level 1).
3. Show **sorted list with color alerts**.
4. Ask whether to **continue** — add more or view again.

`exit` / `quit` / `q` still works on **any** input.

---

## Level 1 recap (still included)

| Field | Example |
|-------|---------|
| Brand | Apple |
| Model | MacBook Pro |
| Purchase date | 2024-03-15 |
| Price USD | 1500 |
| Price local | 15000 |
| Office | Sweden |
| Asset type | Laptop |

---

## Project layout

```
Week45AssetTracking/
├── Program.cs
├── Models/
│   ├── Asset.cs               # Lifespan + GetEndOfLifeStatus (Level 2)
│   ├── AssetTypeKind.cs       # Sort order
│   ├── EndOfLifeStatus.cs
│   ├── Computer.cs
│   └── MobilePhone.cs
├── Services/
│   └── AssetManager.cs        # Sort, sample data, highlighted list
└── Helpers/
    ├── ConsoleUiHelper.cs     # WriteAssetLine colors
    └── InputHelper.cs
```

---

## How to test Level 2 (for grading)

1. Run `dotnet run` and accept **sample data** (`y`).
2. Press **n** when asked to add more in the first session (or add your own assets).
3. Confirm the list is **grouped by type** and **sorted by date** within each group.
4. Find **RED** (Dell OptiPlex) and **YELLOW** (Google Pixel) lines in the sample set.
5. Type **`exit`** at a prompt — app closes without crashing.
6. Answer **n** to “Continue adding…” to finish.

---

## Git — Level 2 commit

```bash
git add Week45AssetTracking/
git commit -m "Week45 Level 2: sort by type/date, end-of-life yellow/red highlights"
git push
```

---

## Troubleshooting

| Problem | What to try |
|--------|-------------|
| No colors | Use Windows Terminal or a modern console. |
| No RED/YELLOW in sample | Run again with sample data `y`; demo dates are calculated from **today**. |
| `'dotnet' not recognized` | Install .NET 10 SDK and restart the terminal. |

---

**Author:** Lotfar Kaes — Asset Tracking, Levels 1–2.

When Level 2 is pushed, ask for **Level 3** to add official offices and currency conversion.