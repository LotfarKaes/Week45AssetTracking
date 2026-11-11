# Company Asset Tracking System

A **C# console application** (.NET 10) that helps a company track laptops, desktops, and mobile phones across offices in **Sweden**, the **USA**, and **Turkey**. It was built step by step for the *Weekly Mini Project — Asset Tracking* (Levels 1–5).

The app uses **object-oriented design**, **LINQ**, colorized console output, and local **JSON** storage so data survives between runs.

**Author:** Lotfar Kaes

---

## How to run

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Open a terminal in this folder and run:

```bash
dotnet build
dotnet run
```

Type **`exit`** at any prompt (also `quit` or `q`) to leave safely. The app **auto-saves** when you quit.

---

## Key features

- **Add and manage assets** — computers (laptop/desktop) and mobile phones with brand, model, purchase date, and USD price.
- **Multi-office** — each asset belongs to an office; local price is calculated in **SEK**, **USD**, or **TRY**.
- **End-of-life alerts** — 3-year lifespan; rows turn **yellow** (< 3 months left) or **red** (< 6 months).
- **Sorting and search** — by type, office, or price; search by brand, model, or office.
- **Unique IDs and age** — every asset has `#id` and a readable age (e.g. `1 year, 2 months`).
- **Persistence** — `assets.json` with auto-save; optional text report export.
- **Live or fallback FX** — European Central Bank rates when online; built-in rates offline.

---

## Complete project layout

```
Week45AssetTracking/
├── Program.cs                          # Main menu and user actions
├── Week45AssetTracking.csproj          # .NET 10 console project
│
├── Models/
│   ├── Asset.cs                        # Abstract base (lifespan, age, EOL logic)
│   ├── ICompanyAsset.cs                # Interface for display / polymorphism
│   ├── Computer.cs                     # Laptop or desktop
│   ├── MobilePhone.cs
│   ├── AssetTypeKind.cs                # Enum for sorting by type
│   ├── EndOfLifeStatus.cs              # Normal / yellow / red / expired
│   ├── OfficeLocation.cs               # Sweden, USA, Turkey
│   ├── OfficeInfo.cs                   # Office names and currency codes
│   └── AssetCatalogFile.cs             # JSON DTOs (Level 5)
│
├── Services/
│   ├── AssetManager.cs                 # List, IDs, LINQ, statistics, validation
│   ├── CurrencyService.cs              # USD → local conversion (ECB XML)
│   └── AssetFileService.cs             # Save/load JSON, export .txt report
│
└── Helpers/
    ├── ConsoleUiHelper.cs              # Colors, banners, EOL highlighting
    └── InputHelper.cs                  # Validation, exit handling, office picker
```

Generated at runtime (gitignored): `assets.json`, `asset-report.txt`

---

## Project architecture

Think of the app in three layers:

| Layer | Responsibility |
|-------|----------------|
| **Models** | Data and rules: what an asset is, how age and end-of-life are calculated |
| **Services** | Behavior: store the list, convert currency, read/write files |
| **Helpers** | Console UX: colors, friendly errors, input checks |
| **Program.cs** | Thin “front desk”: shows the menu and calls the services |

**OOP in practice**

- `Asset` is **abstract**; `Computer` and `MobilePhone` override type labels and sorting.
- The register is a **`List<Asset>`** — one list, many real types (polymorphism).
- `ICompanyAsset` gives a small, clear contract for showing asset details.

**Data flow (simplified)**

```
User input → InputHelper → AssetManager → Models
                ↓                ↓
         ConsoleUiHelper    AssetFileService → assets.json
```

---

## What I built (my contribution)

I implemented the full assignment in one coherent codebase, pushed **level by level** in git:

| Level | What I delivered |
|-------|------------------|
| 1 | Classes, constructors, `List<Asset>` |
| 2 | LINQ sort + `DateTime`/`TimeSpan` end-of-life colors |
| 3 | Offices, currency conversion, sort by office |
| 4 | Unique IDs, age, menu, interface + abstract base |
| 5 | JSON save/load, auto-save, export, duplicate ID handling |

On top of the rubric, I added **stronger validation** (brand/model required), **colored menus and statistics**, **search methods wired to the menu** (`FindByBrand`, `FindByModel`, `GetAssetsByOffice`), and **price sort + total USD** (menu 13) so the UX matches what teachers often look for from earlier product-list projects.

---

## Main menu (quick reference)

| # | Action |
|---|--------|
| 1 | Add asset |
| 2 | View — type → purchase date |
| 3 | View — office → purchase date |
| 4 | Search — brand / model / office / combined |
| 5–7 | View, edit, or remove by ID |
| 8 | Statistics (totals, EOL counts) |
| 9 | Refresh currency rates |
| 10–11 | Save or load `assets.json` |
| 12 | Export `asset-report.txt` |
| 13 | View — sort by USD price + list total |
| 0 | Exit (auto-save) |

---

## Challenges and solutions

| Challenge | How I solved it |
|-----------|-----------------|
| **Different asset types in one list** | Abstract `Asset` + subclasses; sort using `GetAssetTypeKind()` |
| **Fair EOL warnings** | One place: `GetEndOfLifeStatus()` using 3-year rule from the PDF |
| **Currency without breaking offline runs** | Try ECB XML first; fall back to fixed rates if the network fails |
| **Saving phones and computers in one file** | JSON records with `assetKind` + `isLaptop` for computers |
| **IDs after reload** | Store `nextId` in JSON; skip duplicate IDs on load with clear messages |
| **Empty or bad input** | `TryAdd*` methods and `ReadRequiredText` — no “asset without a name” |

---

## Future improvements

Ideas I would add with more time (not required for the assignment):

- **Tablet** category as in the optional PDF extension.
- **Unit tests** for end-of-life dates and currency conversion.
- **Pagination** for very long asset lists.
- **Export to CSV** for Excel.
- **Edit office** on an existing asset and recalculate local price in one step.

---

## Troubleshooting

| Problem | Try |
|---------|-----|
| `dotnet` not found | Install .NET 10 SDK and open a new terminal |
| No colors | Use **Windows Terminal** or a modern console |
| Live rates fail | Expected offline — fallback rates still work |
| Bad JSON file | Fix the file or delete `assets.json` and start fresh / load sample data |

