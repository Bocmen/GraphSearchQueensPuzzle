using CoreGraphSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CoreGraphSearch
{
    public static class BruteForce
    {
        public delegate IEnumerable<ValueInfo<T>> OpenNode<T>(NodeState<T> nodeOpen);
        public delegate int NodeSelectOpen<T>(IEnumerable<NodeState<T>> nodes, int countOpenedNodes);
        public delegate bool NodeIsDecision<T>(NodeState<T> node);
        public delegate void LoggingGraph<T>(IEnumerable<NodeState<T>> nodeOpened, IEnumerable<NodeState<T>> nodeCloused);

        public static IEnumerable<NodeState<T>> Calculate<T>
            (
                OpenNode<T> fOpenNode,
                NodeSelectOpen<T> fSelectOpen,
                NodeIsDecision<T> fNodeIsDecision,
                ValueInfo<T> startNode,
                TypeSearch typeSearch,
                CancellationToken? token = null,
                LoggingGraph<T> logIteration = null,
                Action<IEnumerable<NodeState<T>>> lodEnd = null
            )
        {
            List<NodeState<T>> closeNodes = new List<NodeState<T>>();
            List<NodeState<T>> openNodes = new List<NodeState<T>>() { new NodeState<T>(startNode) };
            int countOpenNodes = 1;
            int indexOpen;
            uint indexIterationOpen = 0;

            while (!(token?.IsCancellationRequested ?? false) && openNodes.Any())
            {
                indexOpen = fSelectOpen(openNodes, countOpenNodes);
                NodeState<T> currentNodeOpen = openNodes[indexOpen];
                openNodes.RemoveAt(indexOpen);
                var nodeOpenValues = fOpenNode(currentNodeOpen);
                bool nodeIsDecision = fNodeIsDecision(currentNodeOpen);
                if (nodeOpenValues != null && nodeOpenValues.Any() && (!nodeIsDecision || typeSearch == TypeSearch.Full))
                {
                    int countOpen = nodeOpenValues.Count();
                    nodeOpenValues = nodeOpenValues.Where(x => x.IsIgnore || !openNodes.Concat(closeNodes).Any(y => y.ValueInfo == x)); // Для моего варианта (когда дерево) это рудимент
                    int countSkip = countOpen - nodeOpenValues.Count();
                    countOpenNodes += countOpen - countSkip;
                    if (nodeOpenValues.Any())
                    {
                        foreach (var node in nodeOpenValues)
                            openNodes.Insert(0, new NodeState<T>(node, currentNodeOpen));
                    }
                    currentNodeOpen.Open(new Statistics(countOpen, countSkip, indexIterationOpen, nodeIsDecision, openNodes.Count));
                }
                else
                    currentNodeOpen.Open(new Statistics(0, 0, indexIterationOpen, nodeIsDecision, openNodes.Count));
                closeNodes.Add(currentNodeOpen);
                logIteration.Invoke(openNodes, closeNodes);
                if (nodeIsDecision) yield return currentNodeOpen;
                indexIterationOpen++;
                countOpenNodes--;
            }
            lodEnd?.Invoke(closeNodes);
            yield break;
        }
        public enum TypeSearch: byte
        {
            Full,
            First
        }
    }
}
