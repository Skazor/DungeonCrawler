using System;
using DungeonCrawler.Interfaces;

namespace DungeonCrawler.Entities;

/*Spilleren, representert med '@'.
  Arver fra Entity (får posisjon X/Y og må definere Symbol/Color).
  Implementerer IDamageable (kan ta skade og ha HP/angrepskraft).*/
public class Player : Entity, IDamageable
{
    // Maks HP spilleren kan ha – brukes som grense i Heal()
    private const int MaxHealth = 100;

    /*Symbolet som vises på skjermen for spilleren.
      Override fra Entity.*/
    public override char Symbol => '@';

    /*Fargen spilleren tegnes i (gul for å skille seg ut).
      Override fra Entity.*/
    public override ConsoleColor Color => ConsoleColor.Yellow;

    /*Spillerens helse (HP), starter på 100.
      private set: Kan bare endres inne i denne klassen (via TakeDamage/Heal).*/
    public int Health { get; private set; } = MaxHealth;

    /*Hvor mye skade spilleren gjør når den angriper fiender.
      private set: Kan endres senere hvis det blir lagt til oppgraderinger.*/
    public int AttackPower { get; private set; } = 12;

    /*Enkel sjekk om spilleren fortsatt lever (brukes i win/lose-sjekk).*/
    public bool IsAlive => Health > 0;

    /*Konstruktør som setter startposisjon.
      Kaller base-konstruktøren i Entity for å lagre X og Y.
      "startX" Start-kolonne
      "startY" Start-rad*/
    public Player(int startX, int startY) : base(startX, startY)
    {
        // Ingen ekstra logikk her, alt håndteres av Entity-basen
    }

    /*Prøver å flytte spilleren i gitt retning (dx, dy).
      Sjekker kollisjon med vegger via GameMap.IsWall().
      Hvis OK oppdaterer X/Y og returnerer true.
      Hvis vegg eller utenfor returnerer false (ingen bevegelse).

      "dx" Endring i X (f.eks. -1 for venstre, +1 for høyre)
      "dy" Endring i Y (f.eks. -1 for opp, +1 for ned)
      "map" Kartet som brukes til kollisjonssjekk
      True hvis bevegelsen lyktes, false hvis blokkert*/
    public bool TryMove(int dx, int dy, GameMap map)
    {
        int newX = X + dx;
        int newY = Y + dy;

        // Sjekk om ny posisjon er vegg eller utenfor
        if (map.IsWall(newX, newY))
            return false;

        // Flytt spilleren
        X = newX;
        Y = newY;
        return true;
    }

    /*Overload 1: Enkel skade uten melding.
      Reduserer HP med gitt mengde og sørger for at HP ikke går under 0.
      "amount" Hvor mye skade som tas*/
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
    }

    /*Overload 2: Skade + melding som skrives ut i rødt.
      Kaller overload 1 for selve skade-logikken, så legger til melding.
      Brukes i kamp for å vise hva som skjedde.
      "amount" Hvor mye skade
      "message" Melding som vises (eks. "Fiende angriper deg!")*/
    public void TakeDamage(int amount, string message)
    {
        TakeDamage(amount); // Kall enkel versjon for å håndtere HP
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /*Øker spillerens HP (eks. ved å plukke opp potion).
      HP kan ikke overstige MaxHealth (100) – healing er begrenset.
      "amount" Hvor mye HP som legges til*/
    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth) Health = MaxHealth; // Klamper til maks
    }
}