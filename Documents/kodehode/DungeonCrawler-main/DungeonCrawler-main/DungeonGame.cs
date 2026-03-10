using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler;

public class DungeonGame : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Spilltilstand: Meny eller Spill
    private enum GameState { Menu, Playing }
    private GameState _state = GameState.Menu;

    // Meny-valg
    private int _menuIndex = 0;
    private string[] _menuOptions = { "Play", "Exit" };

    // Font for å tegne tekst
    private SpriteFont _font;

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
        // Tegn tittel
        DrawRect(new Rectangle(250, 80, 300, 80), Color.DarkBlue);
        // Tegn meny-knapper
        for (int i = 0; i < _menuOptions.Length; i++)
        {
            Color color = i == _menuIndex ? Color.Yellow : Color.White;
            DrawRect(new Rectangle(300, 250 + i * 80, 200, 50), color);
        }
    }

    private void DrawGame()
    {
        // Her tegner vi kartet senere
        DrawRect(new Rectangle(100, 100, 50, 50), Color.Yellow); // Spiller placeholder
    }

    // Hjelpemetode for å tegne fargede rektangler
    private Texture2D _pixel;
    private void DrawRect(Rectangle rect, Color color)
    {
        if (_pixel == null)
        {
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
        _spriteBatch.Draw(_pixel, rect, color);
    }
}