using GraphSearchQueensPuzzle.LabChessFrezi;

namespace SharedWorksQueensPuzzle.Works
{
    [WorkInvoker.Attributes.LoaderWorkBase(FunctionConsts.NameSearchDepth, "", Const.NameGroupWorks)]
    public class SearchDepth : Abstract.WorkBase
    {
        protected override FunctionConsts.ModelType ModelType => FunctionConsts.ModelType.SearchDepth;
    }
    [WorkInvoker.Attributes.LoaderWorkBase(FunctionConsts.NameSearchDepthPlus, "", Const.NameGroupWorks)]
    public class SearchDepthPlus : Abstract.WorkBase
    {
        protected override FunctionConsts.ModelType ModelType => FunctionConsts.ModelType.SearchDepthPlus;
    }
    [WorkInvoker.Attributes.LoaderWorkBase(FunctionConsts.NameSearchWidth, "", Const.NameGroupWorks)]
    public class SearchWidth : Abstract.WorkBase
    {
        protected override FunctionConsts.ModelType ModelType => FunctionConsts.ModelType.SearchWidth;
    }
    [WorkInvoker.Attributes.LoaderWorkBase(FunctionConsts.NameSearchWidthPlus, "", Const.NameGroupWorks)]
    public class SearchWidthPlus : Abstract.WorkBase
    {
        protected override FunctionConsts.ModelType ModelType => FunctionConsts.ModelType.SearchWidthPlus;
    }
}
