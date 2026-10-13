using System;
using System.Drawing;
using System.Linq;

namespace Celowniczek
{
    public class CommunityCrosshair
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int Size { get; set; } = 12;
        public int Thickness { get; set; } = 2;
        public int Gap { get; set; } = 4;
        public int DotRadius { get; set; } = 2;
        public string ColorHex { get; set; } = "#00FFFF";
        public bool EnableOutline { get; set; } = true;
        public CrosshairStyle Style { get; set; } = CrosshairStyle.Cross;

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && Title.Trim().Length <= 64
                && !string.IsNullOrWhiteSpace(Author)
                && Author.Trim().Length <= 40
                && Size >= 2
                && Size <= 50
                && Thickness >= 1
                && Thickness <= 10
                && Gap >= 0
                && Gap <= 30
                && DotRadius >= 0
                && DotRadius <= 10
                && IsValidHexColor(ColorHex)
                && Enum.IsDefined(Style);
        }

        public override string ToString()
        {
            return $"{Title.Trim()} — {Author.Trim()}";
        }

        private static bool IsValidHexColor(string? colorHex)
        {
            return colorHex != null
                && colorHex.Length == 7
                && colorHex[0] == '#'
                && colorHex.Skip(1).All(Uri.IsHexDigit)
                && ColorTranslator.FromHtml(colorHex).A == 255;
        }
    }
}
