using System.IO;

namespace Mus.Parser.MusFileData
{
    public class MusFileHeader
    {
        public uint Magic; //magic number/hash referenced inside the PathMusicMap file
        public uint Count; //how many audio pieces are in this file
        public uint[] UnknownData;

        public static MusFileHeader Parse(BinaryReader reader) {
            var header = new MusFileHeader();
            header.Magic = reader.ReadUInt32();
            header.Count = reader.ReadUInt32();
            header.UnknownData = new uint[8];
            for (var i = 0; i < header.UnknownData.Length; ++i) {
                header.UnknownData[i] = reader.ReadUInt32();
            }
                
            return header;
        }
    };
}
