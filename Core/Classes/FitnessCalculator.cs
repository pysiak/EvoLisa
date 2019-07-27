using System;
using System.Drawing;
using System.Drawing.Imaging;
using GenArt.AST;

namespace GenArt.Classes
{
    public class FitnessCalculator : IDisposable
    {
        private readonly Bitmap b;
        private readonly Graphics g;

        public FitnessCalculator()
        {
            b = new Bitmap(Tools.MaxWidth, Tools.MaxHeight);
            g = Graphics.FromImage(b);
        }

        public void Dispose()
        {
            g.Dispose();
            b.Dispose();
        }

        public double GetDrawingFitness(DnaDrawing newDrawing, Color[,] sourceColors)
        {
            double error = 0;

            Renderer.Render(newDrawing, g, 1);

            BitmapData bmd1 = b.LockBits(new Rectangle(0, 0, Tools.MaxWidth, Tools.MaxHeight), ImageLockMode.ReadOnly,
                                         PixelFormat.Format24bppRgb);

            unchecked
            {
                unsafe
                {
                    for (int y = 0; y < Tools.MaxHeight; y++)
                    {
                        for (int x = 0; x < Tools.MaxWidth; x++)
                        {
                            byte* p = (byte*) bmd1.Scan0 + y * bmd1.Stride + 3 * x;
                            Color c1 = Color.FromArgb(p[2], p[1], p[0]);
                            Color c2 = sourceColors[x, y];

                            int r = c1.R - c2.R;
                            int g = c1.G - c2.G;
                            int b = c1.B - c2.B;

                            error += r * r + g * g + b * b;
                        }
                    }
                }
            }

            b.UnlockBits(bmd1);

            return error;
        }
    }
}