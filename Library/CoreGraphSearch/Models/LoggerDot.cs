using CoreGraphSearch.Extension;
using System.Collections.Generic;
using System.Linq;
using static CoreGraphSearch.Models.LoggerDot;

namespace CoreGraphSearch.Models
{
    public static class LoggerDot
    {
        public delegate void Write(string value);
        public delegate void DrawDotNode<T>(Write writer, NodeState<T> node, NodeType nodeType, long name, long? parentName);

        public enum NodeType : byte
        {
            Unknown,
            Result,
            DeadEnd,
            ParentResult,
            ParentDeadEnd
        }
    }
    public class LoggerDot<T>
    {
        private readonly static IEnumerable<NodeState<T>> EmptyConst = Enumerable.Empty<NodeState<T>>();

        private IEnumerable<NodeState<T>> _nodesCloused = EmptyConst;
        private IEnumerable<NodeState<T>> _nodesOpened = EmptyConst;

        public void SetData(IEnumerable<NodeState<T>> nodeOpened, IEnumerable<NodeState<T>> nodeCloused)
        {
            _nodesCloused = nodeCloused;
            _nodesOpened = nodeOpened;
        }
        public void SetData(IEnumerable<NodeState<T>> nodeCloused)
        {
            _nodesCloused = nodeCloused;
            _nodesOpened = EmptyConst;
        }

        public void DrawToDot(Write writer, DrawDotNode<T> nodeDraw)
        {
            HashSet<long> visited = new HashSet<long>();
            writer("digraph G {");
            void draw(NodeState<T> node, NodeType nodeType, long name)
            {
                if (!visited.Contains(name))
                {
                    visited.Add(name);
                    nodeDraw(writer, node, nodeType, name, node.Parent?.Statistics?.IndexOpen);
                }
            }
            foreach (var node in _nodesCloused)
            {
                if (!node.TryGetStatistics(out Statistics statistics) || !statistics.IsResult) continue;
                draw(node, NodeType.Result, statistics.IndexOpen);
                foreach (var parentNode in node.Parents)
                {
                    if (!parentNode.TryGetStatistics(out Statistics parentStatistics)) continue;
                    draw(parentNode, NodeType.ParentResult, parentStatistics.IndexOpen);
                }
            }
            foreach (var node in _nodesCloused)
            {
                if (node.TryGetStatistics(out Statistics statistics) && !statistics.IsResult)
                    draw(node, statistics.CountChild == 0 ? NodeType.DeadEnd : NodeType.ParentDeadEnd, statistics.IndexOpen);
            }
            int uniqueIndex = 0;
            foreach (var node in _nodesOpened)
                draw(node, NodeType.Unknown, --uniqueIndex);
            writer("}");
        }
    }
}
