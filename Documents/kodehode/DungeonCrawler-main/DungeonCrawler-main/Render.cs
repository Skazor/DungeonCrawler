using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonCrawler.Entities;

namespace DungeonCrawler;

// Renderer-klassen har ett ansvar: tegne spillets visuelle elementer.
// Dette er "separation of concerns", spillogikk ligger i DungeonGame.cs,
// mens alt som vises på skjermen håndteres her.
public class Renderer
{
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private SpriteFont _titleFont;
    private Texture2D _pixel;
    private GraphicsDeviceManager _graphics;
     private Texture2D _tilesetTexture;  
    private Texture2D _playerTexture;  
    private Texture2D _enemyTexture; 

    // Konstruktøren tar imot alle ressurser den trenger utenfra (dependency injection).
    // Renderer lager ikke disse selv, den får dem fra DungeonGame som eier dem.
    public Renderer(SpriteBatch spriteBatch, SpriteFont font, SpriteFont titleFont, Texture2D pixel, GraphicsDeviceManager graphics, Texture2D tilesetTexture, Texture2D playerTexture, Texture2D enemyTexture)
    {
        _spriteBatch = spriteBatch;
        _font = font;
        _titleFont = titleFont;
        _pixel = pixel;
        _graphics = graphics;
        _tilesetTexture = tilesetTexture; 
        _playerTexture = playerTexture;    
        _enemyTexture = enemyTexture; 
    }

    // Tegner hovedmenyen med tittel, valgknapper og instruksjonstekst.
    // menuIndex forteller hvilken knapp som er valgt (markeres med gul farge)
    public void DrawMenu(int menuIndex, string[] menuOptions)
    {
        _spriteBatch.DrawString(_titleFont, "DUNGEON CRAWLER", new Vector2(120, 100), Color.Gold);

        for (int i = 0; i < menuOptions.Length; i++)
        {
            // Valgt knapp får gul bakgrunn og svart tekst, uvalgte får grå bakgrunn og hvit tekst
            Color btnColor = i == menuIndex ? Color.Yellow : Color.DarkGray;
            DrawRect(new Rectangle(300, 270 + i * 80, 200, 50), btnColor);
            Color txtColor = i == menuIndex ? Color.Black : Color.White;
            _spriteBatch.DrawString(_font, menuOptions[i], new Vector2(355, 285 + i * 80), txtColor);
        }

        _spriteBatch.DrawString(_font, "W/S = VELG   ENTER = OK", new Vector2(195, 500), Color.DarkGray);
    }

