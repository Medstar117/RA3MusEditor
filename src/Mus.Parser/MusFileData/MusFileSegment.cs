using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mus.Parser.Utils;

namespace Mus.Parser.MusFileData
{
    public class MusFileSegment
    {
        public MusFileSegmentHeader SegmentHeader;
        public UnknownData DataHeader;
        public uint Index => SegmentHeader.Index;
        public string NameHelper => $"Segment {Index}";

        public static MusFileSegment Parse(BinaryReader reader)
        {
            var ealayer3 = new MusFileSegment();
            ealayer3.SegmentHeader = MusFileSegmentHeader.Parse(reader);
            var streamPosition = reader.BaseStream.Position;
            reader.BaseStream.Seek(ealayer3.SegmentHeader.ActualHeaderOffset, SeekOrigin.Begin);
            ealayer3.DataHeader = new UnknownData(reader.ReadBytes(ealayer3.SegmentHeader.HeaderSize));
            reader.BaseStream.Seek(streamPosition, SeekOrigin.Begin);
            return ealayer3;
        }

        public void Dump(BinaryWriter writer)
        {
            SegmentHeader.Dump(writer);
            var streamPosition = writer.BaseStream.Position;
            writer.BaseStream.Seek(SegmentHeader.ActualHeaderOffset, SeekOrigin.Begin);
            writer.Write(DataHeader.Data);
            writer.BaseStream.Seek(streamPosition, SeekOrigin.Begin);
        }

        public void ExtractAudioData(BinaryReader reader, string outputPath)
        {
            var streamPosition = reader.BaseStream.Position;
            reader.BaseStream.Seek(SegmentHeader.ActualDataOffset, SeekOrigin.Begin);
            File.WriteAllBytes(outputPath, reader.ReadBytes(SegmentHeader.DataSize));
            reader.BaseStream.Seek(streamPosition, SeekOrigin.Begin);
        }

        public void InsertAudioData(BinaryWriter writer, string inputPath)
        {
            var bytes = File.ReadAllBytes(inputPath);
            VerifyUtilities.EnsureEqual(NameHelper, $"file size of \"{inputPath}\"", bytes.Length, SegmentHeader.DataSize);

            var streamPosition = writer.BaseStream.Position;
            writer.BaseStream.Seek(SegmentHeader.ActualDataOffset, SeekOrigin.Begin);
            writer.Write(bytes);
            writer.BaseStream.Seek(streamPosition, SeekOrigin.Begin);
        }

        public void Verify()
        {
            SegmentHeader.Verify();
            VerifyUtilities.EnsureEqual(NameHelper, "data header size", DataHeader.Data.Length, SegmentHeader.HeaderSize);
        }
    }
}
