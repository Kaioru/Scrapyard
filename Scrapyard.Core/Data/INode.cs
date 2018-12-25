using System.Collections.Generic;

namespace Scrapyard.Core.Data
{
    public interface INode
    {
        string Name { get; set; }
        ICollection<INode> Children { get; set; }
    }
}