using System.IO;
using Mus.Parser.Utils;

namespace Mus.Parser.MusFileData
{
    public class MusFileHeader
    {
        public const int UnknownDataSize = 32;
        public uint Magic; //magic number/hash referenced inside the PathMusicMap file
        public int Count; //how many audio pieces are in this file
        public UnknownData UnknownData;

        public static MusFileHeader Parse(BinaryReader reader)
        {
            var header = new MusFileHeader();
            header.Magic = reader.ReadUInt32();
            header.Count = reader.ReadInt32();
            header.UnknownData = new UnknownData(reader.ReadBytes(UnknownDataSize));
            return header;
        }

        public void Dump(BinaryWriter writer)
        {
            writer.Write(Magic);
            writer.Write(Count);
            writer.Write(UnknownData.Data);
        }

        public void Verify()
        {
            VerifyUtilities.EnsureEqual("MusFileHeader", "UnknownData size", UnknownData.Data.Length, UnknownDataSize);
        }
    };
}
