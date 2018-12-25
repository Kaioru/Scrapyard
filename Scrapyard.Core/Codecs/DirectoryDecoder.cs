using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scrapyard.Core.Data;

namespace Scrapyard.Core.Codecs
{
    public class DirectoryDecoder : IDecoder<string>
    {
        public INode Decode(string t)
        {
            var root = new EmptyNode();
            var lookup = new Dictionary<string, INode>();
            var level = new List<string> {t};

            level.ForEach(c => lookup[c] = root);

            while (level.Count > 0)
            {
                var nextLevel = new List<string>();

                foreach (var d in level)
                {
                    var children = Directory.GetDirectories(d).ToList();
                    INode current = new EmptyNode();

                    current.Name = Path.GetFileName(d);

                    foreach (var file in Directory.GetFiles(d))
                    {
                        INode child = new EmptyNode();

                        if (Path.GetExtension(file).Equals(".json"))
                        {
                            var jsonDecoder = new JSONDecoder();

                            using (var text = File.OpenText(file))
                            using (var reader = new JsonTextReader(text))
                                child = jsonDecoder.Decode(JToken.ReadFrom(reader));
                        }

                        child.Name = Path.GetFileNameWithoutExtension(file);
                        current.Children.Add(child);
                    }

                    nextLevel.AddRange(children);

                    if (lookup.ContainsKey(d))
                        lookup[d].Children.Add(current);
                    children.ForEach(c => lookup[c] = current);
                }

                level.Clear();
                level.AddRange(nextLevel);
            }

            return root.Children.FirstOrDefault() ?? root;
        }
    }
}