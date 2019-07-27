using System;
using System.Drawing;
using System.Drawing.Imaging;
using GenArt.AST;

namespace GenArt.Classes
{
   public struct Pixel
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }

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

        public double GetDrawingFitness(DnaDrawing newDrawing, Pixel[] sourcePixels)
        {
            double error = 0;

            Renderer.Render(newDrawing, g, 1);

            BitmapData bmd1 = b.LockBits(new Rectangle(0, 0, Tools.MaxWidth, Tools.MaxHeight), ImageLockMode.ReadOnly,
                                         PixelFormat.Format24bppRgb);

            unchecked
            {
                unsafe
                {
                    fixed (Pixel *psourcePixels = sourcePixels)
                    {
                        Pixel* pc = psourcePixels;
                        for (int y = 0; y < Tools.MaxHeight; y++)
                        {
                            byte* p = (byte*) bmd1.Scan0 + y * bmd1.Stride;
                            for(int x = 0; x < Tools.MaxWidth; x++, p+= 4, pc++)
                            {
                                int r = p[2] - pc->R;
                                int g = p[1] - pc->G;
                                int b = p[0] - pc->B;

                                error += r * r + g * g + b * b;
                            }
                        }
                    }
                }
            }

            b.UnlockBits(bmd1);

            return error;
        }
    }
}