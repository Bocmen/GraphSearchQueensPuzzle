namespace CoreGraphSearch.Models
{
    public struct Statistics
    {
        public int CountChild { get; private set; }
        public int CountSkipChild { get; private set; }
        public uint IndexOpen { get; private set; }
        public bool IsResult { get; private set; }
        public int CountOpen { get; private set; }

        public Statistics(int countChild, int countSkipChild, uint indexOpen, bool isResult, int countOpen)
        {
            CountChild=countChild;
            CountSkipChild=countSkipChild;
            IndexOpen=indexOpen;
            IsResult=isResult;
            CountOpen=countOpen;
        }
    }
}
