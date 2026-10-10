# 🎯 Celowniczek
---
## 🇬🇧 English

A lightweight, open-source, and precise crosshair overlay for Windows, designed for gamers (e.g., CS2, Rust, GTA V).

### ✨ Features
- **100% Click-Through:** The overlay is completely transparent to the mouse (`WS_EX_TRANSPARENT`), preventing focus loss in-game (no Alt+Tab issues while shooting).
- **Precise Positioning:** The crosshair is rendered perfectly in the mathematical center of the screen.
- **Full Customization:**
  - Color adjustment (RGB/HEX palette)
  - Black outline to increase visibility
  - Adjustment of size, line thickness, and gap
  - Optional center dot
- **Settings Persistence:** All preferences are automatically saved in `%APPDATA%\Celowniczek\config.json`.
- **System Tray:** The app runs quietly in the system tray next to the clock.
- **Auto-Updates:** Built-in update checker for new versions on GitHub.

### 🚀 Download & Installation
1. Go to the **Releases** tab.
2. Download the latest installer `CelowniczekInstaller.msi`.
3. Run the file and go through the simple installation process.
4. When running the game, make sure your graphics mode is set to **Borderless / Fullscreen Windowed**.
5. In case of alignment issues, open settings via the tray icon and adjust parameters like gap.

### 🛠️ Requirements & Compilation
If you want to compile the project yourself from sources:
- **System:** Windows 10 / 11 (64-bit)
- **SDK:** .NET 8.0 SDK
- **WiX Toolset v4/v5** (to generate the MSI package)

Compilation commands:
```powershell
$env:WIX_ACCEPT_EULA="1"
wix build Installer.wxs -o CelowniczekInstaller.msi
```

🇵🇱 Polski

Lekka, otwartoźródłowa i precyzyjna nakładka celownika (overlay) dla systemu Windows, stworzona z myślą o graczach (np. CS2, Rust, GTA V).

✨ Funkcje
100% Click-Through: Nakładka jest całkowicie przenikalna dla myszy (WS_EX_TRANSPARENT), co zapobiega gubieniu ostrości w grze (brak problemów z Alt+Tab podczas strzelania).

Precyzyjne pozycjonowanie: Celownik rysowany jest idealnie w matematycznym środku ekranu.

Pełna personalizacja:

Regulacja koloru (paleta RGB/HEX)

Czarny obrys (Outline) zwiększający widoczność

Regulacja rozmiaru, grubości linii oraz przerwy (Gap)

Opcjonalna kropka w samym środku

Zapis ustawień: Wszystkie preferencje zapisują się automatycznie w %APPDATA%\Celowniczek\config.json.

System Tray: Aplikacja działa w tle w zasobniku systemowym obok zegarka.

Automatyczne aktualizacje: Wbudowany moduł sprawdzający nowe wersje na GitHubie.

🚀 Pobieranie i Instalacja
Przejdź do zakładki Releases.

Pobierz najnowszą wersję instalatora CelowniczekInstaller.msi.

Uruchom plik i przejdź przez prosty proces instalacji.

Po uruchomieniu gry upewnij się, że masz ustawiony tryb graficzny Pełny ekran w oknie (Borderless / Fullscreen Windowed).

W razie problemów z miejscem/pozycjonowaniem wejdź w ikonkę celownika w zasobniku systemowym, otwórz ustawienia celownika i zmień np. odległość (Gap).

🛠️ Wymagania i Kompilacja
Jeśli chcesz samodzielnie skompilować projekt ze źródeł:

System: Windows 10 / 11 (64-bit)

SDK: .NET 8.0 SDK

WiX Toolset v4/v5 (do generowania paczki MSI)

Polecenia kompilacji:
```powershell
$env:WIX_ACCEPT_EULA="1"
wix build Installer.wxs -o CelowniczekInstaller.msi
```
