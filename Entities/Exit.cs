using DungeonCrawler.Interfaces;

namespace DungeonCrawler.Entities;

/*Representerer utgangen i spillet (symbolet 'X').
  Målet spilleren skal nå etter å ha drept alle fiender.*/
public class Exit : Entity, IRenderable
{
    /*Symbolet som vises på kartet for utgangen.
      Override fra Entity, dette er det som gjør at vi ser 'X'.*/
    public override char Symbol => 'X';

    /*Fargen på symbolet 'X' når det tegnes.
      Magenta så utgangen skiller seg ut fra fiender og potions.*/
    public override ConsoleColor Color => ConsoleColor.Magenta;

    /*Konstruktør som setter posisjonen til utgangen.
      Kaller base-konstruktøren i Entity for å lagre x og y.
      "x" Kolonne-posisjon på kartet
      "y" Rad-posisjon på kartet*/
    public Exit(int x, int y) : base(x, y)
    {
    }
}