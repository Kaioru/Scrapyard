using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Scrapyard.Core.Data;

namespace Scrapyard.Core.Codecs
{
    public class JSONDecoder : IDecoder<JToken>
    {
        public INode Decode(JToken token)
        {
            var root = new EmptyNode();
            var lookup = new Dictionary<string, INode>();
            var level = token.Children().ToList();

            level.ForEach(c => lookup[c.Path] = root);

            while (level.Count > 0)
            {
                var nextLevel = new List<JToken>();

                foreach (var t in level)
                {
                    var children = t.Children().ToList();

                    if (!(t is JProperty))
                    {
                        INode current = new EmptyNode();
                        var name = (t.Parent as JProperty)?.Name;

                        if (t.Parent is JArray arr)
                            name = arr.IndexOf(t).ToString();
                        if (t is JValue val)
                            switch (val.Type)
                            {
                                case JTokenType.Integer:
                                    current = new Node<long> {Data = val.Value<long>()};
                                    break;
                                case JTokenType.Float:
                                    current = new Node<double> {Data = val.Value<double>()};
                                    break;
                                case JTokenType.String:
                                    current = new Node<string> {Data = val.Value<string>()};
                                    break;
                                case JTokenType.Boolean:
                                    current = new Node<long> {Data = val.Value<bool>() ? 1 : 0};
                                    break;
                            }

                        current.Name = name;

                        if (lookup.ContainsKey(t.Path))
                            lookup[t.Path].Children.Add(current);
                        children.ForEach(c => lookup[c.Path] = current);
                    }

                    nextLevel.AddRange(children);
                }

                level.Clear();
                level.AddRange(nextLevel);
            }

            return root;
        }
    }
}