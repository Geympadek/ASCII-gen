#pragma warning disable CA1416

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_gen
{
    internal class AsciiImg
    {
        public class AsciiChar(char Char = '\0', ConsoleColor Color = ConsoleColor.White)
        {
            public char Char = Char;
            public ConsoleColor Color = Color;
        }
        public AsciiChar[,] content { get; }
        public int width { get; }
        public int height { get; }
        
        public AsciiImg(AsciiChar[,] content)
        {
            this.content = content;
            this.width = content.GetLength(0);
            this.height = content.GetLength(1);
        }
        private static Color FindColor(Color[,] pixels, double charWidth, double charHeight, int x, int y)
        {
            int r = 0, g = 0, b = 0, a = 0;

            x = (int)(x * charWidth);
            y = (int)(y * charHeight);

            for (int i = 0; i < charWidth; i++)
            {
                for (int j = 0; j < charHeight; j++)
                {
                    var pixel = pixels[i + x, j + y];
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    a += pixel.A;
                }
            }
            var charPixels = Math.Ceiling(charWidth) * Math.Ceiling(charHeight);
            r = (int)(r / charPixels);
            g = (int)(g / charPixels);
            b = (int)(b / charPixels);
            a = (int)(a / charPixels);

            return Color.FromArgb(a, r, g, b);
        }
        static private double NormalizeScale(double scale, int imgWidth, int imgHeight, GenInfo info)
        {
            var widthChar = imgWidth / info.charWidth;
            var heightChar = imgHeight / info.charHeight;

            if (imgWidth >= imgHeight)
            {
                scale *= (double)info.normalWidth / widthChar;
            }
            else
            {
                scale *= (double)info.normalHeight / heightChar;
            }
            return scale;
        }
        static private Color[,] GenColoredImage(Color[,] pixels, GenInfo info, double scale)
        {
            int pixelWidth = pixels.GetLength(0);
            int pixelHeight = pixels.GetLength(1);

            scale = NormalizeScale(scale, pixelWidth, pixelHeight, info);

            double charWidth = info.charWidth / scale;
            double charHeight = info.charHeight / scale;

            int width = (int)(pixelWidth/ charWidth);
            int height = (int)(pixelHeight / charHeight);
            Color[,] image = new Color[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] = FindColor(pixels, charWidth, charHeight, x, y);
                }
            }
            return image;
        }
        private static char BrightChar(GenInfo info, double brightness)
        {
            brightness = Math.Clamp(brightness, 0.0, 1.0);
            int index = (int)Math.Round(brightness * (info.gradient.Length - 1));
            return info.gradient[index];
        }
        static public AsciiImg Gen(string imagePath, GenInfo info, double scale)
        {
            var image = new Bitmap(imagePath);

            Color[,] pixels = new Color[image.Width, image.Height];

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                    pixels[x, y] = image.GetPixel(x, y);

            return Gen(pixels, info, scale);
        }
        static private ConsoleColor GetConsoleColor(Color color)
        {
            string[] COLOR_NAMES = ["Blue", "Green", "Cyan", "Red", "Magenta", "Yellow"];
            
            string closestName = "White";
            if (color.GetSaturation() >= 0.15 && color.GetBrightness() >= 0.15)
            {
                int minDiff = 360;
                foreach (var name in COLOR_NAMES)
                {
                    var cmpColor = Color.FromName(name);
                    int diff = (int)Math.Abs(color.GetHue() - cmpColor.GetHue());
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        closestName = name;
                    }
                }
            }
            
            return (ConsoleColor)Enum.Parse(typeof(ConsoleColor), closestName);
        }
        static public AsciiImg Gen(Color[,] pixels, GenInfo info, double scale)
        {
            Color[,] coloredImage = GenColoredImage(pixels, info, scale);
            int width = coloredImage.GetLength(0);
            int height = coloredImage.GetLength(1);
            AsciiChar[,] image = new AsciiChar[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = coloredImage[x, y];
                    double brightness = color.GetBrightness() * color.A / 255.0;
                    image[x, y] = new AsciiChar(Char: BrightChar(info, brightness), Color: GetConsoleColor(color));
                }
            }
            return new(image);
        }
        public static void Print(AsciiImg img, bool enableColors = false)
        {
            for (int y = 0; y < img.height; y++)
            {
                for (int x = 0; x < img.width; x++)
                {
                    var c = img.content[x, y];
                    if (enableColors)
                    {
                        Console.ForegroundColor = c.Color;
                    }
                    Console.Write(c.Char);
                }
                Console.WriteLine();
            }
        }
    }
}
