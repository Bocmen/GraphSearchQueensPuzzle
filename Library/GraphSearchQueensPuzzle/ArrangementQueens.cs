using CoreGraphSearch;
using CoreGraphSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static CoreGraphSearch.BruteForce;
using static CoreGraphSearch.Models.LoggerDot;

namespace GraphSearchQueensPuzzle.LabChessFrezi
{
    public class ArrangementQueens
    {
        public delegate double GetCost(NodeState<CostPath> parent, Vector2Int vectorCreateNode);
        public Vector2Int SizeField { get; private set; }
        public int SearchDepth { get; private set; }
        private readonly LoggerDot<CostPath> _loggerDot = new LoggerDot<CostPath>();
        private readonly NodeSelectOpen<CostPath> _selectNode;
        private List<NodeState<CostPath>> _buffResult;
        private readonly GetCost _getCost;

        public ArrangementQueens(Vector2Int sizeField, int searchDepth, NodeSelectOpen<CostPath> selectNode, GetCost getCost = null)
        {
            SizeField = sizeField;
            SearchDepth = searchDepth;
            _selectNode = selectNode;
            _getCost = getCost ?? GetCostDefault;
        }
        public ArrangementQueens(int size, NodeSelectOpen<CostPath> selectNode, GetCost getCost = null) : this(new Vector2Int(size, size), size, selectNode, getCost) { }

        private static double GetCostDefault(NodeState<CostPath> parent, Vector2Int vectorCreateNode) => 0;

        public IEnumerable<NodeState<CostPath>> GetResult(LoggingGraph<CostPath> connectIteration = null, CancellationToken? token = null)
        {
            if (_buffResult != null) return _buffResult;
            return CreateResult(connectIteration, token);
        }
        public IEnumerable<NodeState<CostPath>> CreateResult(LoggingGraph<CostPath> connectIteration = null, CancellationToken? token = null)
        {
            List<NodeState<CostPath>> res = new List<NodeState<CostPath>>();
            foreach (var node in BruteForce.Calculate(Open, _selectNode, IsDecision, new ValueInfo<CostPath>(new CostPath(new Vector2Int(-1, -1), 0)), TypeSearch.First, token, (o, c) =>
            {
                connectIteration?.Invoke(o, c);
                _loggerDot.SetData(o, c);
            }))
            {
                res.Add(node);
                yield return node;
            }
            _buffResult = res;
            yield break;
        }
        private bool IsDecision(NodeState<CostPath> node) => (node.Depth - 1) == SearchDepth;
        private IEnumerable<ValueInfo<CostPath>> Open(NodeState<CostPath> node)
        {
            int y = node.ValueInfo.Value.Position.Y;
            if (++y == SizeField.Y) yield break;
            for (int x = 0; x < SizeField.X; x++)
            {
                if (!(node.ParentsAddCurrent?.Where(i => i.ValueInfo.Value.Position.X != -1 || i.ValueInfo.Value.Position.Y != -1).Any(j => j.ValueInfo.Value.Position.X == x || j.ValueInfo.Value.Position.Y == y || Math.Abs(j.ValueInfo.Value.Position.X - x) == Math.Abs(j.ValueInfo.Value.Position.Y - y)) ?? false))
                {
                    var vector = new Vector2Int(x, y);
                    yield return new ValueInfo<CostPath>(new CostPath(vector, _getCost(node, vector)));
                }
            }
            yield break;
        }
        public void WriteToDot(Write writer, ColorSetting colorSetting) => _loggerDot.DrawToDot(writer, (_, node, nodeType, name, parentName) => DrawDotNode(writer, node, nodeType, name, parentName, colorSetting));
        private void DrawDotNode(Write writer, NodeState<CostPath> node, NodeType nodeType, long name, long? parentName, ColorSetting colorSetting)
        {
            writer($"{name}[shape=none, margin=0, label=<<TABLE BORDER=\"1\" COLOR=\"black\" CELLBORDER=\"1\" CELLPADDING=\"3\" CELLSPACING=\"3\" BGCOLOR=\"{colorSetting.GetColor(nodeType)}\">");
            // writer($"<TR><TD CELLPADDING=\"0\" BORDER=\"0\" COLSPAN=\"{SizeField.X}\">{(name >= 0 ? name.ToString() : $"Open: {-name - 1}")}, CO: {node.Statistics?.CountOpen}</TD></TR>");
            writer($"<TR><TD CELLPADDING=\"0\" BORDER=\"0\" COLSPAN=\"{SizeField.X}\">{(name >= 0 ? name.ToString() : $"Open: {-name - 1}")}</TD></TR>");
            for (int y = 0; y < SizeField.Y; y++)
            {
                writer("<TR>");
                for (int x = 0; x < SizeField.X; x++)
                    writer($"<TD BGCOLOR=\"{((node.ParentsAddCurrent?.Any(i => i.ValueInfo.Value.Position.X == x && i.ValueInfo.Value.Position.Y == y) ?? true) ? colorSetting.ColorAnyCell : colorSetting.ColorEmptyCell)}\"></TD>");
                writer("</TR>");
            }
            writer("</TABLE>>];");
            if (parentName != null) writer($"{parentName} -> {name};");
        }
        public readonly struct ColorSetting
        {
            public readonly static ColorSetting DefaultInDepthSelectNode = new ColorSetting("#6cc570", "#ca3a27", "#89c5f9", "#ffc014", "white", "#FFFFFFDD", "#333333");

            public readonly string ColorResult;
            public readonly string ColorDeadEnd;
            public readonly string ColorParentResult;
            public readonly string ColorParentDeadEnd;
            public readonly string ColorUnknown;
            public readonly string ColorEmptyCell;
            public readonly string ColorAnyCell;

            public ColorSetting(string colorResult, string colorDeadEnd, string colorParentResult, string colorParentDeadEnd, string colorUnknown, string colorEmptyCell, string colorAnyCell)
            {
                ColorResult=colorResult;
                ColorDeadEnd=colorDeadEnd;
                ColorParentResult=colorParentResult;
                ColorParentDeadEnd=colorParentDeadEnd;
                ColorUnknown=colorUnknown;
                ColorEmptyCell=colorEmptyCell;
                ColorAnyCell=colorAnyCell;
            }

            public string GetColor(NodeType nodeType)
            {
                switch (nodeType)
                {
                    case NodeType.Result: return ColorResult;
                    case NodeType.DeadEnd: return ColorDeadEnd;
                    case NodeType.ParentResult: return ColorParentResult;
                    case NodeType.ParentDeadEnd: return ColorParentDeadEnd;
                    default: return ColorUnknown;
                }
            }
        }
    }
}
