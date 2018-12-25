using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Scrapyard.Core.Data;

namespace Scrapyard.Core.Codecs
{
    public class NXEncoder : IEncoder
    {
        public void Encode(BinaryWriter writer, INode node)
        {
            writer.Write(new byte[] {0x50, 0x4B, 0x47, 0x34});
            writer.Write(new byte[(4 + 8) * 4]);

            var strings = new Dictionary<string, uint>(StringComparer.Ordinal);
            var nodes = new List<INode>();

            var nodeOffset = (ulong) writer.BaseStream.Position;
            var nodeLevel = new List<INode> {node};

            var ensureMultiple = new Action<int>(multiple =>
            {
                var skip = (int) (multiple - writer.BaseStream.Position % multiple);
                if (skip == multiple)
                    return;
                writer.Write(new byte[skip], 0, skip);
            });
            var addString = new Func<string, uint>(str =>
            {
                if (string.IsNullOrWhiteSpace(str)) str = "";
                if (!strings.ContainsKey(str))
                    strings[str] = (uint) strings.Count;
                return strings[str];
            });

            ensureMultiple(4);

            while (nodeLevel.Count > 0)
            {
                var nextNodeLevel = new List<INode>();
                var nextChildID = nodes.Count + nodeLevel.Count;

                foreach (var n in nodeLevel)
                {
                    writer.Write(addString(n.Name));
                    writer.Write((uint) nextChildID);
                    writer.Write((ushort) n.Children.Count);

                    nextChildID += n.Children.Count;

                    switch (n)
                    {
                        case EmptyNode _:
                            writer.Write((ushort) 0);
                            writer.Write((long) 0);
                            break;
                        case Node<long> longNode:
                            writer.Write((ushort) 1);
                            writer.Write(longNode.Data);
                            break;
                        case Node<double> doubleNode:
                            writer.Write((ushort) 2);
                            writer.Write(doubleNode.Data);
                            break;
                        case Node<string> stringNode:
                            writer.Write((ushort) 3);
                            writer.Write(addString(stringNode.Data));
                            break;
                        case Node<Point> pointNode:
                            writer.Write((ushort) 4);
                            writer.Write(pointNode.Data.X);
                            writer.Write(pointNode.Data.Y);
                            break;
                    }

                    nodes.Add(n);
                    nextNodeLevel.AddRange(n.Children.OrderBy(c => c.Name));
                }

                nodeLevel.Clear();
                nodeLevel.AddRange(nextNodeLevel);
            }

            var stringsReverseLookup = strings.ToDictionary(
                kv => kv.Value,
                kv => kv.Key
            );
            var stringsOffsets = new ulong[stringsReverseLookup.Values.Count];
            var stringsRunningID = 0;

            foreach (var str in stringsReverseLookup.Values)
            {
                ensureMultiple(2);
                stringsOffsets[stringsRunningID++] = (ulong) writer.BaseStream.Position;
                writer.Write((ushort) str.Length);
                writer.Write(Encoding.UTF8.GetBytes(str));
            }

            ensureMultiple(8);

            var stringsTableOffset = (ulong) writer.BaseStream.Position;

            foreach (var offset in stringsOffsets)
            {
                writer.Write(offset);
            }

            writer.Seek(4, SeekOrigin.Begin);
            writer.Write(nodes.Count);
            writer.Write(nodeOffset);
            writer.Write(stringsReverseLookup.Values.Count);
            writer.Write(stringsTableOffset);
            writer.Write((uint) 0);
            writer.Write((ulong) 0);
            writer.Write((uint) 0);
            writer.Write((ulong) 0);
        }
    }
}