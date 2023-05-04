using CoreGraphSearch.Models;
using GraphSearchQueensPuzzle.LabChessFrezi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoreGraphSearch.Models.LoggerDot;

namespace QueensPuzzleResultGenerator
{
    internal class Program
    {
        static void Main()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 1; i < 12; i++)
            {
                Console.WriteLine($"Генерирую результаты с размером поля: {i}");
                if (i < 7)
                    tasks.Add(GennerateResult(i, $"{i}_PNG_Full", true, true, true, true, TypeConvert.PNG));
                else
                    tasks.Add(GennerateResult(i, $"{i}_PNG", true, i < 10, false, false, TypeConvert.PNG));
            }
            foreach (var task in tasks) task.Wait();
        }

        private static Task GennerateResult(int size, string directory = "", bool isPurposefulness = true, bool generateResult = true, bool generateStepsResult = true, bool generateSteps = true, TypeConvert typeConvert = TypeConvert.PNG)
        {
            if (!string.IsNullOrWhiteSpace(directory))
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
                Directory.CreateDirectory(directory);
            }
            else
                directory = string.Empty;
            List<Task> tasks = new List<Task>();
            Dictionary<string, IEnumerable<NodeState<CostPath>>> results = new Dictionary<string, IEnumerable<NodeState<CostPath>>>();
            foreach (var typeModel in Enum.GetValues(typeof(FunctionConsts.ModelType)).Cast<FunctionConsts.ModelType>())
            {
                string directoryFullSaveImages = Path.Combine(directory, typeModel.ToString());
                string directoryResultsSaveImages = Path.Combine(directoryFullSaveImages, "Result");
                if (Directory.Exists(directoryFullSaveImages)) Directory.Delete(directoryFullSaveImages, true);
                Directory.CreateDirectory(directoryResultsSaveImages);
                HashSet<uint> visited = new HashSet<uint>();
                int indexFile = 0;
                var model = FunctionConsts.CreateModel(size, typeModel);
                Func<Task> getTask;
                Write writer;
                results.Add(typeModel.ToString(), model.GetResult((openedNodes, clousedNodes) =>
                {
                    if (generateStepsResult)
                    {
                        foreach (var node in clousedNodes)
                        {
                            if (node.Statistics.Value.IsResult && !visited.Contains(node.Statistics.Value.IndexOpen))
                            {
                                visited.Add(node.Statistics.Value.IndexOpen);
                                getTask = SaveDot(out writer, Path.Combine(directoryResultsSaveImages, node.Statistics.Value.IndexOpen.ToString()), typeConvert);
                                model.WriteToDot(writer, ArrangementQueens.ColorSetting.DefaultInDepthSelectNode);
                                tasks.Add(getTask());
                            }
                        }
                    }
                    if (generateSteps)
                    {
                        getTask = SaveDot(out writer, Path.Combine(directoryFullSaveImages, (indexFile++).ToString()), typeConvert);
                        model.WriteToDot(writer, ArrangementQueens.ColorSetting.DefaultInDepthSelectNode);
                        tasks.Add(getTask());
                    }
                }).ToList());
                if (generateResult)
                {
                    getTask = SaveDot(out writer, Path.Combine(directory, $"{typeModel}_ExitResult"), typeConvert);
                    model.WriteToDot(writer, ArrangementQueens.ColorSetting.DefaultInDepthSelectNode);
                    tasks.Add(getTask());
                }
            }
            var statistics = AnalysisTool.Create(results);
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(Path.Combine(directory, "Statistics.csv")), Encoding.UTF8))
            {
                foreach (var nodeIteration in statistics.FullIterationResultRating)
                {
                    writer.WriteLine($"Эффективность относительно других {nodeIteration.Key};Нормализованная эффективность;Имя алгоритма;Номер состояния (результата);Количество отрытых вершин (ожидающих прохода);");
                    foreach (var node in nodeIteration.Value)
                    {
                        writer.WriteLine($"{node.Rating};{node.NormalizationRating};{node.Name};{node.IndexOpen};{node.CountOpen}");
                    }
                }
                writer.WriteLine("\n\n\n\nНомер результата;Лучшие алгоритмы;");
                foreach (var node in statistics.IterationResultRating)
                    writer.WriteLine($"{node.Key};{string.Join(", ", node.Value)};");
                writer.WriteLine("\n\n\n\nРейтинг;Нормализованный рейтинг;Алгоритм;");
                foreach (var (name, rating, normalizationRating) in statistics.Rating.OrderBy(x => x.rating))
                    writer.WriteLine($"{rating};{normalizationRating};{name}");
            }
            if (isPurposefulness)
            {
                using (StreamWriter writer = new StreamWriter(File.OpenWrite(Path.Combine(directory, "Purposefulness.csv")), Encoding.UTF8))
                {
                    writer.WriteLine("Алгоритм;Целенаправленность");
                    foreach (var nodeData in AnalysisTool.CreatePurposefulness(results))
                    {
                        writer.WriteLine($"{nodeData.Key};{nodeData.Value}");
                    }
                }
            }
            return Task.Run(() => Task.WaitAll(tasks.ToArray()));
        }
        private static Func<Task> SaveDot(out Write writer, string fileName, TypeConvert typeConvert = TypeConvert.SVG)
        {
            StreamWriter stream = File.CreateText(fileName);
            writer = stream.Write;
            return () =>
            {
                stream.Close();
                stream.Dispose();
                string typeName = typeConvert == TypeConvert.PNG ? "png" : "svg";
                return Task.Run(() =>
                {
                    try
                    {
                        using (Process process = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "dot.exe",
                            Arguments = $"-T{typeName} {fileName} -o {fileName}.{typeName}",
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        }))
                        {
                            process.WaitForExit();
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Не установлен Graphviz или не добавлин в окружение PATH - https://graphviz.org/download/");
                    }
                    File.Delete(fileName);
                });
            };
        }

        private enum TypeConvert : byte
        {
            PNG,
            SVG
        }
    }
}
