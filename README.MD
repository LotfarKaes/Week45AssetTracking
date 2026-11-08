# Week 4–5 — Asset Tracking System (Levels 1–4)

Console application in **C#** and **.NET 8** for the *Weekly Mini Project — Asset Tracking* assignment.

**Current milestone:** through **Level 4**. Push each level as its own commit. **Level 5** (JSON files) is next.

---

## Run

```bash
dotnet run
```

---

## Progress

| Level | Status | Highlights |
|-------|--------|------------|
| **1** | Done | `Asset`, `Computer`, `MobilePhone`, `List<Asset>` |
| **2** | Done | Sort by type + date, end-of-life colors |
| **3** | Done | Offices, USD → SEK/USD/TRY, sort by office |
| **4** | Done | **Unique IDs**, **age**, **menu**, interface + abstract base |
| **5** | Next | Save/load JSON, export report |

---

## Level 4 — OOP design, IDs, age, menu

### OOP (assignment requirements)

| Concept | In this project |
|---------|------------------|
| **Abstract class** | `Asset` — shared fields and end-of-life logic |
| **Inheritance** | `Computer`, `MobilePhone` extend `Asset` |
| **Polymorphism** | `List<Asset>` holds different types; `ICompanyAsset` for display/search |
| **Interface** | `ICompanyAsset` — `Id`, `AssetType`, `AgeDescription`, `ToDisplayLine()` |

### Unique asset ID

- Every new asset gets `#1`, `#2`, … from `AssetManager` (`_nextId`).
- Shown on each line and used for view / edit / remove.

### Age calculation

- `TimeSpan Age` — days since purchase.
- `AgeDescription` — e.g. `1 year, 2 months` (uses `DateTime` + `TimeSpan`).

### Menu system

| # | Action |
|---|--------|
| 1 | Add asset |
| 2 | View — sort by **type → date** (Level 2) |
| 3 | View — sort by **office → date** (Level 3) |
| 4 | Search brand/model |
| 5 | View details by **ID** |
| 6 | Edit by **ID** |
| 7 | Remove by **ID** |
| 8 | Statistics |
| 9 | Refresh currency rates |
| 0 | Exit (`exit` works on any prompt too) |

---

## Project layout

```
Week45AssetTracking/
├── Program.cs                 # Menu loop (Level 4)
├── Models/
│   ├── ICompanyAsset.cs       # Level 4 interface
│   ├── Asset.cs               # Abstract base + age + ID
│   ├── Computer.cs
│   ├── MobilePhone.cs
│   └── … (office, EOL enums from earlier levels)
├── Services/
│   ├── AssetManager.cs        # IDs, search, stats
│   └── CurrencyService.cs
└── Helpers/
    ├── ConsoleUiHelper.cs
    └── InputHelper.cs
```

---

## Test Level 4

1. Load **sample data** — IDs `#1`–`#8`.
2. Menu **1** — add asset; note new ID (e.g. `#9`).
3. Menu **5** — view `#1`; check **Age** in output.
4. Menu **4** — search `Apple`.
5. Menu **8** — statistics.
6. Menu **0** or type **exit** at a prompt.

---

## Git — Level 4 commit

```bash
git add Week45AssetTracking/
git commit -m "Week45 Level 4: unique IDs, age, ICompanyAsset, menu system"
git push
```

---

## Earlier levels (still included)

- **Level 2:** 3-year lifespan, yellow/red EOL lines, type sort.  
- **Level 3:** `OfficeLocation`, `CurrencyService`, office sort.  

---

**Author:** Lotfar Kaes — Asset Tracking, Levels 1–4.

Say **Level 5** when ready for JSON save/load.