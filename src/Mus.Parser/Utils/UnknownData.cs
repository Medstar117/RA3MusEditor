using System;
using System.Linq;
using System.Xml.Serialization;

namespace Mus.Parser.Utils
{
    public sealed class UnknownData
    {
        [XmlIgnore]
        public byte[] Data;

        [XmlElement("Hex")]
        public string Hex {
            get
            {
                if (Data?.Length > 0)
                {
                    return "0x" + BitConverter.ToString(Data).Replace("-", " 0x");
                }
                return String.Empty;
            }
            set 
            {
                Data = 
                    value
                        .Split(new[] { " ", "0x" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => Convert.ToByte(x, 16))
                        .ToArray();
            }
        }

        private UnknownData()
        {
            Data = new byte[0];
        }

        public UnknownData(byte[] bytes)
        {
            Data = bytes;
        }
    }
}