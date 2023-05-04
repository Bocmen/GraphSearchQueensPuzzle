using CoreGraphSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphSearchQueensPuzzle.LabChessFrezi
{
    public static class FunctionConsts
    {
        public const string NameSearchWidth = "Метод полного перебора";
        public const string NameSearchDepth = "Метод перебора в глубину";
        public const string NameSearchWidthPlus = "Метод равных цен";
        public const string NameSearchDepthPlus = "Метод перебора A*";

        public static double GetCost(NodeState<CostPath> parent, Vector2Int currentCreate)
        {
            var nodes = parent.ParentsAddCurrent.Select(i => i.ValueInfo.Value.Position).Append(currentCreate);
            double x = 0, y = 0;
            int iterCount = 0;
            foreach (var nodeParents in nodes)
            {
                iterCount++;
                x += nodeParents.X;
                y += nodeParents.Y;
            }
            x /= iterCount;
            y /= iterCount;
            double avrgDistance = 0;
            foreach (var nodeParents in nodes)
                avrgDistance += Math.Sqrt(Math.Pow(x - nodeParents.X, 2) + Math.Pow(y - nodeParents.Y, 2));
            return avrgDistance / iterCount;
        }
        public static int SearchWidth(IEnumerable<NodeState<CostPath>> nodesOpened, int countOpenedNodes) => countOpenedNodes - 1;
        public static int SearchDepth(IEnumerable<NodeState<CostPath>> nodesOpened, int countOpenedNodes) => 0;
        public static int SearchDepthPlus(IEnumerable<NodeState<CostPath>> nodesOpened, int countOpenedNodes)
        {
            var parentGroup = nodesOpened.First().Parent;

            int currentIndex = 0;
            double cosMin = double.MaxValue;
            int indexCostMin = currentIndex;
            foreach (var node in nodesOpened)
            {
                if (node.Parent != parentGroup) break;
                if (node.ValueInfo.Value.CostValue < cosMin)
                {
                    cosMin = node.ValueInfo.Value.CostValue;
                    indexCostMin = currentIndex;
                }
                currentIndex++;
            }
            return indexCostMin;
        }
        public static int SearchWidthPlus(IEnumerable<NodeState<CostPath>> nodesOpened, int countOpenedNodes)
        {
            double currentCost = double.MaxValue;
            int currentIndex = 0;
            int indexMinCost = int.MaxValue;
            foreach (var node in nodesOpened)
            {
                if (node.ValueInfo.Value.CostValue < currentCost)
                {
                    currentCost = node.ValueInfo.Value.CostValue;
                    indexMinCost = currentIndex;
                }
                currentIndex++;
            }
            return indexMinCost;
        }
    
        public static ArrangementQueens CreateModel(int size, ModelType modelType)
        {
            switch(modelType)
            {
                case ModelType.SearchDepth: return new ArrangementQueens(size, SearchDepth);
                case ModelType.SearchWidth: return new ArrangementQueens(size, SearchWidth);
                case ModelType.SearchWidthPlus: return new ArrangementQueens(size, SearchWidthPlus, GetCost);
                case ModelType.SearchDepthPlus: return new ArrangementQueens(size, SearchDepthPlus, GetCost);
            }
            throw new ArgumentException("Данный тип не существует", nameof(modelType));
        }

        public static string GetNameModelType(ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.SearchWidth: return NameSearchWidth;
                case ModelType.SearchDepth: return NameSearchDepth;
                case ModelType.SearchWidthPlus: return NameSearchWidthPlus;
                case ModelType.SearchDepthPlus: return NameSearchDepthPlus;
            }
            throw new ArgumentException("Такого типа не существует", nameof(modelType));
        }

        public enum ModelType: byte
        {
            SearchWidth,
            SearchDepth,
            SearchWidthPlus,
            SearchDepthPlus,
        }
    }
}
