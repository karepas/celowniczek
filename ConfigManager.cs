using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Celowniczek
{
    public class CrosshairPositionProfile
    {
        public string Name { get; set; } = "Default";
        public double X { get; set; } = 0.5;
        public double Y { get; set; } = 0.5;
    }

    public class ConfigData
    {
        public string ColorHex { get; set; } = "#00FFFF";
        public string OutlineColorHex { get; set; } = "#000000";
        public bool EnableOutline { get; set; } = true;
        public int Size { get; set; } = 12;
        public int Thickness { get; set; } = 2;
        public int Gap { get; set; } = 4;
        public int DotRadius { get; set; } = 2;
        public CrosshairStyle Style { get; set; } = CrosshairStyle.Cross;
        public string Language { get; set; } = "en"; // Domyślnie angielski
        public string Theme { get; set; } = UiThemeManager.DefaultThemeId;
        public string SelectedPositionProfile { get; set; } = "Default";
        public List<CrosshairPositionProfile> PositionProfiles { get; set; } = new()
        {
            new CrosshairPositionProfile()
        };
    }

    public static class ConfigManager
    {
        private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Celowniczek");
        private static readonly string FilePath = Path.Combine(FolderPath, "config.json");

        public static ConfigData LoadConfig()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    ConfigData? data = JsonSerializer.Deserialize<ConfigData>(json);
                    if (data != null)
                    {
                        NormalizeColors(data);
                        NormalizeProfiles(data);
                        return data;
                    }
                }
            }
            catch { }

            return new ConfigData();
        }

        public static void SaveConfig(ConfigData data)
        {
            try
            {
                NormalizeColors(data);
                NormalizeProfiles(data);
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch { }
        }

        public static void ApplyToForm(ConfigData data, CrosshairForm form)
        {
            NormalizeColors(data);
            CrosshairPositionProfile profile = GetSelectedPositionProfile(data);
            form.CrosshairColor = ColorTranslator.FromHtml(data.ColorHex);
            form.OutlineColor = ColorTranslator.FromHtml(data.OutlineColorHex);
            form.EnableOutline = data.EnableOutline;
            form.SizePx = data.Size;
            form.Thickness = data.Thickness;
            form.Gap = data.Gap;
            form.DotRadius = data.DotRadius;
            form.Style = data.Style;
            form.PositionX = profile.X;
            form.PositionY = profile.Y;

            form.UpdateBoundsToCenter();
            form.Redraw();
        }

        public static CrosshairPositionProfile GetSelectedPositionProfile(ConfigData data)
        {
            NormalizeProfiles(data);
            return data.PositionProfiles.First(profile =>
                string.Equals(profile.Name, data.SelectedPositionProfile, StringComparison.OrdinalIgnoreCase));
        }

        public static void NormalizeProfiles(ConfigData data)
        {
            data.PositionProfiles ??= new List<CrosshairPositionProfile>();
            data.PositionProfiles = data.PositionProfiles
                .Where(profile => profile != null && !string.IsNullOrWhiteSpace(profile.Name))
                .ToList();

            if (data.PositionProfiles.Count == 0)
            {
                data.PositionProfiles.Add(new CrosshairPositionProfile());
            }

            foreach (CrosshairPositionProfile profile in data.PositionProfiles)
            {
                profile.Name = profile.Name.Trim();
                profile.X = double.IsFinite(profile.X) ? Math.Clamp(profile.X, 0, 1) : 0.5;
                profile.Y = double.IsFinite(profile.Y) ? Math.Clamp(profile.Y, 0, 1) : 0.5;
            }

            if (!data.PositionProfiles.Any(profile =>
                string.Equals(profile.Name, data.SelectedPositionProfile, StringComparison.OrdinalIgnoreCase)))
            {
                data.SelectedPositionProfile = data.PositionProfiles[0].Name;
            }
        }

        private static void NormalizeColors(ConfigData data)
        {
            data.ColorHex = NormalizeColorHex(data.ColorHex, "#00FFFF");
            data.OutlineColorHex = NormalizeColorHex(data.OutlineColorHex, "#000000");
        }

        private static string NormalizeColorHex(string? value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            try
            {
                Color color = ColorTranslator.FromHtml(value);
                if (color.A != 255)
                {
                    return fallback;
                }

                return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            catch (ArgumentException)
            {
                return fallback;
            }
        }
    }
}