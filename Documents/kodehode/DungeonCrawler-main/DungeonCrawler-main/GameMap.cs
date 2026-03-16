using System;
using System.Collections.Generic;

namespace DungeonCrawler;

public class GameMap
{
    private char[,] _tiles;
    public int Width { get; private set; }
    public int Height { get; private set; }

    // Liste over alle rom som ble generert – brukes til å plassere spillere, fiender osv.
    public List<Rectangle> Rooms { get; private set; } = new();

    public struct Rectangle
    {
        public int X, Y, W, H;
        public Rectangle(int x, int y, int w, int h) { X = x; Y = y; W = w; H = h; }
        public int CenterX => X + W / 2;
        public int CenterY => Y + H / 2;
    }

    public GameMap(int width = 40, int height = 22)
    {
        Width = width;
        Height = height;
        _tiles = new char[height, width];
        Generate();
    }

    private void Generate()
    {
        var rng = new Random();

        // Fyll alt med vegger først
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _tiles[y, x] = '#';

        // Generer tilfeldige rom
        int maxRooms = 18;
        int minSize = 3;
        int maxSize = 7;

        for (int i = 0; i < maxRooms; i++)
        {
            int w = rng.Next(minSize, maxSize);
            int h = rng.Next(minSize, maxSize);
            int x = rng.Next(1, Width - w - 1);
            int y = rng.Next(1, Height - h - 1);

            var newRoom = new Rectangle(x, y, w, h);

            // Sjekk at rommet ikke overlapper andre rom
            bool overlaps = false;
            foreach (var other in Rooms)
            {
                if (newRoom.X <= other.X + other.W + 1 &&
                    newRoom.X + newRoom.W + 1 >= other.X &&
                    newRoom.Y <= other.Y + other.H + 1 &&
                    newRoom.Y + newRoom.H + 1 >= other.Y)
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps) continue;

            // Grav ut rommet
            for (int ry = y; ry < y + h; ry++)
                for (int rx = x; rx < x + w; rx++)
                    _tiles[ry, rx] = '.';

            // Koble til forrige rom med korridor
            if (Rooms.Count > 0)
            {
                var prev = Rooms[Rooms.Count - 1];
                if (rng.Next(2) == 0)
                {
                    CarveHorizontal(prev.CenterX, newRoom.CenterX, prev.CenterY);
                    CarveVertical(prev.CenterY, newRoom.CenterY, newRoom.CenterX);
                }
                else
                {
                    CarveVertical(prev.CenterY, newRoom.CenterY, prev.CenterX);
                    CarveHorizontal(prev.CenterX, newRoom.CenterX, newRoom.CenterY);
                }
            }

            Rooms.Add(newRoom);
        }
    }

    private void CarveHorizontal(int x1, int x2, int y)
    {
        for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
            _tiles[y, x] = '.';
    }

    private void CarveVertical(int y1, int y2, int x)
    {
        for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
            _tiles[y, x] = '.';
    }

    public bool IsWall(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return true;
        return _tiles[y, x] == '#';
    }

    public char GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return '#';
        return _tiles[y, x];
    }

    public void RenderBackground()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char tile = GetTile(x, y);
                Console.ForegroundColor = tile switch
                {
                    '#' => ConsoleColor.Blue,
                    '.' => ConsoleColor.DarkYellow,
                    _ => ConsoleColor.White
                };
                Console.Write(tile + " ");
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }
}