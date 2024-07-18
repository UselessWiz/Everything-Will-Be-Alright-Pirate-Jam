using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Engine.Global;

namespace JamGame;

public class InitialScene : IScene
{
	public Game1 gameManager {get; set;}
	private SpriteFont debugFont;

	private CutsceneData[] screens;
	private int currentScreenIndex = 0;

	protected Song backgroundMusic;

	public InitialScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		this.screens = new CutsceneData[] { // Define them in here
			new CutsceneData("Backgrounds/Bedroom1", new string[] {"Humans have souls.", "These are the culmination of their being."}, 
				new Vector2[] {new Vector2(10, 20), new Vector2(140, 200)}, new float[] {2f, 6f, 10f}, gameManager.Content)
		};

		LoadContent();

		MediaPlayer.Play(backgroundMusic);
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
		backgroundMusic = gameManager.Content.Load<Song>("Audio/CutsceneBGM");
	}

	public void Update(GameTime gameTime)
	{
		screens[currentScreenIndex].Update(gameTime);

		if (screens[currentScreenIndex].dataComplete == true) {
			currentScreenIndex += 1;

			if (currentScreenIndex >= screens.Length) {
				MediaPlayer.Stop();
				gameManager.SwitchScene(new BattleScene(gameManager));
			}
		}
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		_spriteBatch.Begin();
		screens[currentScreenIndex].Draw(_spriteBatch);
		_spriteBatch.End();
	}

	public void DrawUI(SpriteBatch _spriteBatch)
	{
		_spriteBatch.Begin();
		screens[currentScreenIndex].DrawUI(_spriteBatch);
		_spriteBatch.End();
	}

	public void DrawDebug(GameTime gameTime)
    {
        Globals.spriteBatch.DrawString(debugFont, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2 (10, 10), Color.Red);    
    }
}