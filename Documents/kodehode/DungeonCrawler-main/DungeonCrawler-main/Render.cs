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

    // Konstruktøren tar imot alle ressurser den trenger utenfra (dependency injection).
    // Renderer lager ikke disse selv, den får dem fra DungeonGame som eier dem.
    public Renderer(SpriteBatch spriteBatch, SpriteFont font, SpriteFont titleFont, Texture2D pixel, GraphicsDeviceManager graphics)
    {
        _spriteBatch = spriteBatch;
        _font = font;
        _titleFont = titleFont;
        _pixel = pixel;
        _graphics = graphics;
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
         // Tegn kartet tile for tile. Rutekoordinat (x,y) → pikselposisjon = koordinat * tileSize
        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                char tile = map.GetTile(x, y);
                Color color = tile == '#' ? Color.DarkBlue : Color.Black;
                 // -1 på størrelsen lager en liten margin mellom tiles (grid-effekt)
                DrawRect(new Rectangle(x * tileSize, y * tileSize, tileSize - 1, tileSize - 1), color);
            }
        }

        // Spiller tegnes som gul firkant, +4 og -8 gir margin så den ikke fyller hele tilen
        DrawRect(new Rectangle(player.X * tileSize + 4, player.Y * tileSize + 4, tileSize - 8, tileSize - 8), Color.Yellow);

        // Fiender tegnes som røde firkanter
        foreach (var e in enemies)
            DrawRect(new Rectangle(e.X * tileSize + 4, e.Y * tileSize + 4, tileSize - 8, tileSize - 8), Color.Red);

        // Potions tegnes som magenta firkanter
        foreach (var p in potions)
            DrawRect(new Rectangle(p.X * tileSize + 4, p.Y * tileSize + 4, tileSize - 8, tileSize - 8), Color.Magenta);

        // Utgangen vises kun som grønn firkant når alle fiender er drept
        if (exitOpen)
            DrawRect(new Rectangle(exitX * tileSize + 4, exitY * tileSize + 4, tileSize - 8, tileSize - 8), Color.LimeGreen);

        // HP-bar nederst til venstre
        // Består av en rød bakgrunn og en grønn del som krymper proporsjonalt med HP
        int barWidth = 300;
        int barHeight = 20;
        int barX = 20;
        int barY = _graphics.PreferredBackBufferHeight - 40;

        DrawRect(new Rectangle(barX, barY, barWidth, barHeight), Color.DarkRed);
        // filledWidth beregnes som andel av maks HP – går mot 0 når spilleren nærmer seg døden
        int filledWidth = (int)(barWidth * (player.Health / 100.0));
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
}