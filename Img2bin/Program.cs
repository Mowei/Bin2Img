using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Img2Bin
{
    class Program
    {
        static readonly string outPath = @"C:\temp\Save.bin";
        static readonly string outTempPath = @"C:\temp\Temp";
        static readonly int frameWidth = 1600;
        static readonly int frameHeight = 800;
        static readonly int latticeSize = 1;//min = 1
        static readonly int interval_width = 0;
        static readonly int interval_height = 16;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var fileSize = 0;
            int numBytesWrite = 0;

            using (FileStream fsSource = new FileStream(outPath, FileMode.Append, FileAccess.Write))
            {
                for (var i = 0; i < 50; i++)
                {
                    string srcPath = $@"C:\temp\Img{i}.bmp";
                    try
                    {
                        var data = Img2bin(srcPath);
                        var dataLength = data.Length;
                        var offset = 0;
                        if (i == 0)
                        {
                            var temp = new byte[4];
                            Buffer.BlockCopy(data, 0, temp, 0, 4);
                            //Array.Reverse(temp);
                            fileSize = BitConverter.ToInt32(temp, 0);
                            dataLength -= 4;
                            offset += 4;
                        }

                        if (numBytesWrite + dataLength > fileSize)
                        {
                            fsSource.Write(data, offset, fileSize - numBytesWrite);
                            numBytesWrite += fileSize - numBytesWrite;
                        }
                        else
                        {
                            fsSource.Write(data, offset, dataLength);
                            numBytesWrite += dataLength;
                        }
                    }

                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }

        }
        public static byte[] Img2bin(String srcPath)
        {
            int offsetX = 0;
            int offsetY = 0;
            var fileNumBytesToRead = 0;
            Bitmap bmp = new Bitmap(srcPath);
            //Bitmap getBmp = new Bitmap(frameWidth, frameHeight);
            byte[] byteArray = new byte[frameWidth * frameHeight * 3];
            for (var i = 0; i < frameHeight; i += (latticeSize + interval_width))
            {
                for (var j = 0; j < frameWidth; j += latticeSize)
                {
                    int colorR = 0;
                    int colorG = 0;
                    int colorB = 0;
                    for (var k = 0; k < latticeSize; k++)
                    {
                        for (var l = 0; l < latticeSize; l++)
                        {
                            var color = bmp.GetPixel(offsetX + j + l, offsetY + i + k);
                            //getBmp.SetPixel(j,i,color);
                            colorR += color.R;
                            colorG += color.G;
                            colorB += color.B;
                        }
                    }
                    byteArray[fileNumBytesToRead++] = (byte)(colorR / (latticeSize * latticeSize));
                    byteArray[fileNumBytesToRead++] = (byte)(colorG / (latticeSize * latticeSize));
                    byteArray[fileNumBytesToRead++] = (byte)(colorB / (latticeSize * latticeSize));
                }
                if ((i + 1) % interval_height == 0)
                {
                    i += interval_height;
                }
            }
            //getBmp.Save(srcPath + ".temp.png");
            byte[] resultBytes = new byte[fileNumBytesToRead];
            resultBytes = byteArray.Take(fileNumBytesToRead).ToArray();
            return resultBytes;
        }
    }
}