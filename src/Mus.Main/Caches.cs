using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Mus.Main
{
    public struct CacheData
    {
        public bool ForceCache;
        public DateTime LastWriteTimeUtc;
        public long FileSize;
    }

    public class Cache
    {
        [XmlIgnore]
        public Dictionary<string, CacheData> Caches = new Dictionary<string, CacheData>();

        public void ToXml(string fileName)
        {
            var list = Caches.Select(kv => (kv.Key, kv.Value)).ToList();
            using (var output = new FileStream(fileName, FileMode.Create))
            {
                var serializer = new XmlSerializer(list.GetType());
                serializer.Serialize(output, list);
            }
        }

        public static Cache FromXml(string fileName)
        {
            try
            {
                using (var input = new FileStream(fileName, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(List<(string, CacheData)>));
                    var list = (List<(string, CacheData)>)serializer.Deserialize(input);
                    return new Cache { Caches = list.ToDictionary(x => x.Item1, x => x.Item2) };
                }
            }
            catch
            {
                return new Cache();
            }
        }

        public void AddFile(string path)
        {
            var file = new FileInfo(path);
            Caches[file.FullName] = new CacheData
            {
                ForceCache = false,
                LastWriteTimeUtc = file.LastWriteTimeUtc,
                FileSize = file.Length,
            };
        }

        public bool IsChanged(string path)
        {
            var file = new FileInfo(path);
            if (!Caches.TryGetValue(file.FullName, out var data))
            {
                return true;
            }

            if (data.ForceCache)
            {
                Console.WriteLine($"Force using existing EALayer3 File for {path} because ForceCache is set to true");
                return false;
            }

            if (!file.Exists)
            {
                return true;
            }

            return file.LastWriteTimeUtc != data.LastWriteTimeUtc || file.Length != data.FileSize;
        }
    }
}