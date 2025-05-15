using System.Numerics;

public class MapData
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private bool[,] _collision;

    private int _xOffset;
    private int _yOffset;

    public MapData(int xMin, int xMax, int yMin, int yMax, string[] rawMapLines)
    {
        Width = xMax - xMin + 1;
        Height = yMax - yMin + 1;

        _xOffset = -xMin;
        _yOffset = -yMin;

        _collision = new bool[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            // 줄이 위에서 아래로 되어있기 때문에 y를 반대로 매핑
            string line = rawMapLines[y];
            int actualY = Height - 1 - y;

            for (int x = 0; x < Width; x++)
            {
                _collision[x, actualY] = line[x] == '1';
            }
        }
    }

    public bool IsBlocked(Vector2 position)
    {
        int x = (int)Math.Floor(position.X + 0.5f) + _xOffset;
        int y = (int)Math.Floor(position.Y + 0.5f) + _yOffset;

        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return true;

        return _collision[x, y];
    }
}