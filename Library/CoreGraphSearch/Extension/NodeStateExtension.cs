using CoreGraphSearch.Models;

namespace CoreGraphSearch.Extension
{
    public static class NodeStateExtension
    {
        public static bool TryGetStatistics<T>(this NodeState<T> node, out Statistics statistics)
        {
            statistics = node.Statistics ?? default;
            return node.Statistics != null;
        }
    }
}
