using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JPEG.CompressionAlgorithms;

namespace JPEG
{
    public class CompressedImage
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public PixelFormat PixelFormat { get; set; }

        public int CompressionLevel { get; set; }

        public byte[] DataBytes { get; set; }
        public int ThinIndex { get; set; }

        public void Save(string path)
        {
            using (var sw = new FileStream(path, FileMode.Create))
            {
                var heightBytes = BitConverter.GetBytes(Height);
                sw.Write(heightBytes, 0, 4);

                var widthBytes = BitConverter.GetBytes(Width);
                sw.Write(widthBytes, 0, 4);

                var pixelFormatBytes = BitConverter.GetBytes((int)PixelFormat);
                sw.Write(pixelFormatBytes, 0, 4);

                var thinIndexBytes = BitConverter.GetBytes(ThinIndex);
                sw.Write(thinIndexBytes, 0, 4);

                var compressionLevelBytes = BitConverter.GetBytes(CompressionLevel);
                sw.Write(compressionLevelBytes, 0, 4);

                var numberDataBytes = BitConverter.GetBytes(DataBytes.Length);
                sw.Write(numberDataBytes, 0, 4);

                sw.Write(DataBytes, 0, DataBytes.Length);

                var bitsCount = BitConverter.GetBytes(BitsCount);
                sw.Write(bitsCount, 0, 4);

                var binFormatter = new BinaryFormatter();
                var mStream = new MemoryStream();
                binFormatter.Serialize(mStream, DecodeTable);
                var bytesDecodeTable = mStream.ToArray();

                var numberBytesDecodeTable = BitConverter.GetBytes(bytesDecodeTable.Length);
                sw.Write(numberBytesDecodeTable, 0, 4);
                sw.Write(bytesDecodeTable, 0, bytesDecodeTable.Length);
            }
        }

        public static CompressedImage Load(string path, int dctSize)
        {
            var result = new CompressedImage();
            using (var sr = new FileStream(path, FileMode.Open))
            {
                var buffer = new byte[4];

                sr.Read(buffer, 0, 4);
                result.Height = BitConverter.ToInt32(buffer, 0);

                sr.Read(buffer, 0, 4);
                result.Width = BitConverter.ToInt32(buffer, 0);

                sr.Read(buffer, 0, 4);
                result.PixelFormat = (PixelFormat)BitConverter.ToInt32(buffer, 0);

                sr.Read(buffer, 0, 4);
                result.ThinIndex = BitConverter.ToInt32(buffer, 0);

                sr.Read(buffer, 0, 4);
                result.CompressionLevel = BitConverter.ToInt32(buffer, 0);

                sr.Read(buffer, 0, 4);
                result.NumberDataBytes = BitConverter.ToInt32(buffer, 0);

                var byteList = new List<byte>();
                for (var blockNum = 0; blockNum < result.NumberDataBytes; blockNum++)
                {
                    sr.Read(buffer, 0, 1);
                    byteList.Add(buffer[0]);
                }
                result.DataBytes = byteList.ToArray();

                sr.Read(buffer, 0, 4);
                result.BitsCount = BitConverter.ToInt32(buffer, 0);

                sr.Read(buffer, 0, 4);
                var numberBytesDecodeTable = BitConverter.ToInt32(buffer, 0);

                var bigBuffer = new byte[numberBytesDecodeTable];
                sr.Read(bigBuffer, 0, numberBytesDecodeTable);

                var mStream = new MemoryStream();
                var binFormatter = new BinaryFormatter();

                mStream.Write(bigBuffer, 0, bigBuffer.Length);
                mStream.Position = 0;

                var myObject = binFormatter.Deserialize(mStream) as Dictionary<BitsWithLength, byte>;
                result.DecodeTable = myObject;
            }
            return result;
        }
        public int NumberDataBytes { get; set; }
        internal Dictionary<BitsWithLength, byte> DecodeTable { get; set; }
        public long BitsCount { get; set; }
    }
}