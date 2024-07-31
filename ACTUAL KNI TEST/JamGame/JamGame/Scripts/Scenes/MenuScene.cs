using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Engine.Global;

namespace JamGame;

public class MenuScene : IScene
{
	public Game1 gameManager {get; set;}

	private SpriteFont debugFont;
	private SpriteFont gothicFont;
	private SpriteFont gothicFontSmall;
	private SoundEffect clickSound;

	private Sprite menuBackground;

	private float startGameTimer;

	public MenuScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		gameManager.ChangeResolution(new Point(1280, 960));

		menuBackground = new Sprite(new Vector2(640, 480), "Backgrounds/Menu", gameManager.Content);

		LoadContent();
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
		gothicFont = gameManager.Content.Load<SpriteFont>("Gothic Pixels Title");
		gothicFontSmall = gameManager.Content.Load<SpriteFont>("Gothic Pixels Subtitle");
		clickSound = gameManager.Content.Load<SoundEffect>("Audio/SFX/Menu Click");
	}

	public void Update(GameTime gameTime)
	{
		if (Mouse.GetState().LeftButton == ButtonState.Pressed && startGameTimer == 0) {
			startGameTimer = (float)gameTime.TotalGameTime.TotalSeconds + 2f;
			clickSound.Play();
		}

		if (startGameTimer > 0 && (float)gameTime.TotalGameTime.TotalSeconds >= startGameTimer) gameManager.SwitchScene(new InitialScene(gameManager));
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		// Clear this buffer.
        gameManager.GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();
        menuBackground.Draw(_spriteBatch);
        _spriteBatch.End();
	}

	public void DrawUI(SpriteBatch _spriteBatch)
	{
		_spriteBatch.Begin();
		_spriteBatch.DrawString(gothicFont, "Everything Will Be Alright", new Vector2(236, 200), Color.Black);
		_spriteBatch.DrawString(gothicFontSmall, "Click To Play", new Vector2(538, 720), Color.Black);
		_spriteBatch.End();
	}

	public void DrawDebug(GameTime gameTime)
    {
        Globals.spriteBatch.DrawString(debugFont, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2 (10, 10), Color.Red);    
    }
}