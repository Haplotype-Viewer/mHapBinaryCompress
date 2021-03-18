using System;
using System.Collections;
using System.IO;
using System.IO.Compression;

namespace BinaryZip
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input a file name (.mhap) eg:SRX759486.mhap :");

            var fileName = Console.ReadLine();
            var lines = File.ReadAllLines(fileName);

            var fileStream = new FileStream("tmp.bin", FileMode.Create);
            var streamWriter = new BinaryWriter(fileStream);

            var progess = 0;

            foreach (var line in lines)
            {
                var mHaps = line.Split("\t");

                var chr = "";

                if (mHaps[0].Contains("X") || mHaps[0].Contains("Y") || mHaps[0].Contains("M"))
                {
                    switch (mHaps[0].Replace("chr", ""))
                    {
                        case "X":
                            chr = "23";
                            break;
                        case "Y":
                            chr = "24";
                            break;
                        case "M":
                            chr = "25";
                            break;
                    }

                    chr = Convert.ToString(int.Parse(chr), 2);
                }
                else
                {
                    chr = Convert.ToString(int.Parse(mHaps[0].Replace("chr", "")), 2);
                }

                var begin = Convert.ToString(int.Parse(mHaps[1]), 2);
                var end = Convert.ToString(int.Parse(mHaps[2]), 2);
                var read = mHaps[3];
                var count = Convert.ToString(int.Parse(mHaps[4]), 2);
                var strand = mHaps[5];

                if (strand == "-")
                {
                    strand = "10";
                }
                else
                {
                    strand = strand == "+" ? "1" : "0";
                }

                var binary = chr + begin + end + read + count + strand;

                foreach (var bit in binary)
                {
                    if (bit == '0')
                    {
                        streamWriter.Write(false);
                    }
                    else
                    {
                        streamWriter.Write(true);
                    }
                }

                progess++;

                if (progess % 200 == 0)
                {
                    Console.WriteLine($"progess: {progess} / {lines.Length}");
                }
            }

            fileStream.Close();

            Console.WriteLine($"Compress file...Wait...");

            File.WriteAllBytes("compress.gz", Compress(File.ReadAllBytes("tmp.bin")));
        }


        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close();
                    return outStream.ToArray();
                }
            }
        }
    }
}