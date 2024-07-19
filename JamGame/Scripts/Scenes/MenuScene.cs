using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Engine.Global;

namespace JamGame;

public class MenuScene : IScene
{
	public Game1 gameManager {get; set;}

	private SpriteFont debugFont;
	private SpriteFont gothicFont;
	private SpriteFont gothicFontSmall;

	public MenuScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		LoadContent();
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
		gothicFont = gameManager.Content.Load<SpriteFont>("Gothic Pixels Title");
		gothicFontSmall = gameManager.Content.Load<SpriteFont>("Gothic Pixels Subtitle");
	}

	public void Update(GameTime gameTime)
	{
		if (Mouse.GetState().LeftButton == ButtonState.Pressed) gameManager.SwitchScene(new InitialScene(gameManager));
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		// Clear this buffer.
        gameManager.GraphicsDevice.Clear(Color.CornflowerBlue);
	}

	public void DrawUI(SpriteBatch _spriteBatch)
	{
		_spriteBatch.Begin();
		_spriteBatch.DrawString(gothicFont, "Everything Will Be Alright", new Vector2(50, 50), Color.Black);
		_spriteBatch.DrawString(gothicFontSmall, "Click To Play", new Vector2(120, 180), Color.Black);
		_spriteBatch.End();
	}

	public void DrawDebug(GameTime gameTime)
    {
        Globals.spriteBatch.DrawString(debugFont, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2 (10, 10), Color.Red);    
    }
}