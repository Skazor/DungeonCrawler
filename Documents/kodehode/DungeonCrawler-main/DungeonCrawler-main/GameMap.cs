using System;

namespace DungeonCrawler;

/*Klasse som holder styr på selve kartet: vegger (#), gulv (.), størrelse og regler for bevegelse.
  Kartet er statisk (endres ikke under spilling) og lagret som en array av strenger.*/
public class GameMap
{
    /*Kartet lagret som array av strenger, hver streng er en rad.
      readonly betyr at vi ikke kan endre arrayen etter at den er satt opp.*/
    private readonly string[] _mapData = new string[]
    {
        "############", // Rad 0 topp
        "#..........#",
        "#..#.......#",
        "#..........#",
        "#....#.....#",
        "####...#####", // Rad 5, midtramme med åpning
        "#..........#",
        "#..........#",
        "############"  // Rad 8 bunn
    };

    /*Bredden på kartet (antall kolonner/tegn i en rad).
      Hentes fra lengden på første rad, alle rader er like lange.*/
    public int Width => _mapData[0].Length;

    /*Høyden på kartet (antall rader).
      Hentes fra antall elementer i _mapData-arrayen.*/
    public int Height => _mapData.Length;

    /*Sjekker om en posisjon (x, y) er en vegg eller utenfor kartet.
      Returnerer true hvis man IKKE kan gå dit (vegg eller ugyldig posisjon).
      Brukes i Player.TryMove() for kollisjon.
      "x" Kolonne (horisontal posisjon, fra venstre)
      "y" Rad (vertikal posisjon, fra toppen)
      Returnerer true hvis posisjonen er vegg eller utenfor kartet.*/
    public bool IsWall(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return true;
        return _mapData[y][x] == '#';
    }

    /*Returnerer tegnet på en gitt posisjon i kartet.
      Returnerer '#' hvis posisjonen er utenfor kartet (trygg fallback).*/
    public char GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return '#';
        return _mapData[y][x];
    }

    /*Tegner kun bakgrunnskartet (vegg og gulv) med farger.
      Brukes i Game.Render() før entities tegnes oppå.
      Legger til mellomrom mellom tegn.*/
    public void RenderBackground()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char tile = GetTile(x, y);
                Console.ForegroundColor = tile switch
                {
                    '#' => ConsoleColor.Blue,       // Vegg
                    '.' => ConsoleColor.DarkYellow,  // Gulv
                    _   => ConsoleColor.White         // Fallback for uventede tegn
                };
                Console.Write(tile + " ");
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }
}