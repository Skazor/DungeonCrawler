using System;
using System.Collections.Generic;
using DungeonCrawler.Entities;
using DungeonCrawler.Interfaces;

namespace DungeonCrawler;

// Hovedklassen som styrer hele spillet: initialisering, spill-loop, rendering, input og oppdatering.
public class Game
{
    // Kartet som inneholder vegger og gulv
    private readonly GameMap _map;
    // Spilleren (arver fra Entity og implementerer IDamageable)
    private readonly Player _player;
    // Liste over alle ting som skal tegnes på skjermen (spiller, fiender, potions, utgang)
    private readonly List<IRenderable> _renderables = new();
    // Liste over alle ting som kan ta skade (spiller og fiender)
    private readonly List<IDamageable> _damageables = new();
    // Utgangen (X), når spilleren når denne og alle fiender er drept, vinner man
    private readonly Exit _exit;
    // Styrer om spillet fortsatt kjører
    private bool _isRunning = true;

    // For midlertidige statusmeldinger som vises i noen sekunder ("Fiende drept!")
    private string _statusMessage = "";
    private DateTime _messageExpires = DateTime.MinValue;
    private readonly TimeSpan _messageDuration = TimeSpan.FromSeconds(2.5);

    /*Konstruktør: Oppretter kart, spiler, fiender, potions og utgang.
      Legger alt i riktige lister så interfaces kan brukes senere.*/
    public Game()
    {
        _map = new GameMap();
        _player = new Player(2, 2, new CharacterClass(ClassType.Warrior)); // Starter på posisjon (2,2) inni området

        // Opprett utgang (X), plassert på en gulv-rute
        _exit = new Exit(10, 7);
        _renderables.Add(_exit);

        // Legg til spilleren (høyest prioritet ved tegning)
        _renderables.Add(_player);

        // Opprett fiender og legg til i begge lister
        var e1 = new Enemy(8, 2);
        var e2 = new Enemy(6, 4);
        var e3 = new Enemy(9, 7);
        _renderables.Add(e1);
        _renderables.Add(e2);
        _renderables.Add(e3);
        _damageables.Add(e1);
        _damageables.Add(e2);
        _damageables.Add(e3);

        // Opprett potions (kan bare rendres, ikke ta skade)
        var p1 = new HealthPotion(4, 7);
        var p2 = new HealthPotion(10, 3);
        _renderables.Add(p1);
        _renderables.Add(p2);
    }

    /*Starter spill-loopen: rendrer, leser input og oppdaterer til spillet avsluttes.*/
    public void Run()
    {
        while (_isRunning)
        {
            Render();       // Tegn alt på skjermen
            HandleInput();  // Les tastetrykk fra spilleren
            Update();       // Oppdater logikk (kamp, heal, win/lose)
        }
    }

    /*Tegner hele skjermen: HUD øverst, kart + entities, og eventuell statusmelding.
      Bruker Console.Clear() for å refreshe skjermen hver frame.*/
    private void Render()
    {
        Console.Clear();
        Console.CursorVisible = false;

        // HUD (spillerens stats), alltid øverst
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"HP: {_player.Health,-3}  ATK: {_player.AttackPower}");
        Console.ResetColor();
        Console.WriteLine();

        // Tegn kartet og alle entities
        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
                IRenderable? drawn = null;

                // Sjekk om spilleren er her (høyest prioritet)
                if (x == _player.X && y == _player.Y)
                    drawn = _player;
                else
                {
                    // Sjekk om noen annen renderable (fiende, potion, exit) er her
                    foreach (var r in _renderables)
                    {
                        if (r.X == x && r.Y == y)
                        {
                            drawn = r;
                            break;
                        }
                    }
                }

