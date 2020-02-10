using System;
using System.IO;

namespace Mus.Parser
{
    public class Parser
    {
        public static MusFile Parse(string path) {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var reader = new BinaryReader(stream)) {
                return MusFile.Parse(reader);
            }
        }
    }
}
