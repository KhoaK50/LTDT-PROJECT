using System.Drawing;

namespace Core.Utils
{
    /// <summary>
    /// Cung cấp màu cho các ca thi (màu sắc đỉnh).
    /// Viết đơn giản, tránh switch-expression để hợp C# 7.3.
    /// </summary>
    public static class ColorUtils
    {
        public static Color GetColorForIndex(int colorIndex)
        {
            if (colorIndex < 0)
                return Color.White;

            Color[] palette = new Color[]
            {
                Color.LightBlue,
                Color.LightCoral,
                Color.LightGreen,
                Color.Khaki,
                Color.Plum,
                Color.LightPink,
                Color.LightSalmon,
                Color.Moccasin,
                Color.PaleTurquoise,
                Color.Thistle,
                Color.Wheat,
                Color.PaleGreen
            };

            int idx = colorIndex % palette.Length;
            return palette[idx];
        }
    }
}
