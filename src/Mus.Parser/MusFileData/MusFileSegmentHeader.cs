using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Mus.Parser.Utils;

namespace Mus.Parser.MusFileData
{
    public class MusFileSegmentHeader
    {
        public const int UnknownDataSize = 4;
        public string NameHelper => "Segment Header " + Index;

        public uint Magic; //some sort of magic number/hash for the music segment
        public uint Index;
        public uint ActualHeaderOffset
        {
            get => _rawHeaderOffset * 0x10;
            set
            {
                if (value % 0x10 != 0)
                {
                    throw new InvalidDataException($"{NameHelper}: ActualHeaderOffset ({value}) not multiplier of 0x10");
                }
                _rawHeaderOffset = value / 0x10;
            }
        }
        public uint ActualDataOffset
        {
            get => _rawDataOffset * 0x80;
            set
            {
                if (value % 0x80 != 0)
                {
                    throw new InvalidDataException($"{NameHelper}: ActualDataOffset ({value}) not multiplier of 0x80");
                }
                _rawDataOffset = value / 0x80;
            }
        }
        public int HeaderSize;
        public int DataSize;
        public UnknownData UnknownData;

        //this gets multiplied by 0x10 to find the offset within the .mus data for the header of this music segment
        [XmlIgnore]
        private uint _rawHeaderOffset;
        //this gets multiplied by 0x80 to find the offset within the .mus data for the compressed audio data of this music segment
        [XmlIgnore]
        private uint _rawDataOffset;

        public static MusFileSegmentHeader Parse(BinaryReader reader)
        {
            var header = new MusFileSegmentHeader();
            header.Magic = reader.ReadUInt32();
            header.Index = reader.ReadUInt32();
            header._rawHeaderOffset = reader.ReadUInt32();
            header._rawDataOffset = reader.ReadUInt32();
            header.HeaderSize = reader.ReadInt32();
            header.DataSize = reader.ReadInt32();
            header.UnknownData = new UnknownData(reader.ReadBytes(UnknownDataSize));
            return header;
        }

        public void Dump(BinaryWriter writer)
        {
            writer.Write(Magic);
            writer.Write(Index);
            writer.Write(_rawHeaderOffset);
            writer.Write(_rawDataOffset);
            writer.Write(HeaderSize);
            writer.Write(DataSize);
            writer.Write(UnknownData.Data);
        }

        public void Verify()
        {
            if(ActualHeaderOffset + HeaderSize > ActualDataOffset)
            {
                throw new InvalidDataException($"{NameHelper}: Header should be before data");
            }

            VerifyUtilities.EnsureEqual(NameHelper, "UnknownData size", UnknownData.Data.Length, UnknownDataSize);
        }
    };
}
