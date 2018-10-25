using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Bin2Img
{
    class Program
    {
        static readonly string savePath = @"C:\temp\";
        static readonly string srcPath = @"C:\temp\src.bin";
        static readonly int width = 1600;
        static readonly int height = 800;
        static readonly int latticeSize = 1;//min = 1
        static readonly int interval_width = 0;
        static readonly int interval_height = 16;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var fileByteArrary = FileToByteArrary(srcPath);
            if (fileByteArrary.Length == 0)
            {
                return;
            }
            ByteArrayToBitmap(fileByteArrary);

        }
        public static byte[] FileToByteArrary(String pathSource)
        {
            try
            {
                using (FileStream fsSource = new FileStream(pathSource, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[fsSource.Length + 4];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    Buffer.BlockCopy(BitConverter.GetBytes(numBytesToRead), 0, bytes, 0, 4);
                    numBytesRead = 4;
                    while (numBytesToRead > 0)
                    {
                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    numBytesToRead = bytes.Length;
                    return bytes;
                }
            }
            catch (FileNotFoundException ioEx)
            {
                Console.WriteLine(ioEx.Message);
                return new byte[0];
            }
        }
        public static void ByteArrayToBitmap(byte[] byteArray)
        {

            var fileNumBytesRead = byteArray.Length;
            var fileNumBytesToRead = 0;

            int part = 0;
            while (fileNumBytesToRead < fileNumBytesRead)
            {
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                for (var i = 0; i < height; i += (latticeSize + interval_width))
                {
                    for (var j = 0; j < width; j += latticeSize)
                    {
                        var r = 0;
                        if (fileNumBytesToRead < fileNumBytesRead)
                        {
                            r = byteArray[fileNumBytesToRead];
                            fileNumBytesToRead++;
                        }
                        var g = 0;
                        if (fileNumBytesToRead < fileNumBytesRead)
                        {
                            g = byteArray[fileNumBytesToRead];
                            fileNumBytesToRead++;
                        }
                        var b = 0;
                        if (fileNumBytesToRead < fileNumBytesRead)
                        {
                            b = byteArray[fileNumBytesToRead];
                            fileNumBytesToRead++;
                        }

                        for (var k = 0; k < latticeSize; k++)
                        {
                            for (var l = 0; l < latticeSize; l++)
                            {
                                bmp.SetPixel(j + l, i + k, Color.FromArgb(r, g, b));
                            }
                        }
                    }
                    if ((i + 1) % interval_height == 0)
                    {
                        i += interval_height;
                    }
                }
                Directory.CreateDirectory(savePath);
                bmp.Save($@"{savePath}Img{part}.bmp", ImageFormat.Png);
                part++;
                bmp.Dispose();
            }
        }
    }
}