    // Tegner selve spillskjermen: kart, spiller, fiender, potions, utgang og HP-bar.
    // Tar imot all spilltilstand den trenger som parametere, ingen direkte tilgang til DungeonGame
    public void DrawGame(GameMap map, Player player, List<Enemy> enemies, List<(int X, int Y)> potions, bool exitOpen, int exitX, int exitY, int tileSize)
{
    // Tegn kartet
    for (int y = 0; y < map.Height; y++)
{
    for (int x = 0; x < map.Width; x++)
    {
        char tile = map.GetTile(x, y);
        if (tile == '#')
        {
            // Vegger tegnes svart – usynlig bakgrunn
            DrawRect(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), Color.Black);
        }
        else
        {
            // Gulv tegnes med sprite – kun én tile fra tilesettet (første tile: 0,0,16,16)
            _spriteBatch.Draw(
                _tilesetTexture,
                new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize),
                new Rectangle(0, 0, 16, 16),  // ← henter første tile fra atlas
                Color.White
            );
        }
    }
}

    // Tegn spiller som sprite
    _spriteBatch.Draw(_playerTexture, new Rectangle(player.X * tileSize, player.Y * tileSize, tileSize, tileSize), Color.White);

    // Tegn fiender som sprite
    foreach (var e in enemies)
        _spriteBatch.Draw(_enemyTexture, new Rectangle(e.X * tileSize, e.Y * tileSize, tileSize, tileSize), Color.White);

    // Potions tegnes fortsatt som magenta firkanter (foreløpig)
    foreach (var p in potions)
        DrawRect(new Rectangle(p.X * tileSize + 4, p.Y * tileSize + 4, tileSize - 8, tileSize - 8), Color.Magenta);

    // Utgang
    if (exitOpen)
        DrawRect(new Rectangle(exitX * tileSize + 4, exitY * tileSize + 4, tileSize - 8, tileSize - 8), Color.LimeGreen);

    // HP-bar
    int barWidth = 300;
    int barHeight = 20;
    int barX = 20;
    int barY = _graphics.PreferredBackBufferHeight - 50;

    DrawRect(new Rectangle(barX, barY, barWidth, barHeight), Color.DarkRed);
    int filledWidth = (int)(barWidth * (player.Health / (double)player.Class.MaxHealth));
    DrawRect(new Rectangle(barX, barY, filledWidth, barHeight), Color.Green);
    _spriteBatch.DrawString(_font, $"HP: {player.Health}", new Vector2(barX + barWidth + 10, barY), Color.White);
}

    // Tegner Game Over-skjermen med rød tekst og instruksjon om å starte på nytt
    public void DrawGameOver()
    {
        _spriteBatch.DrawString(_titleFont, "GAME OVER", new Vector2(220, 250), Color.Red);
        _spriteBatch.DrawString(_font, "TRYKK ENTER FOR A STARTE IGJEN", new Vector2(100, 350), Color.White);
    }

    // Tegner vinn-skjermen med gull tekst og instruksjon om å starte på nytt
    public void DrawVictory()
    {
        _spriteBatch.DrawString(_titleFont, "YOU WIN!", new Vector2(270, 250), Color.Gold);
        _spriteBatch.DrawString(_font, "TRYKK ENTER FOR A STARTE IGJEN", new Vector2(100, 350), Color.White);
    }

    // Hjelpemetode: tegner et farget rektangel ved å strekke 1x1-pikselen til ønsket størrelse.
    // Brukes av alle Draw-metodene, DRY-prinsippet (Don't Repeat Yourself)
    private void DrawRect(Rectangle rect, Color color)
    {
        _spriteBatch.Draw(_pixel, rect, color);
    }

    public void DrawCharacterCreation(CharacterClass[] classes, int index)
    {
        var c = classes[index];

        _spriteBatch.DrawString(_titleFont, "CHOOSE CLASS", new Vector2(170, 60), Color.Gold);

        // Pil-navigasjon
        _spriteBatch.DrawString(_font, "< A", new Vector2(60, 300), Color.DarkGray);
        _spriteBatch.DrawString(_font, "D >", new Vector2(740, 300), Color.DarkGray);

        // Klassenavn
        _spriteBatch.DrawString(_titleFont, c.Name, new Vector2(400 - c.Name.Length * 10, 160), Color.Yellow);

        // Beskrivelse
        _spriteBatch.DrawString(_font, c.Description, new Vector2(100, 230), Color.White);

        // Stats-boks
        DrawRect(new Rectangle(250, 270, 364, 200), Color.DarkSlateGray);

        _spriteBatch.DrawString(_font, $"HP:      {c.MaxHealth}", new Vector2(270, 290), Color.LimeGreen);
        _spriteBatch.DrawString(_font, $"ATTACK:  {c.AttackPower}", new Vector2(270, 320), Color.OrangeRed);
        _spriteBatch.DrawString(_font, $"DEFENSE: {c.Defense}", new Vector2(270, 350), Color.CornflowerBlue);
        _spriteBatch.DrawString(_font, $"CRIT:    {(int)(c.CritChance * 100)}%", new Vector2(270, 380), Color.Violet);
        _spriteBatch.DrawString(_font, $"SPEED:   {(c.MoveDelay <= 0.10 ? "FAST" : c.MoveDelay <= 0.14 ? "NORMAL" : "SLOW")}", new Vector2(270, 410), Color.Gold);

        // Klasse-indikatorer nederst
        for (int i = 0; i < classes.Length; i++)
        {
            Color dotColor = i == index ? Color.Yellow : Color.DarkGray;
            DrawRect(new Rectangle(350 + i * 30, 500, 15, 15), dotColor);
        }

        _spriteBatch.DrawString(_font, "ENTER = VELG", new Vector2(310, 540), Color.DarkGray);
    }
}