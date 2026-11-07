# Week 4–5 — Asset Tracking System (Levels 1–3)

Console application in **C#** and **.NET 8** for the *Weekly Mini Project — Asset Tracking* assignment.

**Current milestone:** Levels **1**, **2**, and **3**. Push each level as its own git commit. Level 4 (IDs, menu) is next.

---

## Prerequisites

```bash
dotnet run
```

(from this folder, with [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed)

---

## Progress

| Level | Status | Summary |
|-------|--------|---------|
| **1** | Done | Classes, `List<Asset>`, constructors |
| **2** | Done | Sort by type + date, end-of-life colors |
| **3** | Done | Offices SEK/USD/TRY, USD → local conversion, sort by office |
| **4** | Next | Unique IDs, age, menu system |
| **5** | Planned | JSON persistence |

---

## Level 3 — Multi-office & currency

### Offices

| Office | Currency |
|--------|----------|
| Sweden | SEK |
| USA | USD |
| Turkey | TRY |

Each asset is linked to an `OfficeLocation` enum value (not a free-text string).

### Currency conversion

1. You enter **price in USD** when adding an asset.
2. `CurrencyService` converts to the office currency:
   - **Live rates:** ECB daily XML (same idea as `LiveCurrency.cs` in the course pack).
   - **Fallback:** built-in rates if the network fails (app still runs offline).

After adding, the app shows the calculated local amount, e.g. `SEK 15,750.00`.

### Sorting (Level 3 requirement)

```csharp
_assets.OrderBy(a => a.OfficeDisplayName).ThenBy(a => a.PurchaseDate)
```

Choose **option 2** (or **3**) in the view menu after adding assets.

Level **2** sort (type → date) stays available as **option 1**.

### Refresh rates

When asked, the app can **re-download** ECB rates and **recalculate** every `PriceLocal` from stored USD values.

---

## Level 2 recap

- 3-year lifespan, **yellow** (&lt; 3 months), **red** (&lt; 6 months), expired styling.
- Sort: `AssetTypeKind` → purchase date.

---

## Level 1 recap

- `Asset`, `Computer`, `MobilePhone`, properties from the PDF.

---

## Project layout

```
Week45AssetTracking/
├── Program.cs
├── Models/
│   ├── Asset.cs
│   ├── OfficeLocation.cs      # Level 3
│   ├── OfficeInfo.cs
│   ├── AssetTypeKind.cs
│   ├── EndOfLifeStatus.cs
│   ├── Computer.cs
│   └── MobilePhone.cs
├── Services/
│   ├── AssetManager.cs
│   └── CurrencyService.cs     # Level 3
└── Helpers/
    ├── ConsoleUiHelper.cs
    └── InputHelper.cs         # ReadOfficeLocation()
```

---

## Test Level 3 (grading checklist)

1. Run with **sample data** — verify **SEK**, **USD**, **TRY** labels on each line.
2. Add a asset for **Turkey**, USD `900` — confirm **TRY** local price appears.
3. View list with **option 2** — assets grouped by **Sweden → Turkey → USA** (alphabetical office names).
4. Answer **y** to refresh rates — local prices update (or fallback message if offline).
5. Type **exit** during office selection — clean shutdown.

---

## Git — Level 3 commit

```bash
git add Week45AssetTracking/
git commit -m "Week45 Level 3: offices SEK/USD/TRY, currency conversion, sort by office"
git push
```

---

## Troubleshooting

| Issue | Fix |
|-------|-----|
| Live rates fail | Normal offline — fallback rates still convert. |
| USA local equals USD | Expected: USD office uses 1:1. |
| No colors | Use Windows Terminal. |

---

**Author:** Lotfar Kaes — Asset Tracking, Levels 1–3.

Reply **Level 4** when this commit is pushed.