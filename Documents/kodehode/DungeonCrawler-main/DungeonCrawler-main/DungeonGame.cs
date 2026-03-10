using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;
using DungeonCrawler.Entities;

namespace DungeonCrawler;

public class DungeonGame : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private SpriteFont _titleFont;
    private Texture2D _pixel;

    private enum GameState { Menu, Playing }
    private GameState _state = GameState.Menu;

    private int _menuIndex = 0;
    private string[] _menuOptions = { "PLAY", "EXIT" };

    private GameMap _map;
    private Player _player;
    private int _tileSize;
    private double _moveTimer = 0;
    private double _moveDelay = 0.15; // sekunder mellom hvert steg
    private List<Enemy> _enemies;

    private KeyboardState _prevKeyboard;

    public DungeonGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 864;
        _graphics.PreferredBackBufferHeight = 648;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

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
        _map = new GameMap();
        _player = new Player(1, 1);
        _enemies = new List<Enemy>
        {
            new Enemy(8, 2),
            new Enemy(6, 4),
            new Enemy(9, 7)
        };
        _tileSize = Math.Min(
        _graphics.PreferredBackBufferWidth / _map.Width,
        _graphics.PreferredBackBufferHeight / _map.Height
        );
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (_state == GameState.Menu)
        {
            if (keyboard.IsKeyDown(Keys.S)) _menuIndex = 1;
            if (keyboard.IsKeyDown(Keys.W)) _menuIndex = 0;
            if (keyboard.IsKeyDown(Keys.Enter))
            {
                if (_menuIndex == 0) _state = GameState.Playing;
                if (_menuIndex == 1) Exit();
            }
        }

            if (_state == GameState.Playing)
            {
                var kb = Keyboard.GetState();
                _moveTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                bool moved = false;
                if (kb.IsKeyDown(Keys.W) && (_prevKeyboard.IsKeyUp(Keys.W) || _moveTimer <= 0)) { _player.TryMove(0, -1, _map); moved = true; }
                if (kb.IsKeyDown(Keys.S) && (_prevKeyboard.IsKeyUp(Keys.S) || _moveTimer <= 0)) { _player.TryMove(0,  1, _map); moved = true; }
                if (kb.IsKeyDown(Keys.A) && (_prevKeyboard.IsKeyUp(Keys.A) || _moveTimer <= 0)) { _player.TryMove(-1, 0, _map); moved = true; }
                if (kb.IsKeyDown(Keys.D) && (_prevKeyboard.IsKeyUp(Keys.D) || _moveTimer <= 0)) { _player.TryMove(1,  0, _map); moved = true; }

                if (moved) _moveTimer = _moveDelay;
                _prevKeyboard = kb;
            }

            base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        if (_state == GameState.Menu)
            DrawMenu();
        else
            DrawGame();

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawMenu()
    {
        // Tittel
        _spriteBatch.DrawString(_titleFont, "DUNGEON CRAWLER", new Vector2(120, 100), Color.Gold);

        // Knapper
        for (int i = 0; i < _menuOptions.Length; i++)
        {
            Color btnColor = i == _menuIndex ? Color.Yellow : Color.DarkGray;
            DrawRect(new Rectangle(300, 270 + i * 80, 200, 50), btnColor);
            Color txtColor = i == _menuIndex ? Color.Black : Color.White;
            _spriteBatch.DrawString(_font, _menuOptions[i], new Vector2(355, 285 + i * 80), txtColor);
        }

        // Instruksjon
        _spriteBatch.DrawString(_font, "W/S = VELG   ENTER = OK", new Vector2(195, 500), Color.DarkGray);
    }
    private void DrawGame()
    {
        for (int y = 0; y < _map.Height; y++)
        {
            for (int x = 0; x < _map.Width; x++)
            {
            char tile = _map.GetTile(x, y);
            Color color = tile == '#' ? Color.DarkBlue : Color.Black;
            DrawRect(new Rectangle(x * _tileSize, y * _tileSize, _tileSize - 1, _tileSize - 1), color);
            }
        }

        // Tegn spiller
        DrawRect(new Rectangle(
        _player.X * _tileSize + 4,
        _player.Y * _tileSize + 4,
        _tileSize - 8,
        _tileSize - 8),
        Color.Yellow);

        // Tegn fiender
        foreach (var e in _enemies)
        {
            DrawRect(new Rectangle(
            e.X * _tileSize + 4,
            e.Y * _tileSize + 4,
            _tileSize - 8,
            _tileSize - 8),
            Color.Red);
        }
    }

    private void DrawRect(Rectangle rect, Color color)
    {
        _spriteBatch.Draw(_pixel, rect, color);
    }
}