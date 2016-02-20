using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

namespace JPEG
{
	public class CompressedImage
	{
		public int Height { get; set; }
		public int Width { get; set; }

        public PixelFormat PixelFormat { get; set; }

		public int CompressionLevel { get; set; }

		public List<double> Frequences { get; set; }
	    public int ThinIndex { get; set; }

	    public void Save(string path)
		{
			using(var sw = new FileStream(path, FileMode.Create))
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

				foreach (var t in Frequences)
				{
				    var portion = BitConverter.GetBytes((short)t);
				    sw.Write(portion, 0, portion.Length);
				}
			}
		}

		public static CompressedImage Load(string path, int dctSize)
		{
			var result = new CompressedImage();
			using(var sr = new FileStream(path, FileMode.Open))
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

				var blocksLumia = result.Height * result.Width / (dctSize * dctSize);
			    var blockColor = blocksLumia/(result.ThinIndex*result.ThinIndex);
			    var countBlocks = (blocksLumia + blockColor*2)*result.CompressionLevel;

                result.Frequences = new List<double>();

				for(var blockNum = 0; blockNum < countBlocks; blockNum++)
				{
                    sr.Read(buffer, 0, 2);
					result.Frequences.Add(BitConverter.ToInt16(buffer, 0));
				}
			}
			return result;
		}
	}
}