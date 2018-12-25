using System.Collections.Generic;

namespace Scrapyard.Core.Data
{
    public class EmptyNode : INode
    {
        public string Name { get; set; }
        public ICollection<INode> Children { get; set; }
        
        public EmptyNode() => Children = new List<INode>();
    }
}