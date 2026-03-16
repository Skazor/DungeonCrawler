using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;
using DungeonCrawler.Entities;

namespace DungeonCrawler;

public class DungeonGame : Microsoft.Xna.Framework.Game
{
    // GraphicsDeviceManager håndterer skjerminnstillinger som oppløsning og fullskjerm
    private GraphicsDeviceManager _graphics;
    // SpriteBatch er MonoGames "tegne-kø" – samler opp alt som skal tegnes, og sender det til grafikkortet på en gang 
    private SpriteBatch _spriteBatch;
    // SpriteFont er en ferdig-bakt font som GPU-en kan tegne direkte
    private SpriteFont _font;
    private SpriteFont _titleFont;

    // En 1x1 piksel tekstur – brukes som "pensel" for å tegne fargede rektangler
    // ved å strekke den til ønsket størrelse
    private Texture2D _pixel;
    // GameState er en enum (begrenset liste med tilstander).
    // Dette mønsteret kalles en "state machine", veldig vanlig i spill
    private GameState _state = GameState.Menu;

    private int _menuIndex = 0;
    private string[] _menuOptions = { "PLAY", "EXIT" };
    private GameMap _map;
    private Player _player;
    private int _tileSize;

    // Timere brukes til å kontrollere HVOR OFTE noe skjer,
    // uavhengig av hvor fort spillet kjører (frame rate)
    private double _moveTimer = 0;
    private double _moveDelay = 0.15; // sekunder mellom hvert steg
    private double _enemyTimer = 0;
    private double _enemyDelay = 0.5; // fiender beveger seg hvert 0.5 sekund
    private List<Enemy> _enemies;
    
    // lagrer forrige frames tastaturtilstand for å oppdage
    // når en tast nettopp BLE trykket (ikke bare holdes nede)
    private KeyboardState _prevKeyboard;

    // Enum definert inne i klassen – kun synlig her (private scope)
    private enum GameState { Menu, CharacterCreation, Playing, GameOver, Victory }

    private List<(int X, int Y)> _potions = new();
    private bool _exitOpen = false;
    private int _exitX = 10;
    private int _exitY = 7;
    private Renderer _renderer;
    private CharacterClass[] _classes = new[]
    {
        new CharacterClass(ClassType.Warrior),
        new CharacterClass(ClassType.Mage),
        new CharacterClass(ClassType.Rogue),
        new CharacterClass(ClassType.Paladin),
        new CharacterClass(ClassType.Hunter)
    };
        private int _classIndex = 0;

    public DungeonGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        // Setter vindusstørrelsen i piksler
        _graphics.PreferredBackBufferWidth = 864;
        _graphics.PreferredBackBufferHeight = 648;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    // LoadContent() kjøres en gang ved oppstart – last inn teksturer, fonter, evnt lyder her
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Last inn fonten fra .ttf-filen
        var fontBytes = File.ReadAllBytes("PressStart2P-Regular.ttf");
        _font = TtfFontBaker.Bake(fontBytes, 16, 1024, 1024,
        new[] { CharacterRange.BasicLatin }).CreateSpriteFont(GraphicsDevice);
        _titleFont = TtfFontBaker.Bake(fontBytes, 32, 1024, 1024,
        new[] { CharacterRange.BasicLatin }).CreateSpriteFont(GraphicsDevice);

        // Lag en 1x1 hvit tekstur for å tegne rektangler
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _renderer = new Renderer(_spriteBatch, _font, _titleFont, _pixel, _graphics);

