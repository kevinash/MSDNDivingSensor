// Kevin Ashley, Microsoft, 2018
// SensorKit
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorKitClient
{
    public class VisualHelpers
    {
        public static readonly SKColor[] Colors =
        {

                        SKColor.Parse("#266489"),

                        SKColor.Parse("#68B9C0"),

                        SKColor.Parse("#90D585"),

                        SKColor.Parse("#F3C151"),

                        SKColor.Parse("#F37F64"),

                        SKColor.Parse("#424856"),

                        SKColor.Parse("#8F97A4"),

                        SKColor.Parse("#DAC096"),

                        SKColor.Parse("#76846E"),

                        SKColor.Parse("#DABFAF"),

                        SKColor.Parse("#A65B69"),

                        SKColor.Parse("#97A69D"),
                };

        public static SKColor NextColor(ref int ColorIndex)
        {
            var result = Colors[ColorIndex];
            ColorIndex = (ColorIndex + 1) % Colors.Length;
            return result;
        }
    }
}
