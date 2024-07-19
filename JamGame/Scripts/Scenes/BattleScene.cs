using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Global;
using Engine.Animations;

namespace JamGame;

public class BattleScene : IScene
{
	public Game1 gameManager {get; set;}
	private SpriteFont debugFont;

	private HealthBar bossHealthBar;
	private HealthBar[] finalHealthBars;

	private AnimationClip finalHealthBarAnimation;

	private bool bossDefeated = false;

	public BattleScene(Game1 gameManager)
	{
		this.gameManager = gameManager;

		this.bossHealthBar = new HealthBar(new Vector2(155, 6), "Sprite/UI/HealthBarFG", 300, gameManager.Content);

		PrepareFinalHealthBars();

		LoadContent();
	}

	public void LoadContent()
	{
		debugFont = gameManager.Content.Load<SpriteFont>("DebugFont");
	}

	public void Update(GameTime gameTime)
	{
		if (!bossDefeated) {
			BossKilled();
		}

		else {
			finalHealthBarAnimation.Update(gameTime);

			for (int i = 0; i < 49; i++) {
				finalHealthBars[i].currentValue = (int)finalHealthBarAnimation.values[i];
				finalHealthBars[i].currentSprite = finalHealthBars[i].ChangeHealthBarValue(finalHealthBars[i].currentValue);
			}
		}
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		// Clear this buffer.
        gameManager.GraphicsDevice.Clear(Color.Black);
	}

	public void DrawUI(SpriteBatch _spriteBatch)
	{
		_spriteBatch.Begin();
		//bossHealthBar.DrawUI(_spriteBatch);
		_spriteBatch.End();

		if (!bossDefeated) return;

		_spriteBatch.Begin();

		foreach (HealthBar bar in finalHealthBars) {
			bar.drawn = false;
		}

		foreach (HealthBar bar in finalHealthBars) {
			if (bar.currentValue == 300) {
				bar.DrawUI(_spriteBatch);
				bar.drawn = true;
			}
		}

		foreach (HealthBar bar in finalHealthBars) {
			if (!bar.drawn) {
				bar.DrawUI(_spriteBatch);
			}
		}

		_spriteBatch.End();
	}

	public void DrawDebug(GameTime gameTime)
    {
        Globals.spriteBatch.DrawString(debugFont, $"FPS: {(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2 (10, 10), Color.Red);    
    }

    private void PrepareFinalHealthBars()
    {
    	this.finalHealthBars = new HealthBar[49];
    	Vector2[] finalHealthBarPositions = new Vector2[49];

    	// Set up positions and offsets for each healthbar
    	for (int i = 0; i < 24; i++) {
    		finalHealthBarPositions[i] = new Vector2(155, 10 * i);
    	}

    	for (int i = 24; i < 48; i++) {
			finalHealthBarPositions[i] = new Vector2(155, (10 * (i - 24)) - 5);
    	}

    	finalHealthBarPositions[48] = new Vector2(155, 245);

    	// Set up the animation data
    	float[] keyframeTimes = new float[49];
    	Animation[][] keyframeData = new Animation[49][];

    	for (int i = 0; i < 49; i++) {
    		finalHealthBars[i] = new HealthBar(finalHealthBarPositions[i], "Sprite/UI/HealthBarFG", 0, gameManager.Content);
    		keyframeTimes[i] = 0.5f * i;
    		keyframeData[i] = new Animation[1];
    		keyframeData[i][0] = new Animation(i, 0f, 300f, 0.5f);
    	}

    	// Create the animation
    	this.finalHealthBarAnimation = new AnimationClip(49, keyframeTimes, keyframeData, 25f, true); 
    }

    public void BossKilled()
    {
    	bossDefeated = true;
    	finalHealthBarAnimation.playing = true;
    }
}