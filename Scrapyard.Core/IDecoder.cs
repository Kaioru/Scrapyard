using Scrapyard.Core.Data;

namespace Scrapyard.Core
{
    public interface IDecoder<in T>
    {
        INode Decode(T t);
    }
}