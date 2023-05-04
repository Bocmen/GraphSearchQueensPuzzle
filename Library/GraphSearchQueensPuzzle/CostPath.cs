namespace GraphSearchQueensPuzzle.LabChessFrezi
{
    public struct CostPath
    {
        public Vector2Int Position { get; private set; }
        public double CostValue { get; private set; }

        public CostPath(Vector2Int position, double value)
        {
            Position=position;
            CostValue=value;
        }
    }
}
