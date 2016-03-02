using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JPEG.ExtensionsMethods;

namespace JPEG.CompressionAlgorithms
{
    internal class HuffmanNode
    {
        public byte? LeafLabel { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }
    }

    [Serializable]
    internal class BitsWithLength
    {
        public int Bits { get; set; }
        public int BitsCount { get; set; }

        [Serializable]
        public class Comparer : IEqualityComparer<BitsWithLength>
        {
            public bool Equals(BitsWithLength x, BitsWithLength y)
            {
                if (x == y) return true;
                if (x == null || y == null)
                    return false;
                return x.BitsCount == y.BitsCount && x.Bits == y.Bits;
            }

            public int GetHashCode(BitsWithLength obj)
            {
                if (obj == null)
                    return 0;
                return obj.Bits ^ obj.BitsCount;
            }
        }
    }

    internal class BitsBuffer
    {
        private readonly List<byte> _buffer = new List<byte>();
        private readonly BitsWithLength _unfinishedBits = new BitsWithLength();

        public void Add(BitsWithLength bitsWithLength)
        {
            var bitsCount = bitsWithLength.BitsCount;
            var bits = bitsWithLength.Bits;

            //берет столько битов, чтобы дополнить текущий байт
            var neededBits = 8 - _unfinishedBits.BitsCount;
            while (bitsCount >= neededBits)
            {
                bitsCount -= neededBits;
                //берет needBits битов, дополняет байт, записывает в буффер
                _buffer.Add((byte)((_unfinishedBits.Bits << neededBits) + (bits >> bitsCount)));

                //оставшиеся биты
                bits = bits & ((1 << bitsCount) - 1);

                _unfinishedBits.Bits = 0;
                _unfinishedBits.BitsCount = 0;

                neededBits = 8;
            }
            //записывает оставшиеся биты в неполный байт
            _unfinishedBits.BitsCount += bitsCount;
            _unfinishedBits.Bits = (_unfinishedBits.Bits << bitsCount) + bits;
        }

        public byte[] ToArray(out long bitsCount)
        {
            bitsCount = _buffer.Count * 8L + _unfinishedBits.BitsCount;
            var result = new byte[bitsCount / 8 + (bitsCount % 8 > 0 ? 1 : 0)];
            _buffer.CopyTo(result);
            if (_unfinishedBits.BitsCount > 0)
                result[_buffer.Count] = (byte)(_unfinishedBits.Bits << (8 - _unfinishedBits.BitsCount));
            return result;
        }
    }

    internal class HuffmanCodec
    {
        private static int _numberTasks;
        private static ManualResetEvent _completeCreateEncodeTalbe;

        public static byte[] Encode(byte[] data, out Dictionary<BitsWithLength, byte> decodeTable, out long bitsCount)
        {
            var frequences = CalcFrequencesTable(data);

            var root = BuildHuffmanTree(frequences);

            var encodeTable = new BitsWithLength[byte.MaxValue + 1];

            _numberTasks = 0;
            _completeCreateEncodeTalbe = new ManualResetEvent(false);

            FillEncodeTable(root, encodeTable);

            _completeCreateEncodeTalbe.WaitOne();

            var bitsBuffer = new BitsBuffer();
            foreach (var b in data)
                bitsBuffer.Add(encodeTable[b]);

            decodeTable = CreateDecodeDict(encodeTable);

            return bitsBuffer.ToArray(out bitsCount);
        }

        public static byte[] Decode(byte[] encodedData, Dictionary<BitsWithLength, byte> decodeTable, long bitsCount)
        {
            var result = new List<byte>();

            var sample = new BitsWithLength { Bits = 0, BitsCount = 0 };
            for (var byteNum = 0; byteNum < encodedData.Length; byteNum++)
            {
                var encodedByte = encodedData[byteNum];
                for (var bitNum = 0; bitNum < 8 && byteNum * 8 + bitNum < bitsCount; bitNum++)
                {
                    //сдвигается предыдущее значение, считывается бит под номером bitNum из encodedByte и добавляется => собирается последовательность бит
                    sample.Bits = (sample.Bits << 1) + ((encodedByte & (1 << (8 - bitNum - 1))) != 0 ? 1 : 0);
                    sample.BitsCount++;

                    byte decodedByte;
                    if (!decodeTable.TryGetValue(sample, out decodedByte)) continue;
                    result.Add(decodedByte);

                    sample.BitsCount = 0;
                    sample.Bits = 0;
                }
            }
            return result.ToArray();
        }

        private static Dictionary<BitsWithLength, byte> CreateDecodeDict(IReadOnlyList<BitsWithLength> encodeTable)
        {
            var result = new Dictionary<BitsWithLength, byte>(new BitsWithLength.Comparer());
            for (var b = 0; b < encodeTable.Count; b++)
            {
                var bitsWithLength = encodeTable[b];
                if (bitsWithLength == null)
                    continue;

                result[bitsWithLength] = (byte)b;
            }
            return result;
        }

        private static void FillEncodeTable(HuffmanNode node, IList<BitsWithLength> encodeSubstitutionTable, int bitsIdentificator = 0, int depth = 0)
        {
            if (node.LeafLabel != null)
                encodeSubstitutionTable[node.LeafLabel.Value] = new BitsWithLength {Bits = bitsIdentificator, BitsCount = depth};
            else if(node.Left != null)
            {
                Interlocked.Increment(ref _numberTasks);
                ThreadPool.QueueUserWorkItem(state =>
                    FillEncodeTable(node.Left, encodeSubstitutionTable, (bitsIdentificator << 1) + 1, depth + 1));
                Interlocked.Increment(ref _numberTasks);
                ThreadPool.QueueUserWorkItem(state =>
                    FillEncodeTable(node.Right, encodeSubstitutionTable, (bitsIdentificator << 1) + 0, depth + 1));
            }
            if (Interlocked.Decrement(ref _numberTasks) == 0)
                _completeCreateEncodeTalbe.Set();
        }

        private static HuffmanNode BuildHuffmanTree(IReadOnlyList<int> frequences)
        {
            var nodes = new HashSet<HuffmanNode>(Enumerable
                .Range(0, byte.MaxValue + 1)
                .Where(num => frequences[num] > 0)
                .Select(num => new HuffmanNode { Frequency = frequences[num], LeafLabel = (byte)num }));
            while (nodes.Count > 1)
            {
                var firstMin = nodes.MinOrDefault(node => node.Frequency);
                nodes.Remove(firstMin);
                var secondMin = nodes.MinOrDefault(node => node.Frequency);
                nodes.Remove(secondMin);
                nodes.Add(new HuffmanNode { Frequency = firstMin.Frequency + secondMin.Frequency, Left = secondMin, Right = firstMin });
            }
            return nodes.First();
        }

        private static int[] CalcFrequencesTable(IEnumerable<byte> data)
        {
            var result = new int[byte.MaxValue + 1];
            //foreach (var b in data)
            //    result[b]++;
            Parallel.ForEach(data, b => result[b]++);
            return result;
        }
    }
}