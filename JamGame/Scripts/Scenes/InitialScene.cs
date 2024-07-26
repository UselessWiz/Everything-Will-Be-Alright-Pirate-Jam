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

	private Effect filmGrainShader;

	public InitialScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		gameManager.ChangeResolution(new Point(1280, 960));

		this.screens = new CutsceneData[] { // Define them in here
			new CutsceneData("Backgrounds/Person1", 
				new string[] {"We might think we're complex, but we're only made up of two parts.", "The first is our soul, the core of our being."}, 
				new Vector2[] {new Vector2(200, 20), new Vector2(560, 920)}, new Color[] {Color.Black, Color.Black}, 
				new float[] {2f, 8f, 14f}, gameManager.Content),
			new CutsceneData("Backgrounds/Person2", 
				new string[] {"The other is our shadow, a place the light never touches.", "Almost like the soul's negative photo."}, 
				new Vector2[] {new Vector2(100, 50), new Vector2(550, 920)}, new Color[] {Color.Black, Color.White}, 
				new float[] {16f, 22f, 28f}, gameManager.Content),
			new CutsceneData("Backgrounds/MonsterUnderBed", 
				new string[] {"Shadows are fuelled by fear."}, 
				new Vector2[] {new Vector2(70, 850)}, new Color[] {Color.White}, 
				new float[] {30f, 36f}, gameManager.Content),
			new CutsceneData("Backgrounds/PhoneConnection", 
				new string[] {"As people connect with others, their souls become closer together.", "So do their shadows."}, 
				new Vector2[] {new Vector2(110, 90), new Vector2(490, 760)}, new Color[] {Color.White, Color.Black}, 
				new float[] {38f, 44f, 50f}, gameManager.Content),
			new CutsceneData("Backgrounds/Hands1", 
				new string[] {"Eventually, once people connect closely enough with others...", "... their souls and shadows combine."}, 
				new Vector2[] {new Vector2(90, 100), new Vector2(680, 850)}, new Color[] {Color.Black, Color.Black}, 
				new float[] {52f, 58f, 64f}, gameManager.Content),
			new CutsceneData("Backgrounds/Hands2",
				new string[] {"On its own, a person's shadow feeds back their own negativity.", 
					"But when shadows combine, the fears the combined souls share are\namplified beyond the negativity of any one person."},
				new Vector2[] {new Vector2(40, 120), new Vector2(100, 880)}, new Color[] {Color.White, Color.White}, 
				new float[] {66f, 72f, 78f}, gameManager.Content),
			new CutsceneData("Backgrounds/WrongMerge", 
				new string[] {"If the wrong people connect, and the wrong fears merge together..."}, 
				new Vector2[] {new Vector2(90, 420)}, new Color[] {Color.White}, 
				new float[] {80f, 86f}, gameManager.Content),
			new CutsceneData("Backgrounds/FEAR MUST CONSUME", 
				new string[] {" "}, 
				new Vector2[] {new Vector2(90, 100)}, new Color[] {Color.Black}, 
				new float[] {88f, 96f}, gameManager.Content)
		};

		LoadContent();

		musicPlayer = backgroundMusic.CreateInstance();
		musicPlayer.Play();
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
		backgroundMusic = gameManager.Content.Load<SoundEffect>("Audio/CutsceneBGM");
		filmGrainShader = gameManager.Content.Load<Effect>("Shaders/Film Grain");
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

		filmGrainShader.Parameters["TotalGameTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);

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
        
		_spriteBatch.Begin(effect: filmGrainShader);
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