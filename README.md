# 🎯 Celowniczek

Lekka, otwartoźródłowa i precyzyjna nakładka celownika (overlay) dla systemu Windows, stworzona z myślą o graczach (np. CS2, Rust, GTA V).

![Licencja](https://img.shields.io/github/license/karepas/celowniczek)
![Wersja](https://img.shields.io/github/v/release/karepas/celowniczek)
![Pobrania](https://img.shields.io/github/downloads/karepas/celowniczek/total)

---

## ✨ Funkcje

- **100% Click-Through:** Nakładka jest całkowicie przenikalna dla myszy (`WS_EX_TRANSPARENT`), co zapobiega gubieniu ostrości w grze (brak problemów z Alt+Tab podczas strzelania).
- **Precyzyjne pozycjonowanie:** Celownik rysowany jest idealnie w matematycznym środku ekranu.
- **Pełna personalizacja:**
  - Regulacja koloru (paleta RGB/HEX)
  - Czarny obrys (Outline) zwiększający widoczność
  - Regulacja rozmiaru, grubości linii oraz przerwy (Gap)
  - Opcjonalna kropka w samym środku
- **Zapis ustawień:** Wszystkie preferencje zapisują się automatycznie w `%APPDATA%\Celowniczek\config.json`.
- **System Tray:** Aplikacja działa w tle w zasobniku systemowym obok zegarka.
- **Automatyczne aktualizacje:** Wbudowany moduł sprawdzający nowe wersje na GitHubie.

---

## 🚀 Pobieranie i Instalacja

1. Przejdź do zakładki **[Releases](https://github.com/karepas/celowniczek/releases)**.
2. Pobierz najnowszą wersję instalatora **`CelowniczekInstaller.msi`**.
3. Uruchom plik i przejdź przez prosty proces instalacji.
4. Po uruchomieniu gry upewnij się, że masz ustawiony tryb graficzny **Pełny ekran w oknie (Borderless / Fullscreen Windowed)**.
5. Wrazie problemów z miejscem wejdź w ikonkę celownika później ustawienia celownika i zmień np odległość 

---

## 🛠️ Wymagania i Kompilacja

Jeśli chcesz samodzielnie skompilować projekt ze źródeł:

- **System:** Windows 10 / 11 (64-bit)
- **SDK:** [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- **WiX Toolset v4/v5** (do generowania paczki MSI)

### Polecenia kompilacji:

```powershell
# Pubikacja aplikacji .NET 8
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

# Zbudowanie instalatora MSI
$env:WIX_ACCEPT_EULA="1"
wix build Installer.wxs -o CelowniczekInstaller.msi
