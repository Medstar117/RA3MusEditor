using System;
using System.IO;
using System.Xml.Serialization;

namespace Mus.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("args1 input path");
            var inputName = args[1];
            var outputName = $"{inputName}.xml";

            var serializer = new XmlSerializer(typeof(Mus.Parser.MusFile));
            using (var output = new FileStream(outputName, FileMode.Create))
            {
                serializer.Serialize(output, Mus.Parser.Parser.Parse(inputName));
            }
            
        }
    }
}
