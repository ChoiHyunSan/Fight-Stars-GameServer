using System.Numerics;

public class MapData
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private bool[,] _collision;

    public MapData(int[,] rawMap)
    {
        Height = rawMap.GetLength(0);
        Width = rawMap.GetLength(1);
        _collision = new bool[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _collision[x, y] = (rawMap[y, x] == 1); // 1이면 벽으로 간주
            }
        }
    }

    public bool IsBlocked(Vector2 position)
    {
        int x = (int)Math.Floor(position.X);
        int y = (int)Math.Floor(position.Y);

        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return true; // 맵 밖 = 벽으로 간주

        return _collision[x, y];
    }
}
