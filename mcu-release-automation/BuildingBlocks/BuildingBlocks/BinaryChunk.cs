using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiscUtil.Conversion;

namespace Automation.Base.BuildingBlocks
{
    /// <summary>
    /// Convert usage : bigEndianBitConverter.ToInt64(bytes, offset);
    /// </summary>
    [Serializable]
    public class BinaryChunk
    {
        private byte[] _data;
        private Align _alignment = Align.byte_1;
        private Sign _sign = Sign.UnSigned;
        private LittleEndianBitConverter _littleEndianConverter = new LittleEndianBitConverter();
        private BigEndianBitConverter _bigEndianConverter = new BigEndianBitConverter();
        private DoubleConverter _doubleConverter = new DoubleConverter();
        public BinaryChunk(byte[] data)
        {
            _data = data;
        }
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (value.Length % (int)_alignment != 0)
                {
                    throw new ArgumentException(string.Format(@"-- The input data is not align to {0}", _alignment.ToString()));
                }
                _data = value;
            }
        }
        public string SystemEndian
        {
            get { return BitConverter.IsLittleEndian ? "Little" : "Big"; }
        }
        public int Alignment
        {
            get { return (int)_alignment; }
            set
            {
                _alignment = (Align)Enum.Parse(typeof(Align), value.ToString());
            }
        }
        public string Signed
        {
            get { return _sign.ToString(); }
            set
            {
                _sign = (Sign)Enum.Parse(typeof(Sign), value);
            }
        }
        public override string ToString()
        {
            var group = _group;
            if (null == group)
            {
                return base.ToString();
            }
            var x = new List<string>();
            foreach (var g in group)
            {
                foreach (var val in g)
                {
                    x.Add(Extensions.ByteToHex(val));
                }
            }
            return string.Join(" ", x.ToArray()).TrimEnd();
        }
        private dynamic _group
        {
            get
            {
                try
                {
                    return _data.Select((value, index) => new { value, index })
                            .GroupBy(x => x.index / (int)_alignment, x => x.value).ToArray();
                }
                catch {}
                return null;
            }
        }
    }
}
