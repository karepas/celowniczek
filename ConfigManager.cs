using System;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace Celowniczek
{
    public class ConfigData
    {
        public string Style { get; set; } = "CrossAndDot";
        public string CrosshairColor { get; set; } = "#00FFFF";
        public string OutlineColor { get; set; } = "#000000";
        public bool EnableOutline { get; set; } = true;
        public int SizePx { get; set; } = 12;
        public int Thickness { get; set; } = 2;
        public int Gap { get; set; } = 4;
        public int DotRadius { get; set; } = 2;
    }

    public static class ConfigManager
    {
        private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Celowniczek");
        private static readonly string ConfigPath = Path.Combine(FolderPath, "config.json");

        public static void LoadConfig(CrosshairForm form)
        {
            try
            {
                if (!File.Exists(ConfigPath))
                    return;

                string json = File.ReadAllText(ConfigPath);
                ConfigData? data = JsonSerializer.Deserialize<ConfigData>(json);

                if (data != null)
                {
                    if (Enum.TryParse(data.Style, out CrosshairForm.CrosshairStyle parsedStyle))
                        form.Style = parsedStyle;

                    form.CrosshairColor = ColorTranslator.FromHtml(data.CrosshairColor);
                    form.OutlineColor = ColorTranslator.FromHtml(data.OutlineColor);
                    form.EnableOutline = data.EnableOutline;
                    form.SizePx = data.SizePx;
                    form.Thickness = data.Thickness;
                    form.Gap = data.Gap;
                    form.DotRadius = data.DotRadius;

                    form.UpdateBoundsToCenter();
                }
            }
            catch { }
        }

        public static void SaveConfig(CrosshairForm form)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                ConfigData data = new ConfigData
                {
                    Style = form.Style.ToString(),
                    CrosshairColor = ColorTranslator.ToHtml(form.CrosshairColor),
                    OutlineColor = ColorTranslator.ToHtml(form.OutlineColor),
                    EnableOutline = form.EnableOutline,
                    SizePx = form.SizePx,
                    Thickness = form.Thickness,
                    Gap = form.Gap,
                    DotRadius = form.DotRadius
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }
    }
}