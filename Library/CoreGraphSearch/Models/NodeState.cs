using System.Collections.Generic;
using System.Linq;

namespace CoreGraphSearch.Models
{
    public class NodeState<T>
    {
        public ValueInfo<T> ValueInfo { get; private set; }
        public NodeState<T> Parent { get; private set; }
        public IEnumerable<NodeState<T>> Parents { get; private set; }
        public IEnumerable<NodeState<T>> ParentsAddCurrent { get; private set; }
        public int Depth { get; private set; }
        public Statistics? Statistics { get; private set; }

        public NodeState(ValueInfo<T> valueInfo, NodeState<T> parent = null)
        {
            ValueInfo = valueInfo;
            Parent = parent;
            Parents = GetParents(parent);
            ParentsAddCurrent = Parents.Append(this);
            Depth = Parents.Count() + 1;
        }
        internal void Open(Statistics statistics)
        {
            if(Statistics == null) Statistics = statistics;
        }
        private static IEnumerable<NodeState<T>> GetParents(NodeState<T> node)
        {
            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
            yield break;
        }


        public override string ToString() => ValueInfo.Value.ToString();
    }
}
