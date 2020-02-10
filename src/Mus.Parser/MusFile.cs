using System.IO;
using System.Runtime.InteropServices;
using Mus.Parser.MusFileData;

namespace Mus.Parser
{
    public class MusFile
    {
        public MusFileHeader Header { get; set; }
        
        public static MusFile Parse(BinaryReader reader) {
            var file = new MusFile();
            file.Header = MusFileHeader.Parse(reader);
            return file;
        }
    }
}
