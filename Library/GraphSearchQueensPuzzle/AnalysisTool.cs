using CoreGraphSearch.Models;
using System.Collections.Generic;
using System.Linq;

namespace GraphSearchQueensPuzzle.LabChessFrezi
{
    public static class AnalysisTool
    {
        public static AnalysisData Create(IEnumerable<KeyValuePair<string, IEnumerable<NodeState<CostPath>>>> data)
        {
            Dictionary<int, List<(string name, NodeState<CostPath>)>> dataFullIterationResultRating = new Dictionary<int, List<(string name, NodeState<CostPath>)>>();
            int indexResul;
            foreach (var nodeAlgorithm in data)
            {
                indexResul = 0;
                foreach (var nodeIteration in nodeAlgorithm.Value)
                {
                    if (!dataFullIterationResultRating.ContainsKey(indexResul))
                        dataFullIterationResultRating.Add(indexResul, new List<(string name, NodeState<CostPath>)>());
                    dataFullIterationResultRating[indexResul].Add((nodeAlgorithm.Key, nodeIteration));
                    indexResul++;
                }
            }
            var dataIteration = dataFullIterationResultRating.Select(x => (x.Key, x.Value.GroupBy(v => v.Item2.Statistics.Value.IndexOpen).OrderByDescending(v => v.Key).Select((v, i) => (i + 1, v))));
            IEnumerable<KeyValuePair<int, IEnumerable<NodeFullIterationRating>>> fullIterationResultRating;
            fullIterationResultRating = dataIteration.Select(x =>
            {
                var maxRating = 1.0 / x.Item2.Sum(v => v.Item1 * v.v.Count());
                return new KeyValuePair<int, IEnumerable<NodeFullIterationRating>>(x.Key, x.Item2.SelectMany(v => v.v.Select(y => new NodeFullIterationRating(y.name, y.Item2, v.Item1, maxRating))));
            });
            var iterationResultRating = fullIterationResultRating.Select(x =>
            {
                var grouped = x.Value.GroupBy(v => v.Rating);
                double max = double.MinValue;
                IGrouping<double, NodeFullIterationRating> maxValue = null;
                foreach (var elem in grouped)
                {
                    if (elem.Key > max)
                    {
                        max = elem.Key;
                        maxValue = elem;
                    }
                }
                return new KeyValuePair<int, IEnumerable<string>>(x.Key, maxValue.Select(v => v.Name));
            });
            var rating = fullIterationResultRating.SelectMany(x => x.Value).GroupBy(x => x.Name).Select(x => (x.First().Name, (double)x.Sum(v => v.Rating) / x.Count(), x.Sum(v => v.NormalizationRating) / x.Count()));
            return new AnalysisData(fullIterationResultRating, iterationResultRating, rating);
        }

        public static IEnumerable<KeyValuePair<string, double>> CreatePurposefulness(IEnumerable<KeyValuePair<string, IEnumerable<NodeState<CostPath>>>> data)
        {
            return data.Where(x => x.Value.Any()).Select(x =>
            {
                var value = x.Value.First();

                return new KeyValuePair<string, double>(x.Key, (double)(value.Depth - 1) / value.Statistics.Value.IndexOpen);
            });
        }

        public struct AnalysisData
        {
            public IEnumerable<KeyValuePair<int, IEnumerable<NodeFullIterationRating>>> FullIterationResultRating { get; private set; }
            public IEnumerable<KeyValuePair<int, IEnumerable<string>>> IterationResultRating { get; private set; }
            public IEnumerable<(string name, double rating, double normalizationRating)> Rating { get; private set; }

            public AnalysisData(IEnumerable<KeyValuePair<int, IEnumerable<NodeFullIterationRating>>> fullIterationResultRating, IEnumerable<KeyValuePair<int, IEnumerable<string>>> iterationResultRating, IEnumerable<(string name, double rating, double normalizationRating)> rating)
            {
                FullIterationResultRating=fullIterationResultRating;
                IterationResultRating=iterationResultRating;
                Rating=rating;
            }
        }
        public struct NodeFullIterationRating
        {
            public string Name { get; private set; }
            public uint IndexOpen { get; private set; }
            public int CountOpen { get; private set; }
            public double Rating { get; private set; }
            private readonly double _normalization;
            public double NormalizationRating => Rating * _normalization;

            public NodeFullIterationRating(string name, NodeState<CostPath> node, double rating, double normalization)
            {
                Name=name;
                IndexOpen=node.Statistics.Value.IndexOpen;
                CountOpen=node.Statistics.Value.CountOpen;
                Rating=rating;
                _normalization=normalization;
            }
        }
    }
}
