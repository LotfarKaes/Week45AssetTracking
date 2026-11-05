# Week 4–5 — Asset Tracking System (Level 1)

Console application in **C#** and **.NET 10** for the *Weekly Mini Project — Asset Tracking* assignment.

This folder currently implements **Level 1 only**.

---

## Level 1 — What you build

| Requirement | How it is done |
|-------------|----------------|
| Classes & objects | `Asset`, `Computer`, `MobilePhone` |
| Constructors | Each class sets brand, model, dates, prices, office |
| `List<Asset>` | Hidden inside `AssetManager` |
| Asset fields from PDF | Brand, Model, Purchase Date, Price USD, Price Local, Office, Asset Type |

**Asset type examples**

- Computers: MacBook Pro (Laptop), ThinkPad (Laptop), desktop PCs  
- Phones: iPhone, Samsung Galaxy, Nokia  

---

## Prerequisites

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Open a terminal in this folder.

```bash
dotnet run
```

Build only:

```bash
dotnet build
```

---

## Project layout (Level 1)

```
Week45AssetTracking/
├── Program.cs                 # Add assets in a loop, then show the list
├── Models/
│   ├── Asset.cs               # Shared properties + abstract AssetType
│   ├── Computer.cs            # Laptop or Desktop
│   └── MobilePhone.cs
├── Services/
│   └── AssetManager.cs        # List<Asset>, AddAsset, ShowAllAssets
└── Helpers/
    ├── ConsoleUiHelper.cs     # Colorized titles and messages
    └── InputHelper.cs         # Validation + exit anywhere
```

---

## How the program flows

1. Welcome banner (colorized).
2. Choose **computer** or **mobile phone**.
3. Enter the fields from the assignment (USD and local price are both asked — like *1500 USD* and *15000 SEK* in the PDF example).
4. Choose **laptop vs desktop** for computers.
5. Answer **y/n** to add another asset.
6. When finished, the app prints **every asset** in a neat list.

You can type **`exit`** (any capitalization, with spaces), **`quit`**, or **`q`** at any prompt to stop safely.

---

## Example session

```
Add a new asset
  1 - Computer (laptop or desktop)
  2 - Mobile phone

Your choice (1-2, or exit): 1
Brand: Apple
Model: MacBook Pro
Purchase date: 2024-03-15
Price (USD): 1500
Price (local currency, e.g. SEK): 15000
Office (e.g. Sweden): Sweden
Computer form factor: 1 = Laptop, 2 = Desktop
Your choice (1-2, or exit): 1
Asset added to the list.

Add another asset now? (y/n): n

Company asset list
------------------------------------------------------------------------
  1. Laptop | Apple MacBook Pro | Office: Sweden | Purchased: 2024-03-15 | USD 1,500.00 | Local 15,000.00
------------------------------------------------------------------------
Total assets: 1
```

---

## Key C# ideas (for learning)

### Inheritance

`Computer` and `MobilePhone` **inherit** from `Asset`. The list is declared as `List<Asset>` but can hold both types — that is polymorphism starting already in Level 1.

### Abstract `AssetType`

Each subclass returns its own type string:

- `Computer` → `"Laptop"` or `"Desktop"`
- `MobilePhone` → `"Mobile Phone"`

### Separation of concerns

- **Models** — data and display line  
- **AssetManager** — the list and printing  
- **Helpers** — input and colors  
- **Program.cs** — user conversation only  

---

## Git — push Level 1 first

From the repository root:

```bash
git add Week45AssetTracking/
git commit -m "Week45 Level 1: Asset, Computer, MobilePhone, List<Asset>"
git push
```

When Level 1 is on GitHub, continue with Level 2 in the next commit.

---

## Roadmap (not in this commit yet)

| Level | Coming next |
|-------|-------------|
| **2** | Sort by type and date, end-of-life yellow/red highlights |
| **3** | Offices SEK/USD/TRY, currency conversion |
| **4** | IDs, age, full menu, stronger OOP |
| **5** | JSON save/load, export report |

---

## Troubleshooting

| Problem | What to try |
|--------|-------------|
| `'dotnet' is not recognized` | Install .NET 10 SDK and restart the terminal. |
| Date not accepted | Use `yyyy-MM-dd`, e.g. `2024-03-15`. |
| No colors | Use Windows Terminal or a modern console host. |

---

**Author:** Lotfar Kaes — Asset Tracking, Level 1.