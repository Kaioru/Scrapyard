using System.Collections.Generic;

namespace Scrapyard.Core.Data
{
    public class Node<T> : INode
    {
        public string Name { get; set; }
        public ICollection<INode> Children { get; set; }
        public T Data { get; set; }

        public Node() => Children = new List<INode>();
    }
}