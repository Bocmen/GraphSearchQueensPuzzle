namespace GraphSearchQueensPuzzle.LabChessFrezi
{
    public struct Vector2Int
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Vector2Int(int x, int y)
        {
            X=x;
            Y=y;
        }
        public static bool operator !=(Vector2Int v1, Vector2Int v2) => v1.X != v2.X || v1.Y != v2.Y;
        public static bool operator ==(Vector2Int v1, Vector2Int v2) => !(v1 != v2);

        public override bool Equals(object obj)
        {
            return obj is Vector2Int @int&&
                   X==@int.X&&
                   Y==@int.Y;
        }
        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode=hashCode*-1521134295+X.GetHashCode();
            hashCode=hashCode*-1521134295+Y.GetHashCode();
            return hashCode;
        }

        public override string ToString() => $"X {X} Y {Y}";
    }
}
