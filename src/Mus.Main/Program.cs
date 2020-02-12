using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Mus.Parser;

namespace Mus.Main
{
    class Program
    {
        const string DecoderPath = "files/ealayer3-0.7.0-win32/ealayer3.exe";
        static string Decoder
        {
            get
            {
                if(File.Exists(DecoderPath))
                {
                    return DecoderPath;
                }
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DecoderPath);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Run(args);
                Console.WriteLine("Complete!");
            }
            catch
            {
                Console.Error.WriteLine("Critical: task failed");
                throw;
            }
        }

        static void Run(string[] args)
        {

            Console.WriteLine("args => input path (.xml or .mus), manually backup files before running this tool.");
            Console.WriteLine("Delete cache file manually if you don't want to use it.");
            foreach(var inputName in args)
            {
                var extension = Path.GetExtension(inputName).ToLowerInvariant();
                switch(extension)
                {
                    case ".mus":
                    case ".cdata":
                        Console.WriteLine($"Serializing \"{inputName}\" to xml...");
                        Serialize(inputName);
                        Console.WriteLine($"Successfully serialized \"{inputName}\" to xml");
                        break;
                    case ".xml":
                        Console.WriteLine($"Deserializing \"{inputName}\" to mus...");
                        Deserialize(inputName);
                        Console.WriteLine($"Successfully deserialized \"{inputName}\" to mus");
                        break;
                }
            }
        }

        static void Serialize(string inputName)
        {
            var outputName = Path.ChangeExtension(inputName, ".xml");
            var cache = new Cache();
            using (var input = new FileStream(inputName, FileMode.Open))
            using (var reader = new BinaryReader(input))
            using (var output = new FileStream(outputName, FileMode.Create))
            {
                var musFile = MusFile.Parse(reader);
                var serializer = new XmlSerializer(typeof(MusFile));
                serializer.Serialize(output, musFile);
                var mp3Folder = Directory.CreateDirectory(Path.ChangeExtension(inputName, ".directory"));
                Console.WriteLine("Successfully deserialized mus file to xml");
                Console.WriteLine($"Extracting audio to \"{mp3Folder.FullName}\"...");

                foreach (var segment in musFile.Segments)
                {
                    var outputPathIndex = Path.Combine(mp3Folder.FullName, $"{segment.Index}");
                    var outputEalayer3 = outputPathIndex + ".ealayer3";
                    segment.ExtractAudioData(reader, outputEalayer3);
                    var outputMp3 = outputPathIndex + ".mp3";
                    var options = new ProcessStartInfo
                    {
                        FileName = Decoder,
                        Arguments = $"\"{outputEalayer3}\" -o \"{outputMp3}\" -v -m",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };
                    using (var process = Process.Start(options))
                    {
                        DataReceivedEventHandler write = (_, e) => Console.Write(e.Data + '\n');
                        process.OutputDataReceived += write;
                        process.ErrorDataReceived += write;

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            throw new Exception("EALayer3 -> MP3 Conversion failed");
                        }
                    }

                    cache.AddFile(outputEalayer3);
                    cache.AddFile(outputMp3);
                }
            }

            Console.WriteLine("Successfully extracted audio data, saving cache data...");
            cache.ToXml(Path.ChangeExtension(inputName, ".cache.xml"));
        }

        static void Deserialize(string inputName)
        {
            var cache = Cache.FromXml(Path.ChangeExtension(inputName, ".cache.xml"));
            var outputName = $"{inputName}.mus";

            using (var input = new FileStream(inputName, FileMode.Open))
            using (var output = new FileStream(outputName, FileMode.Create))
            using (var writer = new BinaryWriter(output))
            {
                var serializer = new XmlSerializer(typeof(MusFile));
                var musFile = (MusFile)serializer.Deserialize(input);
                Console.WriteLine($"Xml loaded, number of segments: {musFile.Segments.Count}, validating...");
                musFile.Verify();
                Console.WriteLine($"Mus file verified, dumping data to \"{outputName}\"...");

                var mp3Folder = new DirectoryInfo(Path.ChangeExtension(inputName, ".directory"));

                Console.WriteLine("Inserting audio data...");
                var notChangedCount = 0;
                Action<int> printUnchangedCount = count =>
                {
                    if (count != 0)
                    {
                        Console.WriteLine($"Skipped re-encoding for {count} files because they are not changed");
                    }
                };
                foreach (var segment in musFile.Segments)
                {
                    var outputPathIndex = Path.Combine(mp3Folder.FullName, $"{segment.Index}");
                    var inputMp3 = outputPathIndex + ".mp3";
                    var inputEalayer3 = outputPathIndex + ".ealayer3";

                    if (cache.IsChanged(inputMp3) || cache.IsChanged(inputEalayer3))
                    {
                        printUnchangedCount(notChangedCount);
                        notChangedCount = 0;
                        Console.WriteLine($"\"{inputMp3}\" is changed. Re-encoding to EALayer3...");

                        var options = new ProcessStartInfo
                        {
                            FileName = Decoder,
                            Arguments = $"-E \"{inputMp3}\" -o \"{inputEalayer3}\" -v",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                        };
                        using (var process = Process.Start(options))
                        {
                            DataReceivedEventHandler write = (_, e) => Console.Write(e.Data + '\n');
                            process.OutputDataReceived += write;
                            process.ErrorDataReceived += write;

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();
                            process.WaitForExit();
                            if (process.ExitCode != 0)
                            {
                                throw new Exception("MP3 -> EALayer3 Conversion failed");
                            }
                        }
                    }
                    else
                    {
                        ++notChangedCount;
                    }

                    segment.InsertAudioData(writer, inputEalayer3);
                }
                printUnchangedCount(notChangedCount);
                musFile.Dump(writer);
            }
        }
    }
}
