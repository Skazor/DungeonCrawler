using System;
using System.Collections.Generic;
using DungeonCrawler.Interfaces;

namespace DungeonCrawler.Entities;

/* Representerer en fiende i spillet 'E'.
   Arver fra Entity (får posisjon X/Y og må definere Symbol/Color).
   Implementerer IDamageable (kan ta skade og ha HP/angrepskraft).
   Fiender starter med 30 HP og gjør 5 skade per angrep.*/
public class Enemy : Entity, IDamageable
{
    /*Symbolet som vises på skjermen for fienden.
      Override fra Entity.*/
    public override char Symbol => 'E';
    public override ConsoleColor Color => ConsoleColor.Red;

    /*Fiendens helse (HP), starter på 30.
      private set: Kan bare endres inne i denne klassen (via TakeDamage).*/
    public int Health { get; private set; } = 60;   // var 30
    public int AttackPower { get; private set; } = 15; // var 5

    // Enkel sjekk om fienden fortsatt lever (brukes for å vite om den skal fjernes)
    public bool IsAlive => Health > 0;

    /*Konstruktør som setter startposisjon for fienden.
      Kaller base-konstruktøren i Entity for å lagre X og Y.
      "x" Start-kolonne
      "y" Start-rad*/
    public Enemy(int x, int y) : base(x, y)
    {
    }

    /*Overload 1: Enkel skade uten melding.
      Reduserer HP med gitt mengde og sørger for at HP ikke går under 0.
      "amount" Hvor mye skade fienden tar*/
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
    }

    /*Overload 2: Skade + melding som skrives ut i gult.
      Kaller overload 1 for selve skade-logikken, så legger til melding.
      Brukes i kamp for å vise hva som skjedde (eks. "Du traff fienden!").
      "amount" Hvor mye skade
      "message" Melding som vises*/
    public void TakeDamage(int amount, string message)
    {
        TakeDamage(amount);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    
    public void MoveTowards(int playerX, int playerY, GameMap map, List<Enemy> others)
    {
      int dx = 0;
      int dy = 0;

      if (playerX > X) dx = 1;
      else if (playerX < X) dx = -1;

      if (playerY > Y) dy = 1;
      else if (playerY < Y) dy = -1;

      // Sjekk om destinasjonen er opptatt av en annen fiende
      bool occupied(int nx, int ny) => others.Any(e => e != this && e.X == nx && e.Y == ny);

      // Prøv horisontalt
      if (dx != 0 && !map.IsWall(X + dx, Y) && !occupied(X + dx, Y))
      {
          X += dx;
          return;
      }

        // Prøv vertikalt
        if (dy != 0 && !map.IsWall(X, Y + dy) && !occupied(X, Y + dy))
        {
          Y += dy;
          return;
        }
      }
}