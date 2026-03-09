using DungeonCrawler.Interfaces;

namespace DungeonCrawler.Entities;

/* Abstrakt baseklasse for alle ting i spillet som har posisjon og skal tegnes (spiller, fiender, potions, utgang).
  "Abstrakt" betyr: denne klassen kan ikke oprettes direkte (new Entity()), man må arve fra den.
   Gir felles logikk og tvinger subklasser til å definere Symbol og Color.*/
public abstract class Entity : IRenderable
{
    /*Horisontal posisjon (kolonne) på kartet.
      protected set: Kan endres inne i denne klassen og subklasser (eks. ved bevegelse), men ikke utenfra.*/
    public int X { get; protected set; }

    /*Vertikal posisjon (rad) på kartet.
      protected set: Samme som over, kan kun endres internt (eks. i TryMove).*/
    public int Y { get; protected set; }

    /*Symbolet som vises på skjermen ('@', 'E', 'H', 'X').
      abstract: Hver subklasse må definere sin egen (override).*/
    public abstract char Symbol { get; }

    /*Fargen symbolet skal tegnes i.
      abstract: Hver subklasse bestemmer sin egen farge (eks. gul for spiller, rød for fiende).*/
    public abstract ConsoleColor Color { get; }

    /*Beskyttet konstruktør – kalles fra subklasser (eks. Player, Enemy).
      Setter startposisjon for alle entities.
      "x" Start-kolonne
      "y" Start-rad*/
    protected Entity(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Merk: Ingen eksplisitt interface-implementasjon nødvendig her.
    // X, Y, Symbol og Color er allerede public og matcher IRenderable-kravene direkte.
}