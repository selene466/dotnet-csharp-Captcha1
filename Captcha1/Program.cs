using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

namespace SixLabors.Shapes.DrawShapesWithImageSharp;

class Program
{
    static void Main()
    {
        // main program
        string captchaText = GenerateRandomCaptcha(4);
        Console.WriteLine($"Generated Captcha: {captchaText}");
        GenerateCaptchaImage(captchaText, "captcha.png");
        Console.WriteLine("Captcha image generated successfully!");
    }

    static string GenerateRandomCaptcha(int length) =>
        new Random().Next(1000, 10000).ToString().Substring(0, length);

    static void GenerateCaptchaImage(string captchaText, string outputPath)
    {
        Random random = new Random();

        // initiate img
        int width = 150;
        int height = 60;
        using var img = new Image<Rgba32>(width, height);
        using var imgBase = new Image<Rgba32>(width, height);
        using var imgDot = new Image<Rgba32>(width, height);
        img.Mutate(i => i.Fill(Color.White));
        imgBase.Mutate(i => i.Fill(Color.White));

        // initiate captcha
        var stringText = GenerateRandomCaptcha(4).ToString();
        var fontFamilies = new string[] { "Arial", "Verdana", "Times New Roman", "Courier New" };
        Color[] colorsList = { Color.Navy, Color.Green, Color.Orange, Color.Black };
        var x = 0;
        var y = 0;
        var position = 0.0f;

        // random font family and font style
        FontStyle fontStyle = random.Next(0, 2) == 0 ? FontStyle.Regular : FontStyle.Bold;
        var font = SystemFonts.CreateFont(fontFamilies[random.Next(0, fontFamilies.Length)], 48, fontStyle);

        // random color for font and for dot
        var color = random.Next(colorsList.Length);

        // iterate from generated random numeric
        foreach (char c in stringText)
        {
            var location = new PointF(x + position, y);

            // draw text from configured location -> rotate
            float rotateChar = (float)(random.NextDouble() * (13 - 1) - 6);

            img.Mutate(ctx => ctx.Rotate(rotateChar));
            img.Mutate(ctx => ctx.DrawText(c.ToString(), font, colorsList[color], location));

            // distant between char
            position = position + TextMeasurer.MeasureAdvance(c.ToString(), new TextOptions(font)).Width - random.Next(2, 5);

            // prevent last char go up
            y += 4;
        }

        // random dot
        for (int i = 0; i < 10; i++)
        {
            // draw dot & dot size
            var solidBrush = new SolidBrush(colorsList[color]);
            var positionDot = new PointF(random.Next(imgDot.Width), random.Next(imgDot.Height));
            imgDot.Mutate(x => x.Fill(solidBrush, new EllipsePolygon(positionDot, random.Next(4, 8))));
        }

        // skew img
        float skewX = (float)(random.NextDouble() * (41 - 1) - 20);
        float skewY = (float)(random.NextDouble() * (5 - 1) + 1);
        img.Mutate(ctx => ctx.Skew(skewX, skewY));

        // draw img to imgBase
        int centerX = ((imgBase.Width - img.Width) / 2) + 12;
        imgBase.Mutate(ctx => ctx.DrawImage(img, new Point(centerX, -12), 1));

        // draw imgDot to imgBase
        imgBase.Mutate(ctx => ctx.DrawImage(imgDot, new Point(0, 0), 1));

        // write output file
        using var stream = File.OpenWrite(outputPath);
        imgBase.SaveAsPng(stream);
    }
}
