namespace DungeonCrawler.Interfaces;

/*Interface som sier: "Dette er noe som kan tegnes på kartet i konsollen".
  Alle ting som skal vises (spiller, fiender, potions, utgang osv.) må følge dette.
  Fordelen med dette er at det blir en felles liste i Game.cs som holder alle ting som skal rendres.*/
public interface IRenderable
{
    /*Horisontal posisjon (kolonne) på kartet, fra venstre til høyre.
      Brukes for å vite hvor symbolet skal tegnes.*/
    int X { get; }

    /*Vertikal posisjon (rad) på kartet, fra toppen til bunnen.
      Sammen med X bestemmer dette nøyaktig hvor på skjermen tingen skal vises.*/
    int Y { get; }

    /*Tegnet som vises på skjermen ('@', 'E', 'H', 'X').
      Hver klasse som bruker interfacet bestemmer sitt eget symbol.*/
    char Symbol { get; }

    /*Fargen symbolet skal tegnes i (gul for spiller, rød for fiende osv).
      Gjør at ulike ting får ulike farger uten å måtte ha egen logikk i render-metoden.*/
    ConsoleColor Color { get; }
}