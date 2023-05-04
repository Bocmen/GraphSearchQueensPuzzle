using ConsoleLibrary.ConsoleExtensions;
using CoreGraphSearch.Models;
using GraphSearchQueensPuzzle.LabChessFrezi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SharedWorksQueensPuzzle.Works
{
    [WorkInvoker.Attributes.LoaderWorkBase("Анализ эффективности алгоритмов", "", Const.NameGroupWorks)]
    public class Analytics: WorkInvoker.Abstract.WorkBase
    {
        public override async Task Start(CancellationToken token)
        {
            IEnumerable<FunctionConsts.ModelType> modelTypen = Enum.GetValues(typeof(FunctionConsts.ModelType)).Cast<FunctionConsts.ModelType>();
            int size = await Abstract.WorkBase.LoadWork(Console, $"Анализ следующих алгоритмов: {string.Join(", ", modelTypen.Select(x => $"'{FunctionConsts.GetNameModelType(x)}'"))}", token);
            var editor = await Console.WriteLineReturnEditor("", ConsoleIOExtension.TextStyle.IsTitle);
            Dictionary<string, IEnumerable<NodeState<CostPath>>> dataAnalytics = new Dictionary<string, IEnumerable<NodeState<CostPath>>>();
            foreach (var node in modelTypen)
            {
                string nameModel = FunctionConsts.GetNameModelType(node);
                await editor.SetContent($"Подождите идёт создание моделей и расчёт результатов для алгоритма: {nameModel}");
                dataAnalytics.Add(nameModel, FunctionConsts.CreateModel(size, node).GetResult(token: token).ToList());
            }
            if (token.IsCancellationRequested) return;
            _ = editor.SetContent("Подождите идёт сравнение алгоритмов");
            var resultAnalytics = AnalysisTool.Create(dataAnalytics);
            _ = editor.SetContent($"Всего решений: {resultAnalytics.IterationResultRating.Count()}");
            Console.SetDecorationOneElem();
            bool isDrawFullIterationResultRating = await Console.ReadBool("Хотите получить рейтинг по каждому результату?", token: token);
            bool isDrawIterationResultRating = await Console.ReadBool("Хотите выигрышные алгоритмы по каждому результату?", token: token);
            if (isDrawFullIterationResultRating)
                await DrawFullIterationResultRating(resultAnalytics);
            if (isDrawIterationResultRating)
                await DrawIterationResultRating(resultAnalytics);
            await DrawRating(resultAnalytics);
            await Purposefulness(AnalysisTool.CreatePurposefulness(dataAnalytics));
        }
        private async Task DrawFullIterationResultRating(AnalysisTool.AnalysisData analysisData)
        {
            List<object> title = new List<object>()
            {
                "Название",
                "Рейтинг",
                "Нормализованный рейтинг",
                "Индекс раскрытия результата",
                "Количество открытых вершин в этот момент"
            };
            foreach (var node in analysisData.FullIterationResultRating)
            {
                Console.StartCollectionDecorate();
                await Console.WriteLine($"Номер результата: {node.Key + 1}", ConsoleIOExtension.TextStyle.IsTitle);
                List<List<object>> tableData = new List<List<object>>()
                {
                    title
                };
                tableData.AddRange(node.Value.Select(x => new List<object>()
                {
                    x.Name,
                    x.Rating,
                    x.NormalizationRating,
                    x.IndexOpen,
                    x.CountOpen
                }));
                await Console.DrawTableUseGrid(tableData, isTransposition: true);
            }
        }
        private async Task DrawIterationResultRating(AnalysisTool.AnalysisData analysisData)
        {
            List<List<object>> tableData = new List<List<object>>()
            {
                new List<object>()
                {
                    "Номер результата",
                    "Лучшие алгоритмы"
                }
            };
            Console.StartCollectionDecorate();
            await Console.WriteLine("Топ алгоритмов среди каждого результата", ConsoleIOExtension.TextStyle.IsTitle);
            tableData.AddRange(analysisData.IterationResultRating.Select(x => new List<object>()
            {
                x.Key + 1,
                string.Join(", ", x.Value)
            }));
            await Console.DrawTableUseGrid(tableData, isTransposition: true);
        }
        private async Task DrawRating(AnalysisTool.AnalysisData analysisData)
        {
            List<List<object>> tableData = new List<List<object>>()
            {
                new List<object>()
                {
                    "Название алгоритма",
                    "Рейтинг",
                    "Нормализованный рейтинг"
                }
            };
            tableData.AddRange(analysisData.Rating.OrderByDescending(x => x.rating).Select(x => new List<object>()
            {
                x.name,
                x.rating,
                x.normalizationRating
            }));
            Console.StartCollectionDecorate();
            await Console.WriteLine("Средний рейтинг алгоритмов", ConsoleIOExtension.TextStyle.IsTitle);
            await Console.DrawTableUseGrid(tableData, isTransposition: true);
        }
        private async Task Purposefulness(IEnumerable<KeyValuePair<string, double>> data)
        {
            Console.StartCollectionDecorate();
            await Console.WriteLine("Сравнение по критерию целенаправленности\n(первый результат перебора)", ConsoleIOExtension.TextStyle.IsTitle);
            List<List<object>> tableData = new List<List<object>>()
            {
                new List<object>()
                {
                    "Алгоритм",
                    "Целенаправленность"
                }
            };
            foreach (var node in data.OrderBy(x => x.Value))
            {
                tableData.Add(new List<object>
                {
                    node.Key,
                    node.Value
                });
            }
            await Console.DrawTableUseGrid(tableData, isTransposition: true);
        }
    }
}