        InitializeGame();
    }

    private void InitializeGame()
    {
        var rng = new Random();
        _map = new GameMap(24, 16);

        var firstRoom = _map.Rooms[0];
        _player = new Player(firstRoom.CenterX, firstRoom.CenterY, _classes[_classIndex]);

        _enemies = new List<Enemy>();
        for (int i = 1; i < Math.Min(4, _map.Rooms.Count); i++)
        {
            var room = _map.Rooms[i];
            _enemies.Add(new Enemy(room.CenterX, room.CenterY));
        }

        _potions = new List<(int X, int Y)>();
        for (int i = 4; i < Math.Min(6, _map.Rooms.Count); i++)
        {
            var room = _map.Rooms[i];
            _potions.Add((room.CenterX, room.CenterY));
        }

        var lastRoom = _map.Rooms[_map.Rooms.Count - 1];
        _exitX = lastRoom.CenterX;
        _exitY = lastRoom.CenterY;
        _exitOpen = false;

        _tileSize = Math.Min(
        _graphics.PreferredBackBufferWidth / _map.Width,
        _graphics.PreferredBackBufferHeight / _map.Height
        );
    }

    // Update() kjøres hver frame – all spillogikk (input, fysikk, AI) hører hjemme her
    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (_state == GameState.Menu)
        {
            if (keyboard.IsKeyDown(Keys.S)) _menuIndex = 1;
            if (keyboard.IsKeyDown(Keys.W)) _menuIndex = 0;
            if (keyboard.IsKeyDown(Keys.Enter))
            {
                if (_menuIndex == 0) _state = GameState.CharacterCreation; 
                if (_menuIndex == 1) Exit();
            }

                _prevKeyboard = keyboard;
        }

        if (_state == GameState.CharacterCreation)
        {
            if (keyboard.IsKeyDown(Keys.D) && _prevKeyboard.IsKeyUp(Keys.D))
                _classIndex = (_classIndex + 1) % _classes.Length;
            if (keyboard.IsKeyDown(Keys.A) && _prevKeyboard.IsKeyUp(Keys.A))
                _classIndex = (_classIndex - 1 + _classes.Length) % _classes.Length;
            if (keyboard.IsKeyDown(Keys.Enter) && _prevKeyboard.IsKeyUp(Keys.Enter))
            {
                InitializeGame();
                _state = GameState.Playing;
            }   
                _prevKeyboard = keyboard;
        }

            if (_state == GameState.Playing)
            {
                var kb = Keyboard.GetState();

                // Trekk fra tid siden forrige frame – når _moveTimer når 0 kan spilleren flytte seg igjen
                // gameTime.ElapsedGameTime.TotalSeconds er typisk 0.016 ved 60 FPS
                _moveTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                bool moved = false;

                // IsKeyUp(_prevKeyboard) sjekker om tasten var OPPE forrige frame = første trykk
                // ELLER timeren har gått ut = tillat bevegelse mens tasten holdes nede
                // gir responsiv kontroll uten at spilleren flyr av gårde
                if (kb.IsKeyDown(Keys.W) && (_prevKeyboard.IsKeyUp(Keys.W) || _moveTimer <= 0)) { _player.TryMove(0, -1, _map); moved = true; }
                if (kb.IsKeyDown(Keys.S) && (_prevKeyboard.IsKeyUp(Keys.S) || _moveTimer <= 0)) { _player.TryMove(0,  1, _map); moved = true; }
                if (kb.IsKeyDown(Keys.A) && (_prevKeyboard.IsKeyUp(Keys.A) || _moveTimer <= 0)) { _player.TryMove(-1, 0, _map); moved = true; }
                if (kb.IsKeyDown(Keys.D) && (_prevKeyboard.IsKeyUp(Keys.D) || _moveTimer <= 0)) { _player.TryMove(1,  0, _map); moved = true; }

                if (moved) _moveTimer = _moveDelay; // Reset timer etter bevegelse
                _prevKeyboard = kb; // Husk denne framens tilstand til neste frame

                // Sjekk potion-kollisjon
                for (int i = _potions.Count - 1; i >= 0; i--)
                {
                    if (_potions[i].X == _player.X && _potions[i].Y == _player.Y)
                    {
                        _player.Heal(30);
                        _potions.RemoveAt(i);
                    }
                }

                // Flytt fiender mot spilleren
                // Fiende-AI kjører på en egen timer, saktere enn spilleren
                _enemyTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_enemyTimer <= 0)
                {
                    foreach (var enemy in _enemies)
                    enemy.MoveTowards(_player.X, _player.Y, _map, _enemies);
                    _enemyTimer = _enemyDelay; //reset timer
                }
                // Sjekk kollisjon med fiender
                // Itererer baklengs gjennom listen fordi vi kan fjerne elementer underveis.
                // Hvis vi gikk forover og fjernet index 2, ville index 3 bli hoppet over
                for (int i = _enemies.Count - 1; i >= 0; i--)
                {
                    
                    // Enkel kollisjon: samme ruteposisjon = treff 
                    if (_enemies[i].X == _player.X && _enemies[i].Y == _player.Y)
                    {
                        
                        // Begge parter tar skade samtidig – symmetrisk kamp
                        _player.TakeDamage(_enemies[i].AttackPower);
                        _enemies[i].TakeDamage(_player.AttackPower);

                        if (!_enemies[i].IsAlive)
                            _enemies.RemoveAt(i); // Fjern død fiende fra listen

                        if (!_player.IsAlive)
                        {
                            _state = GameState.GameOver;
                            return; // return avslutter Update() umiddelbart, for å unngå krasj
                        }
                    }
                }

                // Sjekk om alle fiender er drept
                if (_enemies.Count == 0)
                 _exitOpen = true;

                // Sjekk om spilleren går ut
                if (_exitOpen && _player.X == _exitX && _player.Y == _exitY)
                {
                    _state = GameState.Victory;
                    return;
                }
            }
                if (_state == GameState.GameOver) 
                {
                    if (keyboard.IsKeyDown(Keys.Enter))
                    {
                        InitializeGame();
                        _state = GameState.Playing;
                    }
                }

                if (_state == GameState.Victory)
                {
                    if (keyboard.IsKeyDown(Keys.Enter))
                    {
                        InitializeGame();
                        _state = GameState.Playing;
                    }
                }
            

            base.Update(gameTime); // Kall alltid base-metoden, MonoGame trenger dette internt
    }

    // Draw() kjøres hver frame etter Update(), KUN tegning her, ingen spillogikk
    protected override void Draw(GameTime gameTime)
    {
        // Tøm skjermen med svart bakgrunn før det blir tegnet ny frame
        GraphicsDevice.Clear(Color.Black);
        
        // Begin() og End() wrapper all SpriteBatch-tegning.
        // Alt mellom disse samles opp og sendes på en gang
         _spriteBatch.Begin();
         
         if (_state == GameState.Menu)
            _renderer.DrawMenu(_menuIndex, _menuOptions);
        else if (_state == GameState.Playing)
            _renderer.DrawGame(_map, _player, _enemies, _potions, _exitOpen, _exitX, _exitY, _tileSize);
        else if (_state == GameState.GameOver)
            _renderer.DrawGameOver();
        else if (_state == GameState.Victory)
            _renderer.DrawVictory();

        else if (_state == GameState.CharacterCreation)
            _renderer.DrawCharacterCreation(_classes, _classIndex);

        _spriteBatch.End();
        base.Draw(gameTime);
    }


}