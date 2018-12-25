using System.IO;
using Scrapyard.Core.Data;

namespace Scrapyard.Core
{
    public interface IEncoder
    {
        void Encode(BinaryWriter writer, INode node);
    }
}