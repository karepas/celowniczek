using System;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace Celowniczek
{
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
                    if (data != null) return data;
                }
            }
            catch { }

            return new ConfigData();
        }

        public static void SaveConfig(ConfigData data)
        {
            try
            {
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
            form.CrosshairColor = ColorTranslator.FromHtml(data.ColorHex);
            form.OutlineColor = ColorTranslator.FromHtml(data.OutlineColorHex);
            form.EnableOutline = data.EnableOutline;
            form.SizePx = data.Size;
            form.Thickness = data.Thickness;
            form.Gap = data.Gap;
            form.DotRadius = data.DotRadius;
            form.Style = data.Style;

            form.UpdateBoundsToCenter();
            form.Redraw();
        }
    }
}