using DungeonCrawler.Interfaces;

namespace DungeonCrawler.Entities;

/*Representerer en healthpotion i spillet, 'H'.
  Arver fra Entity (får posisjon X/Y og må definere Symbol/Color).
  Implementerer kun IRenderable (kan tegnes på kartet).
  HealthPotion implementerer IKKE IDamageable – den kan ikke ta skade.*/
public class HealthPotion : Entity, IRenderable
{
    public override char Symbol => 'H';
    public override ConsoleColor Color => ConsoleColor.Green;

    /*Hvor mye HP spilleren får når den plukker opp potionen.
      readonly (kun get): Verdien settes én gang ved opprettelse og kan ikke endres etterpå.*/
    public int HealAmount { get; } = 25;

    /*Konstruktør som setter posisjonen til potionen.
      Kaller base-konstruktøren i Entity for å lagre X og Y.
      Ingen ekstra logikk, potion er passiv og venter bare på å bli plukket opp.
      "x" Kolonne-posisjon på kartet
      "y" Rad-posisjon på kartet*/
    public HealthPotion(int x, int y) : base(x, y)
    {
    }
}