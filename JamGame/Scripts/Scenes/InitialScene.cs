using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Engine.Global;

namespace JamGame;

public class InitialScene : IScene
{
	public Game1 gameManager {get; set;}
	private SpriteFont debugFont;

	private CutsceneData[] screens;
	private int currentScreenIndex = 0;

	protected SoundEffect backgroundMusic;
	private SoundEffectInstance musicPlayer;

	public InitialScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		gameManager.ChangeResolution(new Point(1280, 960));

		this.screens = new CutsceneData[] { // Define them in here
			new CutsceneData("Backgrounds/Hands1", 
				new string[] {"Eventually, once people connect closely enough with others...", "... their souls and shadows combine."}, 
				new Vector2[] {new Vector2(90, 100), new Vector2(680, 850)}, new Color[] {Color.Black, Color.Black}, 
				new float[] {2f, 6f, 10f}, gameManager.Content),
			new CutsceneData("Backgrounds/Hands2",
				new string[] {"On its own, a person's shadow feeds back their own negativity.", 
					"But when shadows combine, the fears the combined souls share are\namplified beyond the negativity of any one person."},
				new Vector2[] {new Vector2(40, 120), new Vector2(100, 880)}, new Color[] {Color.Black, Color.White}, 
				new float[] {12f, 18f, 24f}, gameManager.Content)
		};

		LoadContent();

		musicPlayer = backgroundMusic.CreateInstance();
		musicPlayer.Play();
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
		backgroundMusic = gameManager.Content.Load<SoundEffect>("Audio/CutsceneBGM");
	}

	public void Update(GameTime gameTime)
	{
		screens[currentScreenIndex].Update(gameTime);

		if (screens[currentScreenIndex].dataComplete == true) {
			currentScreenIndex += 1;

			if (currentScreenIndex >= screens.Length) {
				musicPlayer.Stop();
				BattleScene battleScene = new BattleScene(gameManager);
				battleScene.sceneTime = (float)gameTime.TotalGameTime.TotalSeconds;
				gameManager.SwitchScene(battleScene);
			}
		}

		// DEBUG ----------------------------------------------------
		if (KeyboardExtended.KeyPressed(Keys.Space)) {
			musicPlayer.Stop();
			BattleScene battleScene = new BattleScene(gameManager);
			battleScene.sceneTime = (float)gameTime.TotalGameTime.TotalSeconds;
			gameManager.SwitchScene(battleScene);
		}
		// ----------------------------------------------------------
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		// Clear this buffer.
        gameManager.GraphicsDevice.Clear(Color.CornflowerBlue);
        
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