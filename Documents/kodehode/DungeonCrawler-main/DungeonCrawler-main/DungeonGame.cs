using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;

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

    public DungeonGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
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
        // Her tegner vi kartet senere
        DrawRect(new Rectangle(100, 100, 30, 30), Color.Yellow);
        _spriteBatch.DrawString(_font, "SPILLET STARTER SNART!", new Vector2(250, 280), Color.White);
    }

    private void DrawRect(Rectangle rect, Color color)
    {
        _spriteBatch.Draw(_pixel, rect, color);
    }
}