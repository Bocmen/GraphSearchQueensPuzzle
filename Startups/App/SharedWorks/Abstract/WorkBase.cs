using ConsoleLibrary.ConsoleExtensions;
using ConsoleLibrary.Models;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using GraphSearchQueensPuzzle.LabChessFrezi;
using System.Linq;
using System.Drawing;

namespace SharedWorksQueensPuzzle.Abstract
{
    public abstract class WorkBase: WorkInvoker.Abstract.WorkBase
    {
        protected abstract FunctionConsts.ModelType ModelType { get; }
        public override async Task Start(CancellationToken token)
        {
            int size = await LoadWork(Console, FunctionConsts.GetNameModelType(ModelType), token);
            var model = FunctionConsts.CreateModel(size, ModelType);
            await Console.WriteLine($"Кол-во решения: {model.GetResult(token: token).Count()}");
            await DrawModel(model);
        }
        private async Task DrawModel(ArrangementQueens model)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                model.WriteToDot(writer.Write, ArrangementQueens.ColorSetting.DefaultInDepthSelectNode);
                await writer.FlushAsync();
                stream.Seek(0, SeekOrigin.Begin);
                await Console.DrawWebViewDot(await (new StreamReader(stream)).ReadToEndAsync());
            }
        }

        public static async Task<int> LoadWork(DecoratorConsole console, string title, CancellationToken token)
        {
            console.StartCollectionDecorate();
            await console.WriteLine("Задание из варианта №7", ConsoleIOExtension.TextStyle.IsTitle);
            await console.DrawSeparatorLine();
            await console.WriteLine("Задача об N ферзях. Разместить N ферзей на шахматной доске NxN так, чтобы на каждой горизонта-ли, вертикали и диагонали стояло не более одного ферзя.");
            console.StartCollectionDecorate();
            await console.WriteLine(title, ConsoleIOExtension.TextStyle.IsTitle);
            await console.WriteLine($"{ConsoleLibrary.Extensions.FormattedStringExtension.ColorPattern("Внимание", Color.Yellow)}: не рекомендуется N > 7");
            return await console.ReadInt("N", startRange: 1, token: token, defaultValue: 5);
        }
    }
}
