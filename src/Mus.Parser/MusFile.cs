using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mus.Parser.MusFileData;
using Mus.Parser.Utils;

namespace Mus.Parser
{
    public class MusFile
    {
        public const int EofPadding = 0x100;
        public MusFileHeader Header { get; set; }
        public List<MusFileSegment> Segments { get; set; }

        public static MusFile Parse(BinaryReader reader)
        {
            var file = new MusFile();
            file.Header = MusFileHeader.Parse(reader);
            file.Segments = new List<MusFileSegment>();
            for (var i = 0; i < file.Header.Count; ++i)
            {
                file.Segments.Add(MusFileSegment.Parse(reader));
            }
            return file;
        }

        public void Dump(BinaryWriter writer)
        {
            Header.Dump(writer);
            foreach (var segment in Segments)
            {
                segment.Dump(writer);
            }
            var endOfHeaders = writer.BaseStream.Position;
            if (endOfHeaders >= Segments.First().SegmentHeader.ActualHeaderOffset)
            {
                throw new InvalidDataException($"Segment DataHeaders should be after SegmentHeaders");
            }
            writer.BaseStream.Seek(0, SeekOrigin.End);
            var currentEof = writer.BaseStream.Position;
            var paddingLeft = EofPadding - (currentEof % EofPadding);
            writer.Write(new byte[paddingLeft]);
            if (writer.BaseStream.Position % EofPadding != 0)
            {
                throw new InvalidDataException("Eof Padding failed");
            }
        }

        public void Verify()
        {
            Header.Verify();

            VerifyUtilities.EnsureEqual("MusFile", "number of segments", Segments.Count, Header.Count);

            var first = Segments.First().SegmentHeader;
            var last = Segments.Last().SegmentHeader;
            if (last.ActualHeaderOffset + last.HeaderSize >= first.ActualDataOffset)
            {
                throw new InvalidDataException("First segment Data overlaps with last segment's DataHeader!");
            }

            for (var i = 0; i < Segments.Count; ++i)
            {
                var current = Segments[i];
                current.Verify();
                if (i > 0)
                {
                    var currentHeader = current.SegmentHeader;
                    var previous = Segments[i - 1];
                    var previousHeader = previous.SegmentHeader;

                    if (previousHeader.ActualHeaderOffset + previousHeader.HeaderSize > currentHeader.ActualHeaderOffset)
                    {
                        throw new InvalidDataException($"Segment DataHeaders overlap: {i - 1} and {i}");
                    }

                    if (previousHeader.ActualDataOffset + previousHeader.DataSize > currentHeader.ActualDataOffset)
                    {
                        throw new InvalidDataException($"Segment Datas overlap: {i - 1} and {i}");
                    }
                }
            }
        }
    }
}
