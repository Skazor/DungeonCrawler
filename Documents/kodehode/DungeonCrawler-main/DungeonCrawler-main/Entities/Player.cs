using System;
using DungeonCrawler.Interfaces;

namespace DungeonCrawler.Entities;

public class Player : Entity, IDamageable
{
    private int _maxHealth;

    public override char Symbol => '@';
    public override ConsoleColor Color => ConsoleColor.Yellow;

    public int Health { get; private set; }
    public int AttackPower { get; private set; }
    public int Defense { get; private set; }
    public double CritChance { get; private set; }
    public bool IsAlive => Health > 0;

    // Klassen spilleren valgte ved character creation
    public CharacterClass Class { get; private set; }

    public Player(int startX, int startY, CharacterClass characterClass) : base(startX, startY)
    {
        Class = characterClass;
        _maxHealth = characterClass.MaxHealth;
        Health = _maxHealth;
        AttackPower = characterClass.AttackPower;
        Defense = characterClass.Defense;
        CritChance = characterClass.CritChance;
    }

    public bool TryMove(int dx, int dy, GameMap map)
    {
        int newX = X + dx;
        int newY = Y + dy;
        if (map.IsWall(newX, newY)) return false;
        X = newX;
        Y = newY;
        return true;
    }

    // Defense reduserer skaden som tas – minimum 1 skade alltid
    public void TakeDamage(int amount)
    {
        int reduced = Math.Max(1, amount - Defense);
        Health -= reduced;
        if (Health < 0) Health = 0;
    }

    public void TakeDamage(int amount, string message)
    {
        TakeDamage(amount);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > _maxHealth) Health = _maxHealth;
    }
}