                if (drawn != null)
                {
                    Console.ForegroundColor = drawn.Color;
                    Console.Write(drawn.Symbol + " ");
                }
                else
                {
                    char tile = _map.GetTile(x, y);
                    Console.ForegroundColor = tile switch
                    {
                        '#' => ConsoleColor.Blue,
                        '.' => ConsoleColor.DarkYellow,
                        _   => ConsoleColor.White
                    };
                        char display = tile == '#' ? '█' : tile;
                        string spacing = tile == '#' ? "█" : " ";
                        Console.Write(display + spacing);
                }
            }
            Console.WriteLine();
        }

        Console.ResetColor();

        // Vis midlertidig melding hvis den fortsatt er gyldig
        if (DateTime.Now < _messageExpires && !string.IsNullOrEmpty(_statusMessage))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n{_statusMessage}");
            Console.ResetColor();
        }

        Console.WriteLine("\nBruk WASD for å bevege deg. Q for å avslutte.");
    }

    /*Leser tastetryk uten å vise dem på skjermen (ReadKey(true)).
      Utfører bevegelse eller avslutter spillet.*/
    private void HandleInput()
    {
        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.W: _player.TryMove(0, -1, _map); break;
            case ConsoleKey.S: _player.TryMove(0,  1, _map); break;
            case ConsoleKey.A: _player.TryMove(-1, 0, _map); break;
            case ConsoleKey.D: _player.TryMove(1,  0, _map); break;
            case ConsoleKey.Q:
                Console.Clear();
                Console.WriteLine("Takk for at du spilte!");
                _isRunning = false;
                return;
        }
    }

    /*Oppdaterer spilltilstanden hver frame:
      - Sjekker kollisjon med potions (heal)
      - Sjekker kollisjon med fiender (kamp)
      - Sjekker win-condition (alle fiender drept + på utgang)*/
    private void Update()
    {
        // Sjekk om spilleren står på en potion
        for (int i = _renderables.Count - 1; i >= 0; i--)
        {
            if (_renderables[i] is HealthPotion p && p.X == _player.X && p.Y == _player.Y)
            {
                _player.Heal(p.HealAmount);
                _renderables.RemoveAt(i);
                ShowStatus($"Du plukket opp en potion! +{p.HealAmount} HP");
            }
        }
        

        // Sjekker kamp med fiender
        for (int i = _damageables.Count - 1; i >= 0; i--)
        {
            if (_damageables[i] is Enemy e && e.X == _player.X && e.Y == _player.Y && e.IsAlive)
            {
                // Bruker TakeDamage-overloaden med melding – viser feedback direkte fra metoden
                _player.TakeDamage(e.AttackPower, $"Fiende angriper! Du tar {e.AttackPower} skade.");
                e.TakeDamage(_player.AttackPower, $"Du angriper fienden for {_player.AttackPower} skade!");

                if (!e.IsAlive)
                {
                    _damageables.RemoveAt(i);
                    _renderables.Remove(e);
                    ShowStatus("Fiende drept!");
                }

                if (!_player.IsAlive)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GAME OVER – Du døde...");
                    Console.ResetColor();
                    _isRunning = false;
                    return;
                }
            }
            foreach(IDamageable d in _damageables.ToList())
            {
                if (d is Enemy enemy && enemy.IsAlive)
                    enemy.MoveTowards(_player.X, _player.Y, _map, _damageables.OfType<Enemy>().ToList());
            }
        }

        // Sjekk om spilleren har vunnet
        if (_renderables.TrueForAll(r => r is not Enemy) &&
            _player.X == _exit.X && _player.Y == _exit.Y)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GRATULERER! Du har drept alle fiender og funnet utgangen!");
            Console.WriteLine("Du vant spillet!");
            Console.ResetColor();
            _isRunning = false;
        }
    }

    /*Viser en midlertidig melding under kartet i noen sekunder.
      Brukes for feedback som "Potion plukket!" eller "Fiende drept!".*/
    private void ShowStatus(string message)
    {
        _statusMessage = message;
        _messageExpires = DateTime.Now + _messageDuration;
    }
